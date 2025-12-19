$(function () {

    loadVatTu();

    $('#btnThemVatTu').on('click', function () {

        var data = {
            buoiDieuTriId: $('#buoiDieuTriId').val(),
            vatTuId: $('#vatTuId').val(),
            soLuong: $('#soLuong').val()
        };

        if (!data.vatTuId || data.soLuong <= 0) {
            alert('Du lieu khong hop le');
            return;
        }

        $.ajax({
            url: '/BuoiDieuTri/ThemVatTu',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function () {
                $('#modalVatTu').modal('hide');
                loadDanhSachVatTuDaThem();
            },
            error: function (xhr) {
                alert(xhr.responseText);
            }
        });
    });
});

// ===============================
// LOAD VAT TU
// ===============================
function loadVatTu() {
    $.get('/VatTu/GetAll', function (data) {
        var html = '';
        data.forEach(function (v) {
            html += `<option value="${v.vatTuId}">
                        ${v.tenVatTu} (${v.tonKho})
                     </option>`;
        });
        $('#vatTuId').html(html);
    });
}

// ===============================
// LOAD DS VAT TU DA THEM
// ===============================
function loadDanhSachVatTuDaThem() {
    var buoiId = $('#buoiDieuTriId').val();
    $('#vatTuContainer').load('/BuoiDieuTri/DanhSachVatTu?buoiDieuTriId=' + buoiId);
}
