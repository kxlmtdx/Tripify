document.getElementById('Document_Type_Id').addEventListener('change', function () {
    const internationalFields = document.querySelector('.international-fields');
    internationalFields.style.display = this.value === '4' ? 'block' : 'none';

    if (this.value !== '4') {
        document.getElementById('International_Document_Number').value = '';
        document.getElementById('Expiration_Date').value = '';
    }
});