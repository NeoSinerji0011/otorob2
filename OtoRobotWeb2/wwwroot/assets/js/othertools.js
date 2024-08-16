function fnk_DataTable_Init() {

    if (document.querySelectorAll('#table_offer thead tr').length < 2) {
        $('#table_offer thead tr')
            .clone(true)
            .addClass('filters')
            .appendTo('#table_offer thead');
    }

    $('#table_offer').dataTable({
        "dom": 'Bflrtip',
        "lengthMenu": [[-1, 100, 50, 25], ["All", 100, 50, 25]],
        "pageLength": -1,
        columns: [
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null
        ],
        buttons: [
            {
                extend: 'excelHtml5',
                text: 'Export to Excel',
                title: 'Otorobot - Parca Sorgu Listesi',
                alignment: "center",
                autoFilter: false,
                //filename: 'td900',
                exportOptions: {
                    columns: [0, 1, 2, 3, 4, 5, 6, 7, 8]
                },

            },
            {
                extend: 'pdfHtml5',
                alignment: "center",
                pageSize: 'A4', //formato stampa
                title: 'Otorobot - Parca Sorgu Listesi',

            }
        ],
        initComplete: function () {
            var api = this.api();
          
            // For each column
            api
                .columns()
                .eq(0)
                .each(function (colIdx) {
                    // Set the header cell to contain the input element
                    var cell = $('.filters th').eq(
                        $(api.column(colIdx).header()).index()
                    );
                    var title = $(cell).text();
                    console.log(title)
                    $(cell).html('<input type="text" placeholder="' + title + '" />');
                    //if ($(api.column(colIdx).header()).index() >= 0) {
                    //    $(cell).html('<input type="text" placeholder="' + title + '"/>');
                    //}

                    // On every keypress in this input
                    $(
                        'input',
                        $('.filters th').eq($(api.column(colIdx).header()).index())
                    )
                        .off('keyup change')
                        .on('change', function (e) {
                            // Get the search value
                            $(this).attr('title', $(this).val());
                            var regexr = '({search})'; //$(this).parents('th').find('select').val();

                            var cursorPosition = this.selectionStart;
                            // Search the column for that value
                            api
                                .column(colIdx)
                                .search(
                                    this.value != ''
                                        ? regexr.replace('{search}', '(((' + this.value + ')))')
                                        : '',
                                    this.value != '',
                                    this.value == ''
                                )
                                .draw();
                        })
                        .on('keyup', function (e) {
                            e.stopPropagation();

                            $(this).trigger('change');
                            $(this)
                                .focus()[0]
                                .setSelectionRange(cursorPosition, cursorPosition);
                        });
                });
        },

    });
    $(".buttons-excel").html('<img src="/assets/images/excel.png" width="25" height="25">')
    $(".buttons-pdf").html('<img src="/assets/images/pdf.png" width="25" height="25">')
     
}

function fnk_DataTable_Destroy() {
    $('#table_offer').dataTable().fnDestroy();
}