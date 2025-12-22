document.addEventListener("DOMContentLoaded", () => {
    const icon = document.querySelector(".background-icon");

    window.addEventListener("scroll", () => {
        const scroll = window.scrollY;

        // Efecto parallax: el icono se mueve más lento que el scroll
        icon.style.transform = `translateY(${scroll * 0.2}px)`;
    });
});