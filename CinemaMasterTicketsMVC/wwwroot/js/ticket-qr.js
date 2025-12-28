

document.addEventListener("DOMContentLoaded", () => {
    // 1. Buscamos el elemento que tiene el ID del ticket (lo guardaremos en un atributo data)
    const qrContainer = document.getElementById("qrcode");

    if (qrContainer) {
        // 2. Obtenemos el ID del ticket desde el atributo 'data-ticket-id'

        const ticketId = qrContainer.getAttribute("data-ticket-id");

        // Construimos la URL. En localhost será algo como: 
        // http://localhost:1234/Tickets/Validar/11
        //const urlValidacion = `${window.location.origin}/Tickets/Validar/${ticketId}`;

        // 3. Ejecutamos la lógica para crear el QR
        new QRCode(qrContainer, {
            //text: urlValidacion, // <--- AQUÍ entra la URL en lugar de solo el ID
            text: ticketId.toString(),
            width: 140,
            height: 140,
            colorDark: "#000000",
            colorLight: "#ffffff",
            correctLevel: QRCode.CorrectLevel.H
        });

        console.log(`QR generado exitosamente para el ticket: ${ticketId}`);
    }
});