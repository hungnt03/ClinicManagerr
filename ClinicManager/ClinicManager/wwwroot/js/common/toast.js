window.showToast = function (type, message, delay = 15000) {

    let bgClass = 'bg-primary';
    let title = 'Thong bao';

    switch (type) {
        case 'success':
            bgClass = 'bg-success';
            title = 'Thanh cong';
            break;
        case 'error':
            bgClass = 'bg-danger';
            title = 'That bai';
            break;
        case 'warning':
            bgClass = 'bg-warning text-dark';
            title = 'Canh bao';
            break;
        case 'info':
            bgClass = 'bg-info';
            title = 'Thong tin';
            break;
    }

    let toastId = 'toast-' + Date.now();

    let toastHtml = `
        <div id="${toastId}"
             class="toast ${bgClass} text-white"
             role="alert"
             aria-live="assertive"
             aria-atomic="true"
             data-bs-delay="${delay}">
            <div class="toast-header ${bgClass} text-white border-0">
                <strong class="me-auto">${title}</strong>
                <button type="button"
                        class="btn-close btn-close-white"
                        data-bs-dismiss="toast"></button>
            </div>
            <div class="toast-body">
                ${message}
            </div>
        </div>
    `;

    let container = document.getElementById('toast-container');
    container.insertAdjacentHTML('beforeend', toastHtml);

    let toastEl = document.getElementById(toastId);
    let toast = new bootstrap.Toast(toastEl);

    toast.show();

    toastEl.addEventListener('hidden.bs.toast', function () {
        toastEl.remove();
    });
};
