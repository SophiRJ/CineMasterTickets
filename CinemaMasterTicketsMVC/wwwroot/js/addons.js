document.addEventListener("DOMContentLoaded", () => {
    // Usamos document directamente para capturar clics en CUALQUIER botón de la página
    document.addEventListener("click", async (e) => {

        // Verificamos si lo que se clickeó fue un botón con la clase btn-toggle
        if (e.target.classList.contains("btn-toggle")) {
            const button = e.target;
            const addonId = button.getAttribute("data-id");

            console.log("Cambiando estado para ID:", addonId); // Para debug

            try {
                // Importante: Asegúrate que la ruta coincide con tu AdminController
                const response = await fetch(`/Admin/ToggleStatus?id=${addonId}`, {
                    method: 'POST',
                    headers: {
                        // Captura el Token de seguridad que pusimos en la vista
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                    }
                });

                if (!response.ok) {
                    throw new Error("Error en la respuesta del servidor");
                }

                const data = await response.json();

                if (data.success) {
                    console.log("¡Éxito!");
                    // Recargamos para que los elementos salten de una tabla a otra
                    location.reload();
                } else {
                    alert("Error: " + data.message);
                }
            } catch (error) {
                console.error("Error en la petición AJAX:", error);
            }
        }
    });
});