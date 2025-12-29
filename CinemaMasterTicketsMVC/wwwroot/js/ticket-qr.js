document.addEventListener("DOMContentLoaded", () => {
    // 1. Creamos un elemento canvas dentro de tu div 'qrcode'
    const qrContainer = document.getElementById("qrcode");
    if (!qrContainer) return;

    // Necesitamos crear una etiqueta CANVAS ya que el QR no es una imagen estatica, se dibuja en tiempo real.
    qrContainer.innerHTML = '<canvas id="qrCanvas"></canvas>';

    //Capturamos los datos necesarios de los elementos de la vista para meterlos en el Qr
    const ticketId = qrContainer.getAttribute("data-ticket-id");
    const movieTitle = document.querySelector(".ticket-visual h3")?.innerText || "";
    const sessionInfo = document.querySelector(".ticket-visual h3 + p")?.innerText || "";
    const seats = document.querySelector(".ticket-visual .fw-bold")?.innerText || "";
    const total = document.querySelector(".total-row .fw-bold")?.innerText || "";

    // Texto completo que mostrará el QR cuando lo escaneemos con el movil
    const infoTicket =
        `TICKET: #${ticketId}\n` +
        `PELICULA: ${movieTitle}\n` +
        `SESION: ${sessionInfo}\n` +
        `ASIENTOS: ${seats}\n` +
        `TOTAL: ${total}`;

    try {
        // Llamada a la librería BWIP-JS (Barcode Writer in Pure JavaScript) la cual no genera QRs pequeños
        //con capacidad limitada de datos, sino que genera QRs vectoriales de alta precisión que permiten
        //alojar bastante mas cantidad de datos. Habiamos utilizado otros, pero se nos quedaban cortos.
        //Esta funcion pinta el Qr sobre el contenedor "canvas que habiamos definido arriba" con los siguientes
        //datos necesarios para su formación
        bwipjs.toCanvas('qrCanvas', {
            bcid: 'qrcode',       // Tipo de código
            text: infoTicket,     // El texto con la info del ticket
            scale: 3,              // Resolución (3 es ideal para impresión)
            height: 50,             // Proporción
            width: 50,
            includetext: false,          // No queremos el texto debajo del QR
            textxalign: 'center',
        });
        //Logs de acierto o error simplemente por las pruebas realizadas
        console.log("QR Industrial generado correctamente con bwip-js");
    } catch (e) {
        console.error("Error generando QR:", e);
    }
});