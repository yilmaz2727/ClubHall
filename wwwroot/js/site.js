// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function showJoinToast(message, color) {
    const toastEl = document.getElementById('joinToast');
    const toastBody = document.getElementById('toastBody');

    toastBody.innerText = message;

    // Soft arka plan rengi
    toastEl.style.background = color;
    toastEl.style.color = "white";

    const toast = new bootstrap.Toast(toastEl);
    toast.show();
}
