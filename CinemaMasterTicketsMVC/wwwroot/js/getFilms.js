//const API_KEY = "e30c1ae43fe34f0a90b23ecb086b8571"; //Esto habrá que meterlo en variables de entorno
//const container = document.getElementById("moviesContainer"); //Recogida del select y el container para meter las pelis
//const genreSelector = document.getElementById("genreSelector");

//let allMovies = []; // Variable global para guardar todas las pelis cargadas

//// Función para cargar todas las películas populares por defecto (recorriendo todas las páginas)
//async function loadMovies() {
//    container.innerHTML = "";
//    allMovies = [];
//    let page = 1; //Empezamos por la pagina 1
//    let totalPages = 1; //Total de paginas 1

//    //Mediante yn do-while vamos imprimiendo y metiendo todas las eplis en el array con todas sus propiedades
//    //y aumentando su numero de pagina hasta que lleguemos al final de TotalPages.
//    try {
//        do {
//            const res = await fetch(`https://api.themoviedb.org/3/movie/popular?api_key=${API_KEY}&language=es-ES&page=${page}`);
//            const data = await res.json();
//            allMovies.push(...data.results);

//            totalPages = data.total_pages;
//            page++;
//        } while (page <= totalPages);
//        // Una vez finalizado el ciclo, cargamos las pelis
//        renderMovies(allMovies);
//    } catch (error) {
//        console.error("Error cargando películas", error);
//    }
//}

//// Función para cargar las pelis con foto en un contenedor
//function renderMovies(movies) {
//    //Primero borramos en contenedor
//    container.innerHTML = ""; 
//    //Por cada peli, se crea unCard con las caracteristicas propias para rellenarlo
//    movies.forEach(movie => { 
//        const card = createMovieCard(movie);
//        //y se le añaden al container de la vista
//        container.appendChild(card);
//    });
//}

////Función para crear una Carde de una pelicula
//function createMovieCard(movie) {
//    //Se crea un div con una clase especifica para darle estilos en el css
//    const card = document.createElement("div");
//    card.className = "movie-card";

//    //Creamos una etiqueta de imagen y le insertamos el cartel de la peli y su nombre
//    const img = document.createElement("img");
//    img.src = `https://image.tmdb.org/t/p/w500${movie.poster_path}`;
//    img.alt = movie.title;

//    //Creamos otro div para el cuerpo de la peli con su class para el css
//    const body = document.createElement("div");
//    body.className = "movie-body";

//    //Otro para el titulo de la peli con el mismo procedimiento
//    const title = document.createElement("div");
//    title.className = "movie-title";
//    title.textContent = movie.title;

//    //El último será el botón de añadir peli
//    const addButton = document.createElement("button");
//    addButton.className = "btn btn-primary";
//    addButton.textContent = "Añadir";

//    //Evento de escucha de los botones para añadir las pelis
//    //Primero se crea el objeto "movie" con los atributos que necesita para que encaje con el modelo
//    addButton.addEventListener("click", async () => {
//        const movieData = {
//            MovieAPIId: movie.id.toString(),
//            Title: movie.title,
//            OverView: movie.overview,
//            PosterUrl: `https://image.tmdb.org/t/p/w500${movie.poster_path}`,
//            DurationMinutes: movie.runtime || 0,
//            Genre: movie.genre_ids ? movie.genre_ids.join(", ") : "" //Puede tener mas de un genero
//        };

//        //Se realiza el fetch al "action" del controlador mediante POST pasandole en el cuerpo el objeto
//        //tipo "movie" para que pueda guardarlo
//        try {
//            const res = await fetch("/Admin/AddMovie", {
//                method: "POST",
//                headers: { "Content-Type": "application/json" },
//                body: JSON.stringify(movieData)
//            });
//            //Esperamos la respuesta del servidor, si no es OK mandamos el mensaje correspondiente y 
//            //cortamos la ejecucion de guardado para que no cambie el aspecto del botón.
//            const data = await res.json();
//            if (!res.ok) {
                
//                console.error("Error al añadir la película:", data.message);
//                alert("No se pudo añadir la película. Inténtalo de nuevo.");
//                return; 
//            }
//            //Si todo va bien cambiamos el boton
//            addButton.textContent = "Añadida";
//            addButton.disabled = true;
//        } catch (error) {
//            console.error("Error al añadir la película:", error);
//        }
//    });

//    //Añadimos el boton y el titulo al cuerpo de la peli
//    body.appendChild(title);
//    body.appendChild(addButton);

//    //Añadimos la imagen y el cuerpo al contenedor de la peli
//    card.appendChild(img);
//    card.appendChild(body);

//    //Retornamos la card para que la pueda implementar mas facilmente en otro elemento
//    return card;
//}

//// Filtrar películas por género cuando cambia el selector
//genreSelector.addEventListener("change", () => {

//    //Recogemos el valor seleccionado parseando a tipo Number
//    const selectedGenre = Number(genreSelector.value);

//    //Si no hay selección, por defecto mostrará todas (las mas populares)
//    if (!selectedGenre) {
//        renderMovies(allMovies); 
//        return;
//    }
//    //Si no, filtramos por el id de la pelicula cuyo genero contenga el que se ha seleccionado
//    const filtered = allMovies.filter(movie => movie.genre_ids.includes(selectedGenre));
//    renderMovies(filtered); //Y se llama al metodo que las construye con las pelis de esa query
//});

//document.addEventListener("DOMContentLoaded", loadMovies);