
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

    /* ---------- NUEVO: Objeto para guardar tipo de usuario por asiento ---------- */
    const selectedSeatTypes = {};

    /* ---------- NUEVO: Descuentos según tipo de usuario ---------- */
    const discounts = {
        "Niño": 0.6,
        "Adulto": 1,
        "Tercera Edad": 0.7,
        "Minusvalia": 0.5
    };

    /* ---------- NUEVO: Contenedor para los selects de tipo de usuario ---------- */
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

        /* ---------- NUEVO: Inicializar tipo de usuario por defecto como Adulto ---------- */
        selectedSeatTypes[seat.dataset.seatId] = "Adulto";
    });

    /* ---------- NUEVO: Función para actualizar los selects de tipo de usuario ---------- */
    function updateUserTypeDisplay() {
        userTypeContainer.innerHTML = "";
        selectedSeatIds.forEach(id => {
            const label = document.querySelector(`.seat[data-seat-id='${id}']`).dataset.seatLabel;
            const tipo = selectedSeatTypes[id] || "Adulto";

            const select = document.createElement("select");
            select.dataset.seatId = id;
            select.className = "form-select mb-2";

            ["Niño", "Adulto", "Tercera Edad", "Minusvalia"].forEach(t => {
                const option = document.createElement("option");
                option.value = t;
                option.textContent = `${t} - ${(pricePerSeat * discounts[t]).toFixed(2).replace('.', ',')} €`;
                if (t === tipo) option.selected = true;
                select.appendChild(option);
            });

            /* ---------- NUEVO: Cambiar tipo de usuario al seleccionar ---------- */
            select.addEventListener("change", (e) => {
                selectedSeatTypes[id] = e.target.value;
                updateSelectedSeatsAndTotal();
            });

            const div = document.createElement("div");
            div.textContent = `Asiento ${label}: `;
            div.appendChild(select);
            userTypeContainer.appendChild(div);
        });
    }

    /* ---------- NUEVO: Función para actualizar total y inputs ocultos según tipo de usuario ---------- */

    function updateSelectedSeatsAndTotal() {

        selectedSeatsDiv.textContent = selectedSeatLabels.join(", ");
        selectedSeatIdsDiv.textContent = selectedSeatIds.join(",");

        let total = 0;
        selectedSeatIds.forEach(id => {
            total += pricePerSeat * discounts[selectedSeatTypes[id]];
        });

        TotalSeatsPrice.value = total.toFixed(2).replace('.', ',');
        totalPriceDiv.textContent = total.toFixed(2).replace('.', ',') + " €";

        selectedSeats.value = selectedSeatIds.join(",");
        selectedSeatNames.value = selectedSeatLabels.join(",");

        document.getElementById("SeatUserTypes").value =
            JSON.stringify(selectedSeatTypes);
    }
    

    /* ---------- NUEVO: Inicializar visualización de selects y total ---------- */
    updateUserTypeDisplay();
    updateSelectedSeatsAndTotal();

    // Manejar clics sobre asientos
    seats.forEach(seat => {
        seat.addEventListener("click", () => {
            const seatId = seat.dataset.seatId;
            const seatLabel = seat.dataset.seatLabel;

            if (selectedSeatIds.includes(seatId)) {
                const index = selectedSeatIds.indexOf(seatId);
                selectedSeatIds.splice(index, 1);
                selectedSeatLabels.splice(index, 1);

                /* ---------- NUEVO: eliminar asiento del objeto de tipos ---------- */
                delete selectedSeatTypes[seatId];

                seat.classList.remove("selected");
            } else {
                selectedSeatIds.push(seatId);
                selectedSeatLabels.push(seatLabel);

                /* ---------- NUEVO: añadir asiento con tipo por defecto ---------- */
                selectedSeatTypes[seatId] = "Adulto";

                seat.classList.add("selected");
            }

            /* ---------- NUEVO: actualizar selects y total después de click ---------- */
            updateUserTypeDisplay();
            updateSelectedSeatsAndTotal();
        });
    });
});


//

