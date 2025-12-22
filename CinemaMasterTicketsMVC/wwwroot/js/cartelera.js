
let currentMovieId = null;

//function loadSessions(movieId) {
//    currentMovieId = movieId;
//    // Hacemos una petición HTTP GET al endpoint GetSessions del controlador Cartelera
//    // Pasamos el ID de la película como query string
//    fetch(`/Cartelera/GetSessions?movieId=${movieId}`)
//        .then(r => r.text()) // Convertimos la respuesta en texto (HTML de la vista parcial)
//        .then(html => {
//            // Insertamos el HTML recibido dentro del contenedor correspondiente
//            // El contenedor tiene un id único por película: "sessions-{movieId}"
//            document.getElementById(`sessions-${movieId}`).innerHTML = html;
//        });
//}
// Función para cargar las sesiones de una película específica -> con control de errores 
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

// Función para guardar una nueva sesión
//function saveSession() {
//    //Limpiar errores
//    const errorDiv = document.getElementById("session-error");
//    errorDiv.classList.add("d-none");
//    errorDiv.textContent = "";

//    const sessionId = document.getElementById("sessionId").value;
//    // Creamos un objeto -> data con la información de la sesión tomada de los inputs del modal
//    //const data = {
//    //    sessionId: sessionId ? sessionId : 0,
//    //    movieId: document.getElementById("movieId").value,
//    //    startTime: document.getElementById("startTime").value,
//    //    roomId: document.getElementById("roomId").value,
//    //    price: document.getElementById("price").value
//    //};
//    const data = {
//        SessionId: sessionId ? sessionId : 0,
//        //MovieId: document.getElementById("movieId").value,
//        MovieId: parseInt(document.getElementById("movieId").value),

//        StartTime: document.getElementById("startTime").value,
//        RoomId: document.getElementById("roomId").value,
//        Price: document.getElementById("price").value
//    };
//    const url = data.SessionId == 0
//        ? "/Cartelera/CreateSession"
//        : "/Cartelera/UpdateSession";

//    // Hacemos una petición HTTP POST al endpoint CreateSession del controlador Cartelera
//    fetch(url, {
//        method: "POST",                            // Método POST para enviar datos
//        headers: { "Content-Type": "application/json" }, // Indicamos que enviamos JSON
//        body: JSON.stringify(data)                 // Convertimos el objeto 'data' a JSON
//    }).then(async response => {
//        if (!response.ok) {
//            const errorMessage = await response.text();
//            showSessionError(errorMessage);  //llamar a la funcion para mostrar el error
//            return;
//        }
//        if (response.ok) {
//            // Es vital usar el ID que tenemos en el objeto data
//            const mid = document.getElementById("movieId").value;
//            loadSessions(mid);

//            // Cerrar el modal correctamente
//            const modalElement = document.getElementById("sessionModal");
//            const modalInstance = bootstrap.Modal.getInstance(modalElement);
//            if (modalInstance) {
//                modalInstance.hide();
//            }
//        }
//        //loadSessions(data.MovieId);

//        //bootstrap.Modal.getInstance(
//        //    document.getElementById("sessionModal")
//        //).hide();
//    })
//        .catch(() => {
//            showSessionError("Error al crear la sesión");
//        });
//}


//function saveSession() {
//    const errorDiv = document.getElementById("session-error");
//    errorDiv.classList.add("d-none");

//    // CAPTURA IMPORTANTE: Guardamos el ID en una constante local
//    // antes de que cualquier otra cosa ocurra.
//    const movieIdInput = document.getElementById("movieId").value;
//    const movieIdParaRecargar = parseInt(movieIdInput);

//    const sessionId = document.getElementById("sessionId").value;

//    const data = {
//        SessionId: sessionId ? parseInt(sessionId) : 0,
//        MovieId: movieIdParaRecargar,
//        StartTime: document.getElementById("startTime").value,
//        RoomId: parseInt(document.getElementById("roomId").value),
//        Price: parseFloat(document.getElementById("price").value)
//    };

//    // Validación simple antes de enviar
//    if (isNaN(movieIdParaRecargar) || movieIdParaRecargar === 0) {
//        console.error("Error: El MovieId no es válido antes de enviar.");
//        return;
//    }

//    const url = data.SessionId === 0 ? "/Cartelera/CreateSession" : "/Cartelera/UpdateSession";

//    fetch(url, {
//        method: "POST",
//        headers: { "Content-Type": "application/json" },
//        body: JSON.stringify(data)
//    })
//        .then(async response => {
//            if (!response.ok) {
//                const errorMessage = await response.text();
//                showSessionError(errorMessage);
//                return;
//            }

//            // ÉXITO: Usamos nuestra constante segura
//            console.log("Recargando sesiones para película:", movieIdParaRecargar);
//            loadSessions(movieIdParaRecargar);

//            // Cerrar modal
//            const modalElement = document.getElementById("sessionModal");
//            const modalInstance = bootstrap.Modal.getInstance(modalElement);
//            if (modalInstance) modalInstance.hide();
//        })
//        .catch(err => {
//            showSessionError("Error de conexión al servidor");
//            console.error(err);
//        });
//}

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

