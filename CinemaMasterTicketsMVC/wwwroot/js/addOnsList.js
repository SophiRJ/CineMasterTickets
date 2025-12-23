document.addEventListener('DOMContentLoaded', () => {
    const select = document.getElementById('addonTypeSelect');
    const container = document.getElementById('addonsTableContainer');
    const grandTotalEl = document.getElementById('grandTotal');

    const seatsSubtotal = parseFloat(document.getElementById('seatsSubtotalRaw').value) || 0;

    let addonQuantities = {};
    let addonPrices = {};

    const renderAddons = (addons) => {
        container.innerHTML = '';
        if (addons.length === 0) {
            container.innerHTML = '<p>No hay complementos disponibles.</p>';
            return;
        }

        const table = document.createElement('table');
        table.className = 'table';
        table.innerHTML = `<thead><tr><th>Imagen</th><th>Producto</th><th>Precio</th><th>Cantidad</th><th>Total</th></tr></thead>`;
        const tbody = document.createElement('tbody');

        addons.forEach(addon => {

            if (!(addon.addOnId in addonQuantities)) {
                addonQuantities[addon.addOnId] = 0;
            }

            addonPrices[addon.addOnId] = parseFloat(addon.price);

            const tr = document.createElement('tr');


            const imagePath = addon.addOnImage || addon.addonImage || "";

            tr.innerHTML = `
                <td>
                <img src="/${imagePath}" 
                         alt="${addon.addOnName || addon.addonName}" 
                         style="width:50px; height:50px; object-fit:cover;" 
                         onerror="this.src='/img/no-photo.png'"/>
                </td>
                <td>${addon.addOnName || addon.addonName}</td>
                <td>${addon.price.toFixed(2)} €</td>
                <td>
                    <div class="input-group">
                        <button type="button" class="btn btn-secondary quit-btn" disabled>-</button>
                        <input type="number" value="0" min="0" class="form-control text-center qty-input" readonly style="width:60px"/>
                        <button type="button" class="btn btn-secondary add-btn">+</button>
                    </div>
                </td>
                <td class="addonTotal">0.00 €</td>`;

            const quitBtn = tr.querySelector('.quit-btn');
            const addBtn = tr.querySelector('.add-btn');
            const input = tr.querySelector('.qty-input');
            const totalCell = tr.querySelector('.addonTotal');

            const qty = addonQuantities[addon.addOnId] || 0;
            input.value = qty;
            totalCell.textContent = (qty * addon.price).toLocaleString('es-ES', {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            }) + ' €';
            quitBtn.disabled = qty === 0;

            const FinalTotal = () => {
                let addonsTotal = 0;
                for (let id in addonQuantities) {
                    addonsTotal += addonQuantities[id] * addonPrices[id];
                }
                const totalCalculado = seatsSubtotal + addonsTotal;
                grandTotalEl.textContent =
                    totalCalculado.toLocaleString('es-ES', {
                        minimumFractionDigits: 2,
                        maximumFractionDigits: 2
                    }) + ' €';
            };

            quitBtn.addEventListener('click', () => {
                let qty = parseInt(input.value || '0');
                if (qty > 0) {
                    qty--;
                    input.value = qty;
                    totalCell.textContent =
                        (qty * addon.price).toLocaleString('es-ES', {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        }) + ' €';
                    addonQuantities[addon.addOnId] = qty;
                    if (qty === 0) quitBtn.disabled = true;
                    FinalTotal();
                }
            });

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

            tbody.appendChild(tr);
        });

        table.appendChild(tbody);
        container.appendChild(table);
    };

    const loadAddons = async (type = '') => {
        const response = await fetch(`/Movie/GetAddOnsByType?type=${type}`);
        const addons = await response.json();
        renderAddons(addons);
    };

    if (select) select.addEventListener('change', () => loadAddons(select.value));
    loadAddons('All');

    document.querySelector('form').addEventListener('submit', () => {
        //Filtrado apra que solo guarde los addOns que su cantidad sea >0
        const selectedAddons = {};
        for (let id in addonQuantities) {
            if (addonQuantities[id] > 0) {
                selectedAddons[id] = addonQuantities[id];
            }
        }

        // 3️⃣ Guardar en sessionStorage (opcional)
        sessionStorage.setItem('addonQuantities', JSON.stringify(selectedAddons));
        sessionStorage.setItem('addonPrices', JSON.stringify(addonPrices));

        // 4️⃣ Guardar en input oculto para enviar al controller
        document.getElementById('addonsData').value = JSON.stringify(selectedAddons);
    });
});
