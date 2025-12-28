document.addEventListener('DOMContentLoaded', () => {
    //Inicializacion y captura de elementos
    const select = document.getElementById('addonTypeSelect');
    const container = document.getElementById('addonsTableContainer');
    const grandTotalEl = document.getElementById('grandTotal');
    const seatsSubtotal = parseFloat(document.getElementById('seatsSubtotalRaw').value) || 0;
    //Colecciones para guardar la cantidad de complementos y el precio de cada uno de ellos
    let addonQuantities = {};
    let addonPrices = {};

    //Esta funcion pinta la tabla de productos a traves de una lista de estos mismos
    const renderAddons = (addons) => {
        //borramos el Html para dejarlo vacio cada vez que el ciclo entre en esta funcion y repintar todo
        container.innerHTML = '';
        //Si no hay addons, saldrá este mensaje
        if (addons.length === 0) {
            container.innerHTML = '<p>No hay complementos disponibles.</p>';
            return;
        }
        //Creamos la tabla con sus atributos y sus cabeceras
        const table = document.createElement('table');
        table.className = 'table text-center align-middle';
        table.innerHTML = `<thead><tr><th colspan="2" class="text-center ps-4">Producto</th><th>Precio</th><th>Cantidad</th><th>Total</th></tr></thead>`;        const tbody = document.createElement('tbody');
        //recorremos los addons
        addons.forEach(addon => {
            //Primero comprobamos siel id del addon que se va a representar ya existe. Si no es así,
            //lo inicializa en 0 para que al cambiar de categoria las cantidades no se pierdan
            if (!(addon.addOnId in addonQuantities)) {
                addonQuantities[addon.addOnId] = 0;
            }

            //Guardamos el complemento en un diccionario donde la clave es su ID y lo convertimos a Float
            //para que se puedan realizar operaciones luego con el
            addonPrices[addon.addOnId] = parseFloat(addon.price);

            //Creamos la fila y vamos añadiendo en ella todos los elementos necesarios para que el complemento
            //quede bien representado.
            //Es importante tambien recalcar que se han añadido los botones "+" y "-" para añadir o quitar cantidades
            const tr = document.createElement('tr');
            const imagePath = addon.addOnImage || addon.addonImage || "";
            tr.innerHTML = `
    <td>
        <img src="/${imagePath}" 
             alt="${addon.addOnName || addon.addonName}" 
             class="addon-img-thumbnail" 
             onerror="this.src='/img/no-photo.png'"/>
    </td>
    <td class="fs-5 fw-bold">${addon.addOnName || addon.addonName}</td>
    <td class="fs-5">${addon.price.toFixed(2)} €</td>
    <td class="col-quantity">
        <div class="d-flex justify-content-center align-items-center gap-2">
            <button type="button" class="btn btn-secondary quit-btn" disabled>-</button>
            <input type="number" value="0" min="0" class="form-control qty-input qty-input-custom" readonly />
            <button type="button" class="btn btn-secondary add-btn">+</button>
        </div>
    </td>
    <td class="addonTotal fs-5">0.00 €</td>`;

            const quitBtn = tr.querySelector('.quit-btn');
            const addBtn = tr.querySelector('.add-btn');
            const input = tr.querySelector('.qty-input');
            const totalCell = tr.querySelector('.addonTotal');

            //Recuperamos la cantidad guardada para el producto determinado
            const qty = addonQuantities[addon.addOnId] || 0;
            //La metemos en el input para que la refleje cuando cambie
            input.value = qty;
            totalCell.textContent = (qty * addon.price).toLocaleString('es-ES', {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            }) + ' €'; //Formateo del numero al estilo europeo (para que coja las "," como separador decimal)
            //Si la cantidad es 0 deshabilitamos el boton de "-" para evitar numeros negativos.
            quitBtn.disabled = qty === 0;
            //Funcion para guardar el total de cantidades en el total de compra
            const FinalTotal = () => {
                let addonsTotal = 0;
                //Recorremos todas las cantidades de todos los productos y hacemos su calculo
                for (let id in addonQuantities) {
                    addonsTotal += addonQuantities[id] * addonPrices[id];
                }
                //Cumamos todo el total de los productos al precio base de las entradas
                const totalCalculado = seatsSubtotal + addonsTotal;
                //Y actualizamos el total en la pantalla con las comas
                grandTotalEl.textContent =
                    totalCalculado.toLocaleString('es-ES', {
                        minimumFractionDigits: 2,
                        maximumFractionDigits: 2
                    }) + ' €';
            };
            //Escuchador para el boton de restar
            quitBtn.addEventListener('click', () => {
                //Leemos la cantidad actual del input
                let qty = parseInt(input.value || '0');
                //Si es mayor que 0
                if (qty > 0) {
                    //Restamos uno y actualizamos el valor del imput, formateando otra vez el numero total
                    qty--;
                    input.value = qty;
                    totalCell.textContent =
                        (qty * addon.price).toLocaleString('es-ES', {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        }) + ' €';
                    // Guardamos la nueva cantidad en el "diccionario" global de cantidades
                    addonQuantities[addon.addOnId] = qty;
                    // Si llegamos a 0 bloqueamos el botón de restar
                    if (qty === 0) quitBtn.disabled = true;
                    FinalTotal();
                }
            });

            //Escuchador del boton de sumar (tiene el mismo funcionamiento que el anterior, solo que este,
            //en vez de restar, suma)
            addBtn.addEventListener('click', () => {
                let qty = parseInt(input.value || '0');
                qty++;
                input.value = qty;
                totalCell.textContent =
                    (qty * addon.price).toLocaleString('es-ES', {
                        minimumFractionDigits: 2,
                        maximumFractionDigits: 2
                    }) + ' €';
                addonQuantities[addon.addOnId] = qty;
                quitBtn.disabled = false;
                FinalTotal();
            });
            //Añadimos la fila al body
            tbody.appendChild(tr);
        });
        //Añadimos el body a la tabla
        table.appendChild(tbody);
        //Añadimos la tabla en su container
        container.appendChild(table);
    };

    // Define una función asíncrona (async) que recibe el tipo de producto (Comida, Bebida...)
    const loadAddons = async (type = '') => {
        //Llamamos al Action del controlador encargado de devolver los productos segun el tipo con FETCH
        const response = await fetch(`/Movie/GetAddOnsByType?type=${type}`);
        //Guardamos la respuesta JSON en la variable...
        const addons = await response.json();
        //Y se la enviamos al metodo que renderiza la tabla para que los pinte
        renderAddons(addons);
    };

    // Si existe el desplegable de tipos (Comida, Bebida...), añade un listener al selector.
    // Cada vez que el usuario cambie la opción, llama a loadAddons con el nuevo valor (select.value).
    if (select) select.addEventListener('change', () => loadAddons(select.value));
    // Cargamos todos los productos por defecto nada más entrar a la página.
    loadAddons('All');

    const form = document.getElementById('addonsForm');

    //Si el formulario existe, escuchamos las pulsaciones del boton "Siguiente" con el evento del formulario "submit"
    if (form) {
        form.addEventListener('submit', (e) => {
            // Creamos un objeto vacío para el "paquete final".
            const selectedAddons = {};
            // Recorremos nuestro diccionario global donde hemos ido guardando cantidades.
            for (let id in addonQuantities) {
                // Solo incluimos en el paquete los productos cuya cantidad sea mayor a 0.
                if (addonQuantities[id] > 0) {
                    selectedAddons[id] = addonQuantities[id];
                }
            }
            // Convierte el objeto de JS en una cadena de texto con formato JSON.
            // Un ejemplo de esto sería de { "1": 2, "5": 1 } a '{"1":2,"5":1}'
            const jsonString = JSON.stringify(selectedAddons);
            // Recogemos el input oculto que esta dentro del formulario HTML. //inputOculto
            const inputHidden = document.getElementById('addonsDataInput');
            // Mete el texto JSON dentro del valor del input.
            // Al enviarse el formulario, el servidor recibirá este String y podrá deserializarlo para trabajar con el.
            if (inputHidden) {
                inputHidden.value = jsonString;
            }
            //Guardamos las cantidades y precios de lso complementos en el Session
            sessionStorage.setItem('addonQuantities', jsonString);
            sessionStorage.setItem('addonPrices', JSON.stringify(addonPrices));
        });
    }
});
