document.addEventListener("DOMContentLoaded", () => {
    // 1. Capturamos solo los botones que tienen la clase específica
    const toggleButtons = document.querySelectorAll('.btn-toggle');
    const activeBody = document.getElementById("active-addons-body");
    const inactiveBody = document.getElementById("inactive-addons-body");

    //Para cada boton de Activar-Desactivar le metemos un escuchador
    toggleButtons.forEach(button => {
        button.addEventListener('click', async (e) => {
            e.preventDefault();
            //Y capturamos su id y su fila completa
            const addonId = button.getAttribute("data-id");
            const row = button.closest('tr'); 

            // Desactivamos el boton mientras el servidor trabaja para que el usuario no lo sobrecargue
            button.disabled = true;
            //Guardamos el texto del boton para saber en que direccion va el cambio
            const originalText = button.textContent.trim();
            //Quitamos el boton y lo cambiamos por un circulo de carga para que se vea que esta trabajando
            button.innerHTML = '<span class="spinner-border spinner-border-sm"></span>';

            try {
                //Aqui llamamos con Fetch al servidor para que nos cambie el estado del boton y nos lo devuelva
                //en formato JSON. Tambien le enviamos el Token para que el ForgeryToken del action nos acepte la llamada
                const response = await fetch(`/Admin/ToggleStatus?id=${addonId}`, {
                    method: 'POST',
                    headers: {
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value,
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });
                //Recogemos la respuesta
                const data = await response.json();

                if (data.success) {
                    //Si es OK, le metemos un efecto dinánico de cambio entre tablas
                    row.style.transition = "all 0.4s ease";
                    row.style.opacity = "0";
                    location.reload();
                    //Lo hacemos con un SetTimeOut para darle mas efecto
                    setTimeout(() => {
                        // Comprobamos en qué tabla está para saber a dónde moverlo
                        if (originalText === "Desactivar") {
                            // Cambiamos el botón para su nuevo estado en la otra tabla
                            button.textContent = "Activar";
                            button.className = "btn btn-success btn-sm btn-toggle";
                            inactiveBody.appendChild(row);
                        } else {
                            button.textContent = "Desactivar";
                            button.className = "btn btn-warning btn-sm btn-toggle";
                            activeBody.appendChild(row);
                        }

                        // Restauramos la visibilidad (mas efectos visuales)
                        row.style.opacity = "1";
                        button.disabled = false;
                    }, 400);
                    //Manejamos errores
                } else {
                    alert("Error: " + data.message);
                    button.disabled = false;
                    button.textContent = originalText;
                }
            } catch (error) {
                alert("Error de conexión con el servidor");
                button.disabled = false;
                button.textContent = originalText;
            }
        });
    });
});