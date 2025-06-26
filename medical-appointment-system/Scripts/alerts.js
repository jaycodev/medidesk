window.addEventListener('DOMContentLoaded', () => {
    const successMessage = document.body.dataset.success;
    const errorMessage = document.body.dataset.error;

    if (successMessage) {
        Swal.fire({
            icon: 'success',
            title: 'Éxito',
            text: successMessage,
            confirmButtonText: 'Aceptar',
            customClass: {
                popup: 'bg-body text-body-emphasis rounded-4',
                confirmButton: 'btn btn-success'
            },
            buttonsStyling: false,
            focusConfirm: false,
            didOpen: () => {
                const confirmBtn = document.querySelector('.swal2-confirm');
                if (confirmBtn) confirmBtn.blur();
            }
        });
    }

    if (errorMessage) {
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: errorMessage,
            confirmButtonText: 'Aceptar',
            customClass: {
                popup: 'bg-body text-body-emphasis rounded-4',
                confirmButton: 'btn btn-danger'
            },
            buttonsStyling: false,
            focusConfirm: false,
            didOpen: () => {
                const confirmBtn = document.querySelector('.swal2-confirm');
                if (confirmBtn) confirmBtn.blur();
            }
        });
    }
});
