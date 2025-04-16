document.addEventListener('DOMContentLoaded', function () {
    // Hotel toggle functionality
    document.getElementById('hotelToggle').addEventListener('change', function () {
        if (this.checked) {
            document.getElementById('includeHotel').value = 'true';
            document.getElementById('hotelSelection').style.display = 'block';
        } else {
            document.getElementById('includeHotel').value = 'false';
            document.getElementById('hotelSelection').style.display = 'none';
            document.getElementById('selectedHotelId').value = '';
            document.getElementById('selectedHotelInfo').style.display = 'none';
            document.getElementById('hotelSelect').value = '';
        }
    });

    // Hotel selection
    document.getElementById('selectHotelBtn').addEventListener('click', function () {
        var select = document.getElementById('hotelSelect');
        var selectedOption = select.options[select.selectedIndex];

        if (selectedOption.value === '') {
            alert('Пожалуйста, выберите отель из списка');
            return;
        }

        // Set hidden field value
        document.getElementById('selectedHotelId').value = selectedOption.value;

        // Display hotel info
        document.getElementById('hotelName').textContent = selectedOption.dataset.name;
        document.getElementById('hotelRating').innerHTML = 'Рейтинг: ' + '★'.repeat(selectedOption.dataset.stars);
        document.getElementById('hotelLocation').textContent = 'Местоположение: ' +
            selectedOption.dataset.city + ', ' + selectedOption.dataset.country;
        document.getElementById('hotelNutrition').textContent = 'Питание: ' + selectedOption.dataset.nutrition;

        // Show hotel info and hide selector
        select.closest('.input-group').style.display = 'none';
        document.getElementById('selectedHotelInfo').style.display = 'block';
    });

    // Change hotel button
    document.getElementById('changeHotelBtn').addEventListener('click', function () {
        document.getElementById('selectedHotelId').value = '';
        document.getElementById('selectedHotelInfo').style.display = 'none';
        document.getElementById('hotelSelect').closest('.input-group').style.display = 'flex';
        document.getElementById('hotelSelect').value = '';
    });

    // Form validation
    document.getElementById('bookingForm').addEventListener('submit', function (e) {
        if (document.getElementById('hotelToggle').checked &&
            document.getElementById('selectedHotelId').value === '') {
            e.preventDefault();
            alert('Пожалуйста, выберите отель');
            return false;
        }
        return true;
    });
});