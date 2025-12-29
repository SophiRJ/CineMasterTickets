document.addEventListener("DOMContentLoaded", () => {
    const deleteButtons = document.querySelectorAll('.btn-delete-item');

    deleteButtons.forEach(button => {
        button.addEventListener('click', async () => {
            const id = button.getAttribute('data-id');
            const name = button.getAttribute('data-name');

            //Antes teniamos un metodo AJAX tanto para empleado como para cliente. Despues de unos cambios, decidimos que el
            //cliente no podria ser borrado por el Admin, sino que se daría de baja por su cuenta propia. Esta variable
            //Se encargaba de recoger el tipo de usuario para meterlo en la llamada AJAx. Podia llamar a DeleteCustomerAJAX
            //o DeleteEmployeeAjax. Ahora solo funciona con Employee, pero se deja para que se vea el procedimiento que hemos segido
            //ya que nos ha parecido bastante interesante 
            const type = button.getAttribute('data-type');

            //Confirmacion de seguridad antes de borrar
            if (confirm(`¿Seguro que deseas eliminar a ${type}: ${name}?`)) {
                // Recuperamos el token para enviarlo al Action
                const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
                if (!tokenElement) {
                    console.error("No se encontró el token de verificación.");
                    return;
                }
                const token = tokenElement.value;

                try {
                    // El fetch que llamara a DeleteEmployeeAjax/id para borrar el empleado desde el servidor
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

                    //Con la respuesta ya de vuelta
                    const data = await response.json();

                    if (data.success) {
                        //Construimos el nombre que tendra la fila
                        const rowId = `row-${type.toLowerCase()}-${id}`;
                        const row = document.getElementById(rowId);

                        //Si la encuentra, le damos efectos de animación
                        if (row) {
                            row.style.transition = "all 0.4s ease";
                            row.style.opacity = "0";
                            row.style.transform = "translateX(20px)";
                            setTimeout(() => row.remove(), 400);
                        }
                        //Si no, capturamos errores
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