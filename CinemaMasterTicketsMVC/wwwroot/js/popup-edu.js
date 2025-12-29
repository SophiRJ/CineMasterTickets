// Versión para que aparezca SIEMPRE en todas las páginas y se cierre solo
document.addEventListener("DOMContentLoaded", () => {
    const toastElement = document.getElementById('eduToast');
    if (toastElement) {
        const toast = new bootstrap.Toast(toastElement, {
            autohide: true,
            delay: 4000
        });
        toast.show();
    }
});