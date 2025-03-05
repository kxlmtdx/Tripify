// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Flatpickr
flatpickr("#date", {
    dateFormat: "d.m.Y",
    locale: "ru",
    minDate: "today",
});

// Swiper
var swiper = new Swiper('.swiper-container', {
    slidesPerView: 'auto',
    spaceBetween: 20,
    pagination: {
        el: '.swiper-pagination',
        clickable: true,
    },
});