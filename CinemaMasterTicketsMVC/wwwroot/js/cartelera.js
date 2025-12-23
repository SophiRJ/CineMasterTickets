
let currentMovieId = null;


function loadSessions(movieId) {
    if (!movieId) {
        console.error("Error: Se intentó cargar sesiones sin un MovieId válido.");
        return;
    }

    currentMovieId = movieId;
    const containerId = `sessions-${movieId}`;
    const container = document.getElementById(containerId);

    // Verificación de seguridad para evitar la excepción
    if (!container) {
        console.error(`Error: No se encontró el contenedor con ID "${containerId}" en el HTML.`);
        return;
    }

    fetch(`/Cartelera/GetSessions?movieId=${movieId}&t=${new Date().getTime()}`)
        .then(r => {
            if (!r.ok) throw new Error("Error en la respuesta del servidor");
            return r.text();
        })
        .then(html => {
            container.innerHTML = html;
        })
        .catch(err => console.error("Error al cargar sesiones:", err));
}

// Función para abrir el modal de creación de sesion
function openSessionModal(movieId) {
    //Limpiar el modal al abrirlo y
    // asignamos el ID de la película al input hidden dentro del modal
    document.getElementById("sessionId").value = "";
    document.getElementById("movieId").value = movieId;
    document.getElementById("startTime").value = "";
    document.getElementById("roomId").value = "";
    document.getElementById("price").value = "";
    // Inicializamos y mostramos el modal usando Bootstrap 5
    new bootstrap.Modal(document.getElementById("sessionModal")).show();
}

function saveSession() {
    const errorDiv = document.getElementById("session-error");
    errorDiv.classList.add("d-none");

    //Intentar obtener el MovieId del input
    let movieIdVal = document.getElementById("movieId").value;

    //Si el input está vacío, usar la variable global que se asignó al abrir
    if (!movieIdVal || movieIdVal == "0") {
        movieIdVal = currentMovieId;
    }

    const sessionId = document.getElementById("sessionId").value;

    const data = {
        SessionId: sessionId ? parseInt(sessionId) : 0,
        MovieId: parseInt(movieIdVal),
        StartTime: document.getElementById("startTime").value,
        RoomId: parseInt(document.getElementById("roomId").value),
        Price: document.getElementById("price").value.replace(',', '.') // Manejo de decimales
    };

    // VALIDACIÓN FINAL antes de enviar al servidor
    if (isNaN(data.MovieId) || data.MovieId <= 0) {
        showSessionError("Error interno: No se detectó el ID de la película. Cierre el modal e intente de nuevo.");
        console.error("Data incompleta:", data);
        return;
    }

    const url = data.SessionId === 0 ? "/Cartelera/CreateSession" : "/Cartelera/UpdateSession";

    fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    })
        .then(async response => {
            if (!response.ok) {
                const msg = await response.text();
                throw new Error(msg);
            }

            // RECARGA INSTANTÁNEA
            loadSessions(data.MovieId);

            // 2. NUEVO: Recarga la lista global del panel lateral si está abierto
            //const globalContent = document.getElementById('activeSessionsContent');
            //if (globalContent) {
            //    fetch('/Cartelera/ActiveSessions')
            //        .then(r => r.text())
            //        .then(html => globalContent.innerHTML = html);

            // CERRAR MODAL
            const modalElement = document.getElementById("sessionModal");
            const modalInstance = bootstrap.Modal.getInstance(modalElement);
            if (modalInstance) modalInstance.hide();
        })
        .catch(error => {
            showSessionError(error.message || "Error al procesar la solicitud");
        });
}
//Funcion para editar la sesion usamos el mismo modal
function openEditSession(sessionId, startTime, roomId, price, movieId) {
    document.getElementById("sessionId").value = sessionId;
    document.getElementById("movieId").value = movieId;
    document.getElementById("startTime").value = startTime;
    document.getElementById("roomId").value = roomId;
    document.getElementById("price").value = price;

    new bootstrap.Modal(document.getElementById("sessionModal")).show();
}


//Funcion para cancelar la sesion en caso de que no tenga tickets vendidos
function cancelSession(sessionId, movieId) {
    if (!confirm("¿Cancelar sesión?")) return;

    fetch("/Cartelera/CancelSession", {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: `sessionId=${sessionId}`
    })
        .then(() => loadSessions(movieId));
}

//Eliminar peliculas que no tengan sesiones
function deleteMovieFromSessions() {
    if (!confirm("¿Eliminar película?")) return;

    fetch("/Cartelera/DeleteMovie", {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: `movieId=${currentMovieId}`
    })
        .then(r => {
            if (!r.ok) throw new Error();
            location.reload();
        })
        .catch(() => alert("No se puede borrar la película"));
}


// mostrar el error en caso de que la sesion no se pueda crear
function showSessionError(message) {
    const div = document.getElementById("session-error");
    div.textContent = message;
    div.classList.remove("d-none");
}
//window.openActiveSessionsModal = function () {
//    // 1. Instanciar y mostrar el Offcanvas de Bootstrap
//    const element = document.getElementById('activeSessionsModal');
//    if (!element) {
//        console.error("No se encontró el elemento 'activeSessionsModal' en el DOM.");
//        return;
//    }
//    const offcanvas = bootstrap.Offcanvas.getOrCreateInstance(element);
//    offcanvas.show();

//    // 2. Cargar el contenido de la tabla
//    const contentDiv = document.getElementById('activeSessionsContent');

//    fetch('/Cartelera/ActiveSessions')
//        .then(response => {
//            if (!response.ok) throw new Error("Error en la carga");
//            return response.text();
//        })
//        .then(html => {
//            contentDiv.innerHTML = html;
//        })
//        .catch(err => {
//            contentDiv.innerHTML = '<div class="alert alert-danger">No se pudieron cargar las sesiones.</div>';
//            console.error(err);
//        });
//}

