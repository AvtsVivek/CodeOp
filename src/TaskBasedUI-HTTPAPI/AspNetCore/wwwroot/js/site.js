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
    setSalesInfo(product);
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
    document.getElementById('productName').value = product.name;
    document.getElementById('productDescription').value = product.description;
}

async function getSalesProduct(sku) {
    const url = `/sales/products/${sku}`;
    const response = await fetch(url);
    return await response.json();
}

function setSalesInfo(product) {
    document.getElementById('productPrice').innerText = product.price;
}

async function load() {

    const sku = 'abc123';

    const warehouseProduct = await getQuantityOnHand(sku);
    setQuantityOnHandLabel(warehouseProduct);

    const catalogProduct = await getProductInfo(sku);
    setProductInfo(catalogProduct);

    const salesProduct = await getSalesProduct(sku);
    setSalesInfo(salesProduct);
}

load();