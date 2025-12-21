$(function () {

    if ($('#buoiDieuTriId').length) {
        loadVatTu();
        loadDanhSachVatTu();
    }

    // ===== INIT SELECT2 KHI MỞ MODAL =====
    $('#modalVatTu').on('shown.bs.modal', function () {

        if (!$('#vatTuId').hasClass("select2-hidden-accessible")) {

            $('#vatTuId').select2({
                dropdownParent: $('#modalVatTu'),
                placeholder: 'Go ten vat tu...',
                allowClear: true,
                minimumInputLength: 2,
                ajax: {
                    url: '/VatTu/Search',
                    dataType: 'json',
                    delay: 300,
                    data: function (params) {
                        return {
                            term: params.term
                        };
                    },
                    processResults: function (data) {
                        return {
                            results: data
                        };
                    }
                }
            });
        }
    });

    //$('#modalVatTu').on('hidden.bs.modal', function () {
    //    $('#btnMoVatTu').focus();
    //});

    // ===== THEM VAT TU =====
    $('#btnThemVatTu').click(function () {

        let data = {
            buoiDieuTriId: parseInt($('#buoiDieuTriId').val()),
            vatTuId: parseInt($('#vatTuId').val()),
            soLuong: parseInt($('#soLuong').val())
        };

        if (isNaN(data.vatTuId) || data.soLuong <= 0) {
            alert('Vui long chon vat tu va so luong hop le');
            return;
        }

        $.ajax({
            url: '/BuoiDieuTri/ThemVatTu',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function () {
                $('#modalVatTu').modal('hide');
                loadDanhSachVatTu();
            },
            error: function (xhr) {
                alert(xhr.responseText);
            }
        });
    });
});

function loadVatTu() {
    $.get('/VatTu/GetAll', function (data) {
        let html = '';
        data.forEach(v => {
            html += `<option value="${v.vatTuId}">
                        ${v.tenVatTu} (${v.tonKho})
                     </option>`;
        });
        $('#vatTuId').html(html);
    });
}

function loadDanhSachVatTu() {
    let id = $('#buoiDieuTriId').val();
    $('#vatTuContainer').load('/BuoiDieuTri/DanhSachVatTu?buoiDieuTriId=' + id);
}
