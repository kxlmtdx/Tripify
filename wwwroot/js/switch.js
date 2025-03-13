document.addEventListener('DOMContentLoaded', function () {
    console.log('DOM fully loaded and parsed');

    const activeRadio = document.getElementById('active');
    const archivedRadio = document.getElementById('archived');
    const activeOrders = document.getElementById('active-orders');
    const archivedOrders = document.getElementById('archived-orders');

    if (!activeRadio || !archivedRadio || !activeOrders || !archivedOrders) {
        console.error('One or more elements not found!');
        return;
    }

    function toggleOrders() {
        if (activeRadio.checked) {
            activeOrders.style.display = 'block';
            archivedOrders.style.display = 'none';
            console.log('Active orders shown');
        } else if (archivedRadio.checked) {
            activeOrders.style.display = 'none';
            archivedOrders.style.display = 'block';
            console.log('Archived orders shown');
        }
    }

    activeRadio.addEventListener('change', toggleOrders);
    archivedRadio.addEventListener('change', toggleOrders);

    toggleOrders();
});