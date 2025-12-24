document.addEventListener("DOMContentLoaded", () => {
    const deleteButtons = document.querySelectorAll('.btn-delete-item');

    deleteButtons.forEach(button => {
        button.addEventListener('click', async () => {
            const id = button.getAttribute('data-id');
            const name = button.getAttribute('data-name');
            const type = button.getAttribute('data-type');

            if (confirm(`¿Seguro que deseas eliminar a ${type}: ${name}?`)) {
                // Seleccionamos el token correctamente
                const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
                if (!tokenElement) {
                    console.error("No se encontró el token de verificación.");
                    return;
                }
                const token = tokenElement.value;

                try {
                    // fetch
                    const response = await fetch(`/Admin/Delete${type}Ajax/${id}`, {
                        method: 'POST',
                        headers: {
                            // ASP.NET busca el token aquí cuando es AJAX
                            'RequestVerificationToken': token,
                            'X-Requested-With': 'XMLHttpRequest'
                        }
                    });

                    // Si el servidor devuelve 400 o 500, response.json() fallará y saltará al catch
                    if (!response.ok) {
                        const errorText = await response.text();
                        throw new Error(`Error del servidor (${response.status}): ${errorText}`);
                    }

                    const data = await response.json();

                    if (data.success) {
                        const rowId = `row-${type.toLowerCase()}-${id}`;
                        const row = document.getElementById(rowId);

                        if (row) {
                            row.style.transition = "all 0.4s ease";
                            row.style.opacity = "0";
                            row.style.transform = "translateX(20px)";
                            setTimeout(() => row.remove(), 400);
                        }
                    } else {
                        alert("Error: " + data.message);
                    }
                } catch (error) {
                    console.error('Error detallado:', error);
                    alert("No se pudo completar la operación. Revisa la consola.");
                }
            }
        });
    });
});