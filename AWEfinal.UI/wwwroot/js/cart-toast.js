(function () {
    const toastRoot = document.getElementById('aweToastRoot');
    const cartBadge = document.getElementById('js-cart-count');

    function showToast(message, isError) {
        if (!toastRoot) return;
        const toast = document.createElement('div');
        toast.className = `awe-toast${isError ? ' awe-toast--error' : ''}`;
        toast.textContent = message;
        toastRoot.appendChild(toast);
        setTimeout(() => toast.classList.add('is-hiding'), 3500);
        setTimeout(() => toast.remove(), 4000);
    }

    function updateCartCount(count) {
        if (!cartBadge) return;
        const value = Number(count) || 0;
        if (value <= 0) {
            cartBadge.classList.add('is-hidden');
            cartBadge.textContent = '0';
        } else {
            cartBadge.classList.remove('is-hidden');
            cartBadge.textContent = value;
        }
    }

    async function addToCart(productId, quantity) {
        const response = await fetch('/Cart/AddToCartAjax', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({ productId, quantity })
        });

        if (!response.ok) {
            throw new Error('Failed to add to cart');
        }

        return await response.json();
    }

    document.addEventListener('click', async (event) => {
        const trigger = event.target.closest('.js-add-to-cart');
        if (!trigger) return;

        event.preventDefault();
        if (trigger.dataset.loading === '1') return;

        const productId = parseInt(trigger.getAttribute('data-product-id') || '0', 10);
        const quantity = parseInt(trigger.getAttribute('data-quantity') || '1', 10) || 1;
        const fallbackUrl = trigger.getAttribute('href');

        if (!productId) {
            if (fallbackUrl) window.location.href = fallbackUrl;
            return;
        }

        trigger.dataset.loading = '1';
        trigger.classList.add('is-loading');

        try {
            const result = await addToCart(productId, quantity);
            if (result?.success) {
                showToast(result.message || 'Added to cart');
                updateCartCount(result.cartCount);
            } else {
                showToast(result?.message || 'Unable to add to cart', true);
                if (fallbackUrl) window.location.href = fallbackUrl;
            }
        } catch (err) {
            showToast('Unable to add to cart', true);
            if (fallbackUrl) window.location.href = fallbackUrl;
        } finally {
            delete trigger.dataset.loading;
            trigger.classList.remove('is-loading');
        }
    });
})();
