
document.addEventListener("DOMContentLoaded", () => {
    // Creacion de elementos
    const pricePerSeat = parseFloat(document.getElementById("pricePerSeatData")?.value || 0);
    const TotalSeatsPrice = document.getElementById("TotalSeatsPrice");
    const totalPriceDiv = document.getElementById("totalPriceDiv");
    const selectedSeats = document.getElementById("SelectedSeats");
    const selectedSeatNames = document.getElementById("SelectedSeatNames");
    const selectedSeatsDiv = document.getElementById("selectedSeatsDiv");
    const selectedSeatIdsDiv = document.getElementById("selectedSeatIdsDiv");
    const seats = document.querySelectorAll(".seat.available");
    const divSelectors = document.getElementById("userTypeContainer")

    // Arrays para manejar IDs y nombres
    const selectedSeatIds = [];
    const selectedSeatLabels = [];

    /* Objeto para guardar tipo de usuario por asiento */
    const selectedSeatTypes = {};

    /* ---------- Descuentos según tipo de usuario ---------- */
    // Aqui definimos los descuentos que se van a aplicar directamente sobre el precio original segun
    //el tipo de asiento que seleccione el usuario, sin necesidad de consultar a la base de datos ni
    // recargar la pagina para calcularlos.
    const discounts = {
        "Niño": 0.6,
        "Adulto": 1,
        "Tercera Edad": 0.7,
        "Minusvalia": 0.5
    };

    /* Contenedor para los selects de tipo de usuario */
    let userTypeContainer = document.getElementById("userTypeSelectors");
    if (!userTypeContainer) {
        userTypeContainer = document.createElement("div");
        userTypeContainer.id = "userTypeSelectors";
        userTypeContainer.className = "mb-3";
    divSelectors.appendChild(userTypeContainer);
    }

    // Inicializar arrays con asientos preseleccionados
    document.querySelectorAll('.seat.selected').forEach(seat => {
        selectedSeatIds.push(seat.dataset.seatId);
        selectedSeatLabels.push(seat.dataset.seatLabel);

    //Inicializamos el tipo de asiento directamente como adulto por defecto
        selectedSeatTypes[seat.dataset.seatId] = "Adulto";
    });

    /* Función para actualizar los selects de tipo de usuario */
    //Esta función se encarga de crear los menús desplegables (dropdowns) que aparecen cuando
    //eliges un asiento.
    function updateUserTypeDisplay() {
        const userTypeLabel = document.getElementById("userTypeLabel");
        //Si hay algun asiento seleccionado, mostramos el titulo de la seccion de seleccion de asientos
        if (selectedSeatIds.length > 0) {
            userTypeLabel.style.display = "inline";
        } else {
            userTypeLabel.style.display = "none";
        }
        //Borramos el contenedor de seleccion de usuario para dejarlo limpio
        userTypeContainer.innerHTML = "";
        userTypeContainer.className = "mb-3";
        //Por cada ID de asiento seleccionado
        selectedSeatIds.forEach(id => {
            //Buscamos cada nombre del asiento guardado en el dataset
            const label = document.querySelector(`.seat[data-seat-id='${id}']`).dataset.seatLabel;
            //Ponemos el tipoi de asiento si ya tiene uno seleccionado, si no le ponemos por defecto el de adulto.
            const tipo = selectedSeatTypes[id] || "Adulto";
            //Creamos un selector propio para el asiento, donde recogemos su id para poder identificarlo
            //cuando se le seleccione un precio
            const select = document.createElement("select");
            select.dataset.seatId = id;
            select.className = "form-select form-select-sm mb-2 bg-dark text-white border-0";
            //Creamos un array con los tipos de usuario e implementamos una logica para que, cuando se 
            //seleccione uno de ellos, el spinner se quede seleccionado con ese valor y guarde su tipo
            const userTypesArray = ["Niño", "Adulto", "Tercera Edad", "Minusvalia"]
            userTypesArray.forEach(t => {
                const option = document.createElement("option");
                option.value = t;
                option.textContent = `${t} - ${(pricePerSeat * discounts[t]).toFixed(2).replace('.', ',')} €`;
                if (t === tipo) option.selected = true;
                select.appendChild(option);
            });

            /* ---------- Cambiar tipo de usuario al seleccionar ---------- */
            //Cuando se cambia el selector, es necesario volver a leer el tipo de usuario que se ha seleccionado
            //y recalcular el descuento o el precio del asiento seleccionado para que lo plasme en el total mediante
            //la funcion "updateSelectedSeatsAndTotal();"
            select.addEventListener("change", (e) => {
                selectedSeatTypes[id] = e.target.value;
                updateSelectedSeatsAndTotal();
            });

            //generamos un div para meterle los options generados con el nombre y el tipo y los metemos en el select,
            //añadiendolo al div donde se muestran todos los select
            const div = document.createElement("div");
            div.className = "seat-selector-row p-2";
            // Usamos innerHTML para que el texto sea un nodo hijo directo y reciba el color
            div.innerHTML = `Asiento ${label}: `;
            div.appendChild(select);
            userTypeContainer.appendChild(div);
        });
    }

    /* Función para actualizar total y inputs ocultos según tipo de usuario */
    //Esta funcion hace de Calculadora, sumando los precios al precio total y metiendolo en los inputs ocultos necesarios
    //para poder pasarselos al controlador.
    function updateSelectedSeatsAndTotal() {
        //Escribimos los nombres de los asientos en los div
        selectedSeatsDiv.textContent = selectedSeatLabels.join(", ");
        //Empezamos la cuenta en 0
        let total = 0;
        //Por cada id de asiento selecctionado, vamos calculando su precio * el descuento y lo vamos añadiendo al total
        selectedSeatIds.forEach(id => {
            total += pricePerSeat * discounts[selectedSeatTypes[id]];
        });
        //En esta variable guardamos el total para el servidor
        TotalSeatsPrice.value = total.toFixed(2).replace('.', ',');
        //En esta variable guardamos el precio calculado total para mostrarlo por pantalla
        totalPriceDiv.textContent = total.toFixed(2).replace('.', ',') + " €";
        //En esta variable guardamos los IDs de los asientos para enviarlos al servidor
        selectedSeats.value = selectedSeatIds.join(",");
        //En esta guardamos los nombres de los asientos (A1, B3...) para poder representarlos en el ticket y en pantalla
        selectedSeatNames.value = selectedSeatLabels.join(",");

        //Guarda los asientos con su tipo para poder envialo al servidor e implementarlo en las sessions y representarlo en los tickets
        document.getElementById("SeatUserTypes").value =
            JSON.stringify(selectedSeatTypes);
    }
    

    /* Inicializar visualización de selects y total */
    updateUserTypeDisplay();
    updateSelectedSeatsAndTotal();

    // En esta seccion manejamos los clicks sobre cada asiento
    seats.forEach(seat => {
        seat.addEventListener("click", () => {
            const seatId = seat.dataset.seatId; //Id del asiento
            const seatLabel = seat.dataset.seatLabel; //Nombre del asiento (A5)

            //Si el asiento ya estaba elegido...Si aparece en la lista de los asientos seleccionados
            if (selectedSeatIds.includes(seatId)) {
                //Lo quitamos de la lista con splice y de la lista de nombres tambien para que no se represente
                const index = selectedSeatIds.indexOf(seatId);
                selectedSeatIds.splice(index, 1);
                selectedSeatLabels.splice(index, 1);

                //Y tambien lo borramos del array de tipos de usuario
                delete selectedSeatTypes[seatId];

                //Tambien le quitamos el atributo de selected para que cambie su color
                seat.classList.remove("selected");
                //Si no, lo añadimos a cada array correspondiente
            } else {
                if (selectedSeatIds.length >= 15) {
                    alert("Lo sentimos, solo puedes seleccionar un máximo de 20 asientos por compra.");
                    return; // Detiene la ejecución y no añade el asiento
                }
                selectedSeatIds.push(seatId); //Añade a la lista de IDs
                selectedSeatLabels.push(seatLabel); //Añade a la lista de nombres
                selectedSeatTypes[seatId] = "Adulto";//Lo añade por defecto como Adulto
                seat.classList.add("selected"); //Y le añade el atributo selected para el color
            }

            //Llamamos a los metodos para actualizar los selectores, por si hay que quitar o poner alguno nuevo
            updateUserTypeDisplay();
            //Y actualizamos la suma total de precios si es necesario
            updateSelectedSeatsAndTotal();
        });
    });
});










