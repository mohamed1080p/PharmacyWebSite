// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Import Components
import './components/darkMode.js';
import './components/cart.js';

// Global Functions
function showLoading() {
    const spinner = document.createElement('div');
    spinner.className = 'spinner';
    document.body.appendChild(spinner);
}

function hideLoading() {
    const spinner = document.querySelector('.spinner');
    if (spinner) {
        spinner.remove();
    }
}

// Add ripple effect to buttons
document.addEventListener('click', function (e) {
    if (e.target.classList.contains('btn')) {
        const ripple = document.createElement('span');
        ripple.className = 'ripple';
        e.target.appendChild(ripple);

        const rect = e.target.getBoundingClientRect();
        const size = Math.max(rect.width, rect.height);
        ripple.style.width = ripple.style.height = `${size}px`;

        const x = e.clientX - rect.left - size / 2;
        const y = e.clientY - rect.top - size / 2;
        ripple.style.left = `${x}px`;
        ripple.style.top = `${y}px`;

        setTimeout(() => ripple.remove(), 600);
    }
});

// Smooth scroll for anchor links
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    });
});

// Initialize tooltips
const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl);
});

// Initialize popovers
const popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
popoverTriggerList.map(function (popoverTriggerEl) {
    return new bootstrap.Popover(popoverTriggerEl);
});

// Form validation
document.querySelectorAll('form').forEach(form => {
    form.addEventListener('submit', function (e) {
        if (!form.checkValidity()) {
            e.preventDefault();
            e.stopPropagation();
        }
        form.classList.add('was-validated');
    });
});

// Lazy loading for images
document.addEventListener('DOMContentLoaded', function () {
    const lazyImages = document.querySelectorAll('img[data-src]');
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.removeAttribute('data-src');
                observer.unobserve(img);
            }
        });
    });

    lazyImages.forEach(img => imageObserver.observe(img));
});

// Animated Page Transitions
function fadeInPage() {
    document.body.style.opacity = 0;
    document.body.style.transition = 'opacity 0.5s';
    setTimeout(() => { document.body.style.opacity = 1; }, 50);
}
window.addEventListener('DOMContentLoaded', fadeInPage);

// Global Modal and Toast System (Bootstrap based)
window.showGlobalToast = function (type, message) {
    const toastId = type === 'success' ? 'successToast' : 'errorToast';
    const toastElement = document.getElementById(toastId);
    if (toastElement) {
        toastElement.querySelector('.toast-body').textContent = message;
        const toast = new bootstrap.Toast(toastElement);
        toast.show();
    }
};