//function updateSelectedSeatsAndTotal() {
//    selectedSeatsDiv.textContent = selectedSeatLabels.join(", ");
//    selectedSeatIdsDiv.textContent = selectedSeatIds.join(",");

//    let total = 0;
//    selectedSeatIds.forEach(id => {
//        const tipo = selectedSeatTypes[id];
//        total += pricePerSeat * discounts[tipo];
//    });

//    if (TotalSeatsPrice) TotalSeatsPrice.value = total.toFixed(2);
//    if (totalPriceDiv) totalPriceDiv.textContent = total.toFixed(2).replace('.', ',') + " €";

//    selectedSeats.value = selectedSeatIds.join(",");
//    selectedSeatNames.value = selectedSeatLabels.join(",");

//    /* ---------- NUEVO: Guardar tipos en hidden input para enviar al servidor ---------- */
//    const seatUserTypesInput = document.getElementById("SeatUserTypes");
//    if (seatUserTypesInput) seatUserTypesInput.value = JSON.stringify(selectedSeatTypes);
//}


//document.addEventListener("DOMContentLoaded", () => {
//    // Referencias a elementos
//    const pricePerSeat = parseFloat(document.getElementById("pricePerSeatData")?.value || 0);
//    const totalPriceInput = document.getElementById("TotalSeatsPrice");
//    const totalPriceDisplay = document.getElementById("totalPriceDisplay");
//    const selectedSeatsInput = document.getElementById("SelectedSeats");
//    const selectedSeatNamesInput = document.getElementById("SelectedSeatNames");
//    const selectedSeatsDisplay = document.getElementById("selectedSeatsDisplay");
//    const selectedSeatIdsDisplay = document.getElementById("selectedSeatIdsDisplay");
//    const seats = document.querySelectorAll(".seat.available");

//    // Arrays para manejar IDs y nombres
//    const selectedSeatIds = [];
//    const selectedSeatLabels = [];

//    // Inicializar arrays con asientos preseleccionados
//    document.querySelectorAll('.seat.selected').forEach(seat => {
//        selectedSeatIds.push(seat.dataset.seatId);
//        selectedSeatLabels.push(seat.dataset.seatLabel);
//    });

//    // Inicializar inputs y visualización
//    const totalInicial = selectedSeatIds.length * pricePerSeat;
//    if (totalPriceInput) totalPriceInput.value = totalInicial.toFixed(2);
//    if (totalPriceDisplay) totalPriceDisplay.textContent = totalInicial.toFixed(2).replace('.', ',') + " €";

//    selectedSeatsInput.value = selectedSeatIds.join(",");
//    selectedSeatNamesInput.value = selectedSeatLabels.join(",");
//    selectedSeatsDisplay.textContent = selectedSeatLabels.join(", ");
//    selectedSeatIdsDisplay.textContent = selectedSeatIds.join(",");

//    // Manejar clics sobre asientos
//    seats.forEach(seat => {
//        seat.addEventListener("click", () => {
//            const seatId = seat.dataset.seatId;
//            const seatLabel = seat.dataset.seatLabel;

//            if (selectedSeatIds.includes(seatId)) {
//                const index = selectedSeatIds.indexOf(seatId);
//                selectedSeatIds.splice(index, 1);
//                selectedSeatLabels.splice(index, 1);
//                seat.classList.remove("selected");
//            } else {
//                selectedSeatIds.push(seatId);
//                selectedSeatLabels.push(seatLabel);
//                seat.classList.add("selected");
//            }

//            // Actualizar total
//            const total = selectedSeatIds.length * pricePerSeat;
//            if (totalPriceInput) totalPriceInput.value = total.toFixed(2);
//            if (totalPriceDisplay) totalPriceDisplay.textContent = total.toFixed(2).replace('.', ',') + " €";

//            // Actualizar inputs ocultos y divs de visualización
//            selectedSeatsInput.value = selectedSeatIds.join(",");
//            selectedSeatNamesInput.value = selectedSeatLabels.join(",");
//            selectedSeatsDisplay.textContent = selectedSeatLabels.join(", ");
//            selectedSeatIdsDisplay.textContent = selectedSeatIds.join(",");
//        });
//    });
//});
