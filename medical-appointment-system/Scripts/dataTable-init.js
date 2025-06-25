function toggleTableLoadingState(action, selector) {
    const $spinner = $('#spinnerLoad');
    const $allButtons = $('#buttonGroupHeader button');
    const $exportButtons = $('[data-export]');

    if (action === 'loading') {
        $spinner.removeClass('d-none');
        $allButtons.prop('disabled', true);
    } else if (action === 'loaded') {
        $spinner.addClass('d-none');
        $allButtons.prop('disabled', false);

        const table = $(selector).DataTable();
        const hasRows = table.rows().count() > 0;
        $exportButtons.prop('disabled', !hasRows);
    }
}

function initCustomDataTable(selector) {
    const table = $(selector);

    table.DataTable({
        responsive: true,
        lengthMenu: [
            [10, 15, 25, -1],
            [10, 15, 25, 'Todo'],
        ],
        columnDefs: [{ orderable: false, targets: -1 }],
        language: {
            search: '',
            searchPlaceholder: 'Buscar...',
            lengthMenu: 'Mostrar _MENU_ ',
            info: 'Mostrando _START_ a _END_ de _TOTAL_ registros',
            infoEmpty: 'Mostrando 0 a 0 de 0 registros',
            infoFiltered: '(filtrado de _MAX_ registros totales)',
            zeroRecords: 'No se encontraron registros coincidentes',
            emptyTable: 'No hay datos disponibles',
        },
        initComplete: function () {
            toggleTableLoadingState('loaded', selector)
            $('#tableContainer').removeClass('d-none')

            table.wrap('<div class="rounded overflow-hidden border mt-2 mb-2 p-0"></div>');

            const $dtSearch = $('.dt-search');
            $dtSearch.find('label').remove();
            $dtSearch.addClass('input-group');
            $dtSearch.prepend(
                '<span class="input-group-text bg-body p-0 ps-2 d-flex align-items-center justify-content-center">' +
                '<i class="fa-solid fa-magnifying-glass text-muted"></i>' +
                '</span>'
            );
        }
    });

    setTimeout(function () {
        if ($('#spinnerLoad').is(':visible')) {
            toggleTableLoadingState('loaded', selector)
            $('#tableContainer').removeClass('d-none')
        }
    }, 3000)
}
