document.addEventListener('DOMContentLoaded', function () {
    console.log('Админ-панель: Инициализация');

    const tabContents = {
        accounts: document.getElementById('accounts-content'),
        tours: document.getElementById('tours-content'),
        hotels: document.getElementById('hotels-content'),
        bookings: document.getElementById('bookings-content')
    };

    const tabButtons = {
        accounts: document.getElementById('accounts'),
        tours: document.getElementById('tours'),
        hotels: document.getElementById('hotels'),
        bookings: document.getElementById('bookings')
    };

    let hasErrors = false;
    Object.entries(tabContents).forEach(([name, element]) => {
        if (!element) {
            console.error(`Контентная вкладка "${name}" не найдена`);
            hasErrors = true;
        }
    });
    Object.entries(tabButtons).forEach(([name, element]) => {
        if (!element) {
            console.error(`Кнопка вкладки "${name}" не найдена`);
            hasErrors = true;
        }
    });
    if (hasErrors) return;

    const slider = document.querySelector('.admin-tabs-slider');
    if (!slider) {
        console.error('Слайдер не найден');
        return;
    }

    function updateActiveTab() {
        Object.values(tabContents).forEach(content => {
            content.classList.remove('active');
        });

        const activeTab = Object.keys(tabButtons).find(key => tabButtons[key].checked);

        if (activeTab && tabContents[activeTab]) {
            tabContents[activeTab].classList.add('active');
            const activeLabel = document.querySelector(`label[for="${activeTab}"]`);

            if (activeLabel) {
                slider.style.width = `${activeLabel.offsetWidth}px`;
                slider.style.transform = `translateX(${activeLabel.offsetLeft}px)`;
                console.log(`Активная вкладка: ${activeLabel.textContent.trim()}`);
            }
        }
    }

    Object.values(tabButtons).forEach(button => {
        button.addEventListener('change', updateActiveTab);
    });

    updateActiveTab();
    console.log('Админ-панель: Инициализация завершена');
});