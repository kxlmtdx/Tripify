document.addEventListener('DOMContentLoaded', function () {
    console.log('Админ-панель: DOM полностью загружен');

    const elements = {
        tabs: {
            accounts: document.getElementById('accounts'),
            tours: document.getElementById('tours'),
            hotels: document.getElementById('hotels'),
            status: document.getElementById('status')
        },
        content: {
            accounts: document.getElementById('accounts-content'),
            tours: document.getElementById('tours-content'),
            hotels: document.getElementById('hotels-content'),
            status: document.getElementById('status-content')
        }
    };


    for (const type in elements) {
        for (const key in elements[type]) {
            if (!elements[type][key]) {
                console.error('Element not found:', key);
                return;
            }
        }
    }
    console.log('Все элементы управления найдены');

    function switchTabs() {
        console.log('Переключение вкладок...');

        for (const key in elements.content) {
            elements.content[key].style.display = 'none';
        }

        if (elements.tabs.accounts.checked) {
            elements.content.accounts.style.display = 'block';
            console.log('Показан раздел "Аккаунты"');
        }
        else if (elements.tabs.tours.checked) {
            elements.content.tours.style.display = 'block';
            console.log('Показан раздел "Туры"');
        }
        else if (elements.tabs.hotels.checked) {
            elements.content.hotels.style.display = 'block';
            console.log('Показан раздел "Отели"');
        }
        else if (elements.tabs.status.checked) {
            elements.content.status.style.display = 'block';
            console.log('Показан раздел "Статус"');
        }

        updateSliderPosition();
    }

    function updateSliderPosition() {
        const slider = document.querySelector('.admin-tabs-slider');
        if (!slider) {
            console.error('Слайдер не найден');
            return;
        }

        let activeLabel;
        if (elements.tabs.accounts.checked) {
            activeLabel = document.querySelector('label[for="accounts"]');
        }
        else if (elements.tabs.tours.checked) {
            activeLabel = document.querySelector('label[for="tours"]');
        }
        else if (elements.tabs.hotels.checked) {
            activeLabel = document.querySelector('label[for="hotels"]');
        }
        else if (elements.tabs.status.checked) {
            activeLabel = document.querySelector('label[for="status"]');
        }

        if (activeLabel) {
            slider.style.width = activeLabel.offsetWidth + 'px';
            slider.style.transform = `translateX(${activeLabel.offsetLeft}px)`;
            console.log(`Слайдер перемещен к "${activeLabel.textContent}"`);
        }
    }

    for (const key in elements.tabs) {
        elements.tabs[key].addEventListener('change', switchTabs);
    }

    console.log('Обработчики событий назначены');

    switchTabs();
    console.log('Инициализация завершена');
});