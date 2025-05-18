// Dark Mode Toggle Functionality
class DarkMode {
    constructor() {
        this.darkModeToggle = document.querySelector('.dark-mode-toggle');
        this.isDarkMode = localStorage.getItem('darkMode') === 'true';
        this.init();
    }

    init() {
        // Set initial state
        if (this.isDarkMode) {
            document.body.classList.add('dark-mode');
        }

        // Add event listener
        if (this.darkModeToggle) {
            this.darkModeToggle.addEventListener('click', () => this.toggle());
        }

        // Listen for system dark mode changes
        if (window.matchMedia) {
            window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', e => {
                if (!localStorage.getItem('darkMode')) {
                    this.setDarkMode(e.matches);
                }
            });
        }
    }

    toggle() {
        this.isDarkMode = !this.isDarkMode;
        this.setDarkMode(this.isDarkMode);
        localStorage.setItem('darkMode', this.isDarkMode);
    }

    setDarkMode(isDark) {
        if (isDark) {
            document.body.classList.add('dark-mode');
            this.darkModeToggle.innerHTML = '<i class="bi bi-sun"></i>';
        } else {
            document.body.classList.remove('dark-mode');
            this.darkModeToggle.innerHTML = '<i class="bi bi-moon"></i>';
        }
    }
}

// Initialize dark mode
document.addEventListener('DOMContentLoaded', () => {
    new DarkMode();
}); 