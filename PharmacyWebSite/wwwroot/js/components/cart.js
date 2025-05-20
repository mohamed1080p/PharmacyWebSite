// Shopping Cart Functionality
class Cart {
    constructor() {
        this.cart = JSON.parse(localStorage.getItem('cart')) || [];
        this.cartCount = document.querySelector('#cartCount');
        this.init();
    }

    init() {
        // Update cart count on load
        this.updateCartCount();

        // Add event listeners to all add-to-cart buttons
        document.querySelectorAll('.add-to-cart').forEach(button => {
            button.addEventListener('click', (e) => {
                const productId = e.target.dataset.productId;
                const productName = e.target.dataset.productName;
                const productPrice = parseFloat(e.target.dataset.productPrice);
                this.addItem(productId, productName, productPrice);
            });
        });
    }

    addItem(id, name, price) {
        const existingItem = this.cart.find(item => item.id === id);

        if (existingItem) {
            existingItem.quantity += 1;
        } else {
            this.cart.push({
                id,
                name,
                price,
                quantity: 1
            });
        }

        this.saveCart();
        this.updateCartCount();
        this.showToast(`${name} added to cart`);
    }

    removeItem(id) {
        this.cart = this.cart.filter(item => item.id !== id);
        this.saveCart();
        this.updateCartCount();
    }

    updateQuantity(id, quantity) {
        const item = this.cart.find(item => item.id === id);
        if (item) {
            item.quantity = Math.max(0, quantity);
            if (item.quantity === 0) {
                this.removeItem(id);
            } else {
                this.saveCart();
                this.updateCartCount();
            }
        }
    }

    getTotal() {
        return this.cart.reduce((total, item) => total + (item.price * item.quantity), 0);
    }

    saveCart() {
        localStorage.setItem('cart', JSON.stringify(this.cart));
    }

    updateCartCount() {
        if (this.cartCount) {
            const count = this.cart.reduce((total, item) => total + item.quantity, 0);
            this.cartCount.textContent = count;
            this.cartCount.style.display = count > 0 ? 'block' : 'none';
        }
    }

    showToast(message) {
        const toast = document.createElement('div');
        toast.className = 'toast';
        toast.textContent = message;
        document.body.appendChild(toast);

        setTimeout(() => {
            toast.remove();
        }, 3000);
    }

    clearCart() {
        this.cart = [];
        this.saveCart();
        this.updateCartCount();
    }
}

// Initialize cart
document.addEventListener('DOMContentLoaded', () => {
    window.cart = new Cart();
}); 