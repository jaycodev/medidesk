
//MANEJO DE CARGA DE LA IMAGEN DE PERFIL
// Este script maneja la carga de la imagen de perfil del usuario en el modal de edición de perfil.
// Asegurate de que SweetAlert2 y Bootstrap estén correctamente importados en tu proyecto.

//redirectAfterUpload es la URL a la que se redirigirá al usuario después de subir la imagen.
//editProfileUrl es la URL del endpoint que maneja la subida de la imagen de perfil.

document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector('#editProfilePictureModal form');

    form.addEventListener("submit", async function (e) {
        e.preventDefault();

        const formData = new FormData(form);

        Swal.fire({
            title: 'Subiendo imagen...',
            text: 'Por favor espera.',
            customClass: {
                popup: 'bg-body text-body-emphasis rounded-4'
            },
            allowOutsideClick: false,
            allowEscapeKey: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });

        try {
            const response = await fetch(editProfileUrl, {
                method: 'POST',
                body: formData
            });

            const result = await response.json();

            if (result.success) {
                Swal.fire({
                    icon: 'success',
                    title: 'Éxito',
                    text: result.message,
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
                }).then(() => {
                    // Redirige después de cerrar el mensaje de éxito
                    window.location.href = redirectAfterUpload ;
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: result.message
                });
            }

        } catch (err) {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: result.message,
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
});

//MANEJO DE SELECCIÓN DE AVATAR
// Este script maneja la selección de un avatar por parte del usuario.
function selectAvatar(imgElement) {
    // Quitar clase seleccionada de todos los avatares
    document.querySelectorAll(".avatar-img").forEach(el => {
        el.classList.remove("avatar-selected");
    });

    // Agregar clase a la imagen seleccionada
    imgElement.classList.add("avatar-selected");

    // Seleccionar el radio button asociado
    const radio = imgElement.previousElementSibling;
    if (radio) {
        radio.checked = true;
    }

    // Enviar el formulario automáticamente
    document.getElementById("avatarForm").submit();
}