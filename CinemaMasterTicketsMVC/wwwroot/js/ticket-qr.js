//document.addEventListener("DOMContentLoaded", () => {
//    // 1. Buscamos el elemento que tiene el ID del ticket (lo guardaremos en un atributo data)
//    const qrContainer = document.getElementById("qrcode");

//    if (qrContainer) {
//        // 2. Obtenemos el ID del ticket desde el atributo 'data-ticket-id'

//        const ticketId = qrContainer.getAttribute("data-ticket-id");

//        // Construimos la URL. En localhost será algo como:
//        // http://localhost:1234/Tickets/Validar/11
//        //const urlValidacion = ${window.location.origin}/Tickets/Validar/${ticketId};

//        // 3. Ejecutamos la lógica para crear el QR
//        new QRCode(qrContainer, {
//            //text: urlValidacion, // <--- AQUÍ entra la URL en lugar de solo el ID
//            text: ticketId.toString(),
//            width: 140,
//            height: 140,
//            colorDark: "#000000",
//            colorLight: "#ffffff",
//            correctLevel: QRCode.CorrectLevel.H
//        });

//        console.log(`QR generado exitosamente para el ticket: ${ticketId}`);
//    }
//});

document.addEventListener("DOMContentLoaded", () => {
    // 1. Creamos un elemento canvas dentro de tu div 'qrcode'
    const qrContainer = document.getElementById("qrcode");
    if (!qrContainer) return;

    // Limpiamos el contenedor y añadimos un canvas
    qrContainer.innerHTML = '<canvas id="qrCanvas"></canvas>';

    const ticketId = qrContainer.getAttribute("data-ticket-id");
    const movieTitle = document.querySelector(".ticket-visual h3")?.innerText || "";
    const sessionInfo = document.querySelector(".ticket-visual h3 + p")?.innerText || "";
    const seats = document.querySelector(".ticket-visual .fw-bold")?.innerText || "";
    const total = document.querySelector(".total-row .fw-bold")?.innerText || "";

    // Texto completo sin miedo al tamaño
    const infoTicket =
        `TICKET: #${ticketId}\n` +
        `PELICULA: ${movieTitle}\n` +
        `SESION: ${sessionInfo}\n` +
        `ASIENTOS: ${seats}\n` +
        `TOTAL: ${total}`;

    try {
        // Llamada a la librería BWIP-JS
        bwipjs.toCanvas('qrCanvas', {
            bcid: 'qrcode',       // Tipo de código
            text: infoTicket,     // El texto con los 15 asientos
            scale: 3,              // Resolución (3 es ideal para impresión)
            height: 50,             // Proporción
            width: 50,
            includetext: false,          // No queremos el texto debajo del QR
            textxalign: 'center',
        });
        console.log("QR Industrial generado correctamente con bwip-js");
    } catch (e) {
        console.error("Error generando QR:", e);
    }
});