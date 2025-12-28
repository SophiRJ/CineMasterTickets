
let currentMovieId = null;

document.addEventListener("DOMContentLoaded", () => {

    // --- 1. CAPTURA DE ELEMENTOS DEL DOM ---
    const btnSaveSession = document.getElementById("btnSaveSession");
    const sessionModalElement = document.getElementById("sessionModal");
    // Inicializamos el objeto Modal de Bootstrap una sola vez
    const bsModal = new bootstrap.Modal(sessionModalElement);

    // --- 2. ASIGNACIÓN DE EVENTOS CON LAMBDAS ---

    // Evento para el botón GUARDAR del modal
    btnSaveSession.addEventListener("click", () => saveSession(bsModal));

    // DELEGACIÓN DE EVENTOS (Para botones que aparecen dinámicamente)
    document.addEventListener("click", (e) => {

        // Botón "Ver Sesiones"
        if (e.target.classList.contains("btn-load-sessions")) {
            const movieId = e.target.getAttribute("data-movie-id");
            loadSessions(movieId);
        }

        // Botón "Añadir Sesión" (Abre modal vacío)
        if (e.target.classList.contains("btn-add-session")) {
            const movieId = e.target.getAttribute("data-movie-id");
            openSessionModal(movieId, bsModal);
        }

        // Botón "Editar Sesión" (Carga datos en modal)
        if (e.target.closest(".btn-edit-session")) {
            const btn = e.target.closest(".btn-edit-session");
            const data = {
                id: btn.dataset.sessionId,
                start: btn.dataset.startTime,
                room: btn.dataset.roomId,
                price: btn.dataset.price,
                movie: btn.dataset.movieId
            };
            openEditSession(data, bsModal);
        }

        // Botón "Cancelar Sesión"
        if (e.target.classList.contains("btn-cancel-session")) {
            const sid = e.target.dataset.sessionId;
            const mid = e.target.dataset.movieId;
            cancelSession(sid, mid);
        }

        // Botón "Eliminar Película"
        //if (e.target.classList.contains("btn-delete-movie")) {
        //    deleteMovieFromSessions();
        //}
        if (e.target.classList.contains("btn-delete-movie")) {
            // Capturamos el ID directamente del botón que pulsamos
            const movieId = e.target.getAttribute("data-movie-id");

            // Si por algún motivo el partial no tiene el ID, usamos la global como plan B
            const idFinal = movieId || currentMovieId;

            if (idFinal) {
                deleteMovieFromSessions(idFinal);
            } else {
                alert("Error: No se pudo identificar la película a eliminar.");
            }
        }
    });
});

// --- 3. FUNCIONES DE LÓGICA (Convertidas a Lambdas para seguir el estilo) ---

//const loadSessions = (movieId) => {
//    currentMovieId = movieId;
//    const container = document.getElementById(`sessions-${movieId}`);

//    fetch(`/Cartelera/GetSessions?movieId=${movieId}`)
//        .then(r => r.text())
//        .then(html => container.innerHTML = html)
//        .catch(err => console.error("Error:", err));
//};
const loadSessions = (movieId) => {
    if (!movieId) return;

    currentMovieId = movieId;
    const container = document.getElementById(`sessions-${movieId}`);

    // PASO CLAVE: Limpiar el contenedor antes de la nueva carga para evitar duplicados
    container.innerHTML = '<div class="spinner-border spinner-border-sm text-primary"></div>';

    fetch(`/Cartelera/GetSessions?movieId=${movieId}&t=${new Date().getTime()}`) // 't' evita caché del navegador
        .then(r => r.text())
        .then(html => {
            container.innerHTML = html;
        })
        .catch(err => {
            container.innerHTML = '<span class="text-danger">Error al cargar.</span>';
            console.error("Error:", err);
        });
};

const openSessionModal = (movieId, modalInstance) => {
    // Limpiamos campos
    document.getElementById("session-error").classList.add("d-none");
    document.getElementById("sessionId").value = "";
    document.getElementById("movieId").value = movieId;
    document.getElementById("startTime").value = "";
    document.getElementById("roomId").value = "";
    document.getElementById("price").value = "";

    modalInstance.show();
};

const openEditSession = (data, modalInstance) => {
    document.getElementById("session-error").classList.add("d-none");
    document.getElementById("sessionId").value = data.id;
    document.getElementById("movieId").value = data.movie;
    document.getElementById("startTime").value = data.start;
    document.getElementById("roomId").value = data.room;
    document.getElementById("price").value = data.price;

    modalInstance.show();
};

const saveSession = (modalInstance) => {
    const errorDiv = document.getElementById("session-error");

    const data = {
        SessionId: parseInt(document.getElementById("sessionId").value) || 0,
        MovieId: parseInt(document.getElementById("movieId").value),
        StartTime: document.getElementById("startTime").value,
        RoomId: parseInt(document.getElementById("roomId").value),
        Price: document.getElementById("price").value.replace(',', '.')
    };

    const url = data.SessionId === 0 ? "/Cartelera/CreateSession" : "/Cartelera/UpdateSession";

    fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    })
        .then(async r => {
            if (!r.ok) throw new Error(await r.text());
            loadSessions(data.MovieId);
            modalInstance.hide();
        })
        .catch(err => {
            errorDiv.textContent = err.message;
            errorDiv.classList.remove("d-none");
        });
};

const cancelSession = (sessionId, movieId) => {
    if (!confirm("¿Cancelar sesión?")) return;

    fetch("/Cartelera/CancelSession", {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: `sessionId=${sessionId}`
    })
        .then(() => loadSessions(movieId));
};

//const deleteMovieFromSessions = () => {
//    if (!confirm("¿Eliminar película?")) return;

//    fetch("/Cartelera/DeleteMovie", {
//        method: "POST",
//        headers: { "Content-Type": "application/x-www-form-urlencoded" },
//        body: `movieId=${currentMovieId}`
//    })
//        .then(r => {
//            if (!r.ok) throw new Error();
//            location.reload();
//        })
//        .catch(() => alert("No se puede borrar la película"));
//};
const deleteMovieFromSessions = (movieId) => {
    if (!confirm("¿Eliminar película?")) return;

    fetch("/Cartelera/DeleteMovie", {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        // Enviamos el ID que recibimos por parámetro
        body: `movieId=${movieId}`
    })
        .then(r => {
            if (!r.ok) {
                // Si el controlador devuelve BadRequest, lanzamos el error para el catch
                return r.text().then(text => { throw new Error(text) });
            }
            location.reload();
        })
        .catch(err => alert(err.message || "No se puede borrar la película"));
};
