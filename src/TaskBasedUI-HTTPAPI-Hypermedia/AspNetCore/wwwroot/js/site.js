const sku = 'abc123';

async function saveProduct(sku, name, description) {
    const url = `/catalog/products/${sku}`;
    await fetch(url, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            name: name,
            description: description
        })
    });
}

async function increasePrice(sku, price) {
    const url = `/sales/products/${sku}/increasePrice`;
    await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            price: price
        })
    });

    const product = await getSalesProduct(sku);
    await setSalesInfo(product);
}

async function inventoryAdjustment(sku, adjustmentQuantity) {
    const url = `/warehouse/products/${sku}/inventoryAdjustment`;
    await fetch(url, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            adjustmentQuantity: adjustmentQuantity
        })
    });

    const product = await getQuantityOnHand(sku);
    setQuantityOnHandLabel(product);

    await loadSales();
}

async function getQuantityOnHand(sku) {
    const url = `/warehouse/products/${sku}`;
    const response = await fetch(url);
    return await response.json();
}

function setQuantityOnHandLabel(product) {
    document.getElementById('quantityOnHand').innerText = product.quantityOnHand;
}

async function getProductInfo(sku) {
    const url = `/catalog/products/${sku}`;
    const response = await fetch(url);
    return await response.json();
}

function setProductInfo(product) {
    const productName = document.getElementById('productName');
    if (productName) {
        productName.value = product.name;
    }

    const productDescription = document.getElementById('productDescription');
    if (productDescription) {
        productDescription.value = product.description;
    }
}

async function getSalesProduct(sku) {
    const url = `/sales/products/${sku}`;
    const response = await fetch(url);
    return await response.json();
}

async function setSalesInfo(product) {
    document.getElementById('productPrice').innerText = product.price;
    document.getElementById('forSaleStatus').innerText = product.forSale ? 'Available' : 'Unavailable';

    const setAsUnavailableLink = product.actions.find(x => x.name === 'UnavailableForSale');
    const setAsUnavailableItem = document.getElementById('setAsUnavailable');
    setAsUnavailableItem.style.display = setAsUnavailableLink ? 'block' : 'none';
    setAsUnavailableItem.dataset.href = setAsUnavailableLink?.href;

    const setAsAvailableLink = product.actions.find(x => x.name === 'AvailableForSale');
    const setAsAvailableItem = document.getElementById('setAsAvailable');
    setAsAvailableItem.style.display = setAsAvailableLink ? 'block' : 'none';
    setAsAvailableItem.dataset.href = setAsAvailableLink?.href;
}

async function setAsAvailable() {
    const item = document.getElementById('setAsAvailable');
    await fetch(item.dataset.href,{ method: 'POST' });
    await loadSales();
}

async function setAsUnavailable() {
    const item = document.getElementById('setAsUnavailable');
    await fetch(item.dataset.href,{ method: 'POST' });
    await loadSales();
}

async function loadSales() {
    const salesProduct = await getSalesProduct(sku);
    await setSalesInfo(salesProduct);
}

async function load() {

    const warehouseProduct = await getQuantityOnHand(sku);
    setQuantityOnHandLabel(warehouseProduct);

    const catalogProduct = await getProductInfo(sku);
    setProductInfo(catalogProduct);

    await loadSales(sku);
}

load();