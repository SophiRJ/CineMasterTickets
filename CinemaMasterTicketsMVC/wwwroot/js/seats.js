document.addEventListener("DOMContentLoaded", () => {
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

            selectedSeatsInput.value = selectedSeatIds.join(",");
            selectedSeatsDisplay.textContent = selectedSeatLabels.join(", ");
            selectedSeatIdsDisplay.textContent = selectedSeatIds.join(", "); 
        });
    });
});