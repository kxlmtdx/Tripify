document.addEventListener('DOMContentLoaded', function () {
    console.log('Админ-панель: DOM полностью загружен');

    const accountsTab = document.getElementById('accounts');
    const toursTab = document.getElementById('tours');
    const hotelsTab = document.getElementById('hotels');
    const statusTab = document.getElementById('status');

    const accountsContent = document.getElementById('accounts-content');
    const toursContent = document.getElementById('tours-content');
    const hotelsContent = document.getElementById('hotels-content');
    const statusContent = document.getElementById('status-content');

    const elements = [accountsTab, toursTab, hotelsTab, statusTab,
        accountsContent, toursContent, hotelsContent, statusContent];

    for (const el of elements) {
        if (!el) {
            console.error('undefined');
            return;
        }
    }
    console.log('Все элементы управления найдены');

    function switchTabs() {
        console.log('Переключение вкладок...');

        accountsContent.style.display = 'none';
        toursContent.style.display = 'none';
        hotelsContent.style.display = 'none';
        statusContent.style.display = 'none';

        if (accountsTab.checked) {
            accountsContent.style.display = 'block';
            console.log('Показан раздел "Аккаунты"');
        }
        else if (toursTab.checked) {
            toursContent.style.display = 'block';
            console.log('Показан раздел "Туры"');
        }
        else if (hotelsTab.checked) {
            hotelsContent.style.display = 'block';
            console.log('Показан раздел "Отели"');
        }
        else if (statusTab.checked) {
            statusContent.style.display = 'block';
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
        if (accountsTab.checked) activeLabel = document.querySelector('label[for="accounts"]');
        else if (toursTab.checked) activeLabel = document.querySelector('label[for="tours"]');
        else if (hotelsTab.checked) activeLabel = document.querySelector('label[for="hotels"]');
        else if (statusTab.checked) activeLabel = document.querySelector('label[for="status"]');

        if (activeLabel) {
            slider.style.width = activeLabel.offsetWidth + 'px';
            slider.style.transform = `translateX(${activeLabel.offsetLeft}px)`;
            console.log(`Слайдер перемещен к "${activeLabel.textContent}"`);
        }
    }
    u
    accountsTab.addEventListener('change', switchTabs);
    toursTab.addEventListener('change', switchTabs);
    hotelsTab.addEventListener('change', switchTabs);
    statusTab.addEventListener('change', switchTabs);

    console.log('Обработчики событий назначены');

    switchTabs();
    console.log('Инициализация завершена');
});