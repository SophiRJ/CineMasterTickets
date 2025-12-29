//Este documento JS simplemente sirve para darle efectos al icono de detras del fondo de pantalla
document.addEventListener("DOMContentLoaded", () => {
    const icon = document.querySelector(".background-icon");

    //capturamos el scroll con un escuchador directo a la pantalla
    window.addEventListener("scroll", () => {
        //Lo guardamos en una variable
        const scroll = window.scrollY;

        // Modificando la propiedad transform del icono, podemos hacer que se mueva mas lento
        //hacia abajo(eje Y) restandole valor al movimiento scroll
        icon.style.transform = `translateY(${scroll * 0.2}px)`;
    });
});