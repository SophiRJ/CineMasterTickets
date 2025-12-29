// Variable global para recordar con que pelicula estamos trabajando, la usamos para refrescar listas.
let currentMovieId = null;

document.addEventListener("DOMContentLoaded", () => {

    //Capturamos el boton de guardar y el elemento html del modal 
    const btnSaveSession = document.getElementById("btnSaveSession");
    const sessionModalElement = document.getElementById("sessionModal");

    // Inicializamos el modal de bootstrap una sola vez
    const bsModal = new bootstrap.Modal(sessionModalElement);

    // Asignacion de eventos

    // Evento para el botón guardar el modal, ejecuta la funcion 'saveSession' al pulsarlo
    //la funcion cerrara el modal al final si todo a ido bien
    btnSaveSession.addEventListener("click", () => saveSession(bsModal));

    // aqui se configura un escucha general de clics para gestionar elementos 
    //que se crean dinamicamente como los botones de eliminar pelicula si no hay sesiones o editar sesion 
    document.addEventListener("click", (e) => {

        // Identifica si el elemento pulsado tiene la clase para cargar sesiones
        if (e.target.classList.contains("btn-load-sessions")) {
            // Obtiene el identificador de la película desde el atributo de datos del boton
            const movieId = e.target.getAttribute("data-movie-id");
            // Ejecuta la funcion para cargar sesiones establecida mas abajo
            loadSessions(movieId);
        }

        // Identifica si el elemento pulsado tiene la clase para añadir una nueva sesion.
        if (e.target.classList.contains("btn-add-session")) {
            const movieId = e.target.getAttribute("data-movie-id");
            // Ejecuta la funcion para preparar y abrir el modal definida mas abajo.
            openSessionModal(movieId, bsModal);
        }

        // Boton editar Sesion -> carga los datos en la modal 
        if (e.target.closest(".btn-edit-session")) {
            const btn = e.target.closest(".btn-edit-session");
            // Recopila los datos almacenados en los atributos del boton en un objeto.
            const data = {
                id: btn.dataset.sessionId,
                start: btn.dataset.startTime,
                room: btn.dataset.roomId,
                price: btn.dataset.price,
                movie: btn.dataset.movieId
            };
            // Ejecuta la función para cargar esos datos en el modal
            openEditSession(data, bsModal);
        }

        // Boton cancelar sesion-> Identifica si el elemento pulsado tiene la clase para cancelar una sesion
        if (e.target.classList.contains("btn-cancel-session")) {
            const sid = e.target.dataset.sessionId;
            const mid = e.target.dataset.movieId;
            cancelSession(sid, mid);
        }

        // Botón eliminar pelicula
        
        if (e.target.classList.contains("btn-delete-movie")) {
            // Captura el Id directamente del boton que pulsamos
            const movieId = e.target.getAttribute("data-movie-id");

            // si no se carga el id desde la vista parcial se usa el id global 
            const idFinal = movieId || currentMovieId;

            //se ejecuta el metodo si hay id si no se lanza un alert 
            if (idFinal) {
                deleteMovieFromSessions(idFinal);
            } else {
                alert("Error: No se pudo identificar la película a eliminar.");
            }
        }
    });
});

//===============================FUNCIONES=======================

//Funcion para cargar sesiones de una pelicula especifica
const loadSessions = (movieId) => {
    //Se detiene la ejecucion si no llega un id valido
    if (!movieId) return;

    //Se actualiza la variable de pelicula actual y se define el contenedor donde se mostraran las sesiones
    currentMovieId = movieId;
    const container = document.getElementById(`sessions-${movieId}`);

    //Aqui se limpia el contenedor antes de la nueva carga
    container.innerHTML = '<div class="spinner-border spinner-border-sm text-primary"></div>';


    // hacemos la peticion al servidor,usamos 't' para evitar cache del navegador
    fetch(`/Cartelera/GetSessions?movieId=${movieId}&t=${new Date().getTime()}`) 
        .then(r => r.text())
        .then(html => {
            container.innerHTML = html;// reemplazamos el contenido del contenedor con el texto recibido
        })
        .catch(err => {
            // mostramos un mensaje de error en el contenedor si la peticion falla.
            container.innerHTML = '<span class="text-danger">Error al cargar.</span>';
            console.error("Error:", err);
        });
};
//funcion para abrir el modal cuando vamos a crear algo nuevo
const openSessionModal = (movieId, modalInstance) => {
    // Primero escondemos el cartel de error por si se quedo prendido antes
    document.getElementById("session-error").classList.add("d-none");

    //Dejamos todos los campos del formulario vacíos para escribir desde cero
    document.getElementById("sessionId").value = "";
    document.getElementById("movieId").value = movieId;//Excepto el id de la pelicula
    document.getElementById("startTime").value = "";
    document.getElementById("roomId").value = "";
    document.getElementById("price").value = "";

    // Finalmente mostramos el modal en la pantalla
    modalInstance.show();
};

//Funcion para abrir el modal pero en modo edicion
const openEditSession = (data, modalInstance) => {
    // Tambien escondemos el error por las dudas
    document.getElementById("session-error").classList.add("d-none");

    // Aqui no vaciamos nada, sino que llenamos los campos con la info que ya tenemos
    document.getElementById("sessionId").value = data.id;
    document.getElementById("movieId").value = data.movie;
    document.getElementById("startTime").value = data.start;
    document.getElementById("roomId").value = data.room;
    document.getElementById("price").value = data.price;

    //// Mostramos el modal con la informacion cargada
    modalInstance.show();
};

//Funcion para guardar cambios o la nueva sesion creada
const saveSession = (modalInstance) => {
    //Capturamos el formulario y el contenedor de errores
    const form = $("#sessionForm");
    const errorDiv = document.getElementById("session-error");
    errorDiv.classList.add("d-none"); // Limpiar errores previos

    // validacion del servidor
    if (!form.valid()) {
        return;
    }

    //Se guarda los campos escritos despues de la validacion
    const sessionId = document.getElementById("sessionId").value;
    const movieId = document.getElementById("movieId").value;
    const startTime = document.getElementById("startTime").value;
    const roomId = document.getElementById("roomId").value;
    const price = document.getElementById("price").value;

    //creamos un objeto con toda la info de los campos
    const data = {
        SessionId: parseInt(sessionId) || 0,
        MovieId: parseInt(movieId),
        StartTime: startTime,
        RoomId: parseInt(roomId),
        Price: price.replace(',', '.')// Si alguien puso coma, la cambiamos por punto
    };

    //va al metodo crear o update depende de si llega un id o no 
    const url = data.SessionId === 0 ? "/Cartelera/CreateSession" : "/Cartelera/UpdateSession";

    //Enviamos la peticion al servidor
    fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    })
        .then(async r => {
            //si hay algun error
            if (!r.ok) throw new Error(await r.text());
            // Si todo salio bien, refrescamos los datos de la sesion y cerramos el modal
            loadSessions(data.MovieId);
            modalInstance.hide();
        })
        .catch(err => {
            errorDiv.textContent = err.message;
            errorDiv.classList.remove("d-none");
        });
};
//Funcion para cancelar la sesion
const cancelSession = (sessionId, movieId) => {
    if (!confirm("¿Cancelar sesión?")) return;

    //Enviar la peticion al servidor con el id de la sesion
    fetch("/Cartelera/CancelSession", {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: `sessionId=${sessionId}`
    })
        .then(() => loadSessions(movieId));// Al terminar, refrescamos la lista de sesiones
};

//funcion para borrar una pelicula
const deleteMovieFromSessions = (movieId) => {
    if (!confirm("¿Eliminar película?")) return;

    //Enviamos la peticion al servidor con el id de la pelicula
    fetch("/Cartelera/DeleteMovie", {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: `movieId=${movieId}`
    })
        .then(r => {
            if (!r.ok) {
                // Si el controlador devuelve BadRequest, lanzamos el error para el catch
                return r.text().then(text => { throw new Error(text) });
            }
            // Si se borró bien, recargamos toda la página para que ya no se vea la película
            location.reload();
        })
        .catch(err => alert(err.message || "No se puede borrar la película"));
};
