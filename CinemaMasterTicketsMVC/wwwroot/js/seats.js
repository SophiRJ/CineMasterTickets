document.addEventListener("DOMContentLoaded", () => {
    // 1. Declaramos las referencias a los nuevos elementos
    const pricePerSeat = parseFloat(document.getElementById("pricePerSeatData")?.value|| 0);
    const totalPriceInput = document.getElementById("TotalSeatsPrice");
    const totalPriceDisplay = document.getElementById("totalPriceDisplay");

    const seats = document.querySelectorAll(".seat.available");
    const selectedSeatsInput = document.getElementById("SelectedSeats");
    const selectedSeatsDisplay = document.getElementById("selectedSeatsDisplay");
    const selectedSeatIdsDisplay = document.getElementById("selectedSeatIdsDisplay");

    const selectedSeatIds = [];
    const selectedSeatLabels = [];

    seats.forEach(seat => {
        seat.addEventListener("click", () => {
            const seatId = seat.dataset.seatId;
            const seatLabel = seat.dataset.seatLabel;

            if (selectedSeatIds.includes(seatId)) {
                const index = selectedSeatIds.indexOf(seatId);
                selectedSeatIds.splice(index, 1);
                selectedSeatLabels.splice(index, 1);
                seat.classList.remove("selected");
            } else {
                selectedSeatIds.push(seatId);
                selectedSeatLabels.push(seatLabel);
                seat.classList.add("selected");
            }

            // 2. Calculamos el total basado en la cantidad de IDs en el array
            const total = selectedSeatIds.length * pricePerSeat; //Aqui llega 9.5 (sin coma)
            
            // 3. Actualizamos los inputs ocultos que irán al controlador
            selectedSeatsInput.value = selectedSeatIds.join(",");
            if (totalPriceInput) {
                totalPriceInput.value = total.toFixed(2);
                console.log("[DEBUG JS] TotalSeatsPrice =", totalPriceInput.value);
            }

            // 4. Actualizamos la parte visual para el cliente
            selectedSeatsDisplay.textContent = selectedSeatLabels.join(", ");
            selectedSeatIdsDisplay.textContent = selectedSeatIds.join(", ");

            if (totalPriceDisplay) {
                totalPriceDisplay.textContent = total.toFixed(2).replace('.', ',') + " €";
            }
        });
    });
});