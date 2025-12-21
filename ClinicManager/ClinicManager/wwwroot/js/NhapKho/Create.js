let items = [];
let index = 0;

$(function () {

    // ===== SELECT2 SEARCH =====
    $('#vatTuId').select2({
        dropdownParent: $('#modalVatTu'),
        placeholder: 'Go ten vat tu...',
        minimumInputLength: 2,
        ajax: {
            url: '/VatTu/Search',
            dataType: 'json',
            delay: 300,
            data: p => ({ term: p.term }),
            processResults: data => ({ results: data })
        }
    });

    // ===== ADD ITEM =====
    $('#btnThem').click(function () {

        let vatTuId = parseInt($('#vatTuId').val());
        let vatTuText = $('#vatTuId option:selected').text();
        let soLuong = parseInt($('#soLuong').val());
        let donGia = parseFloat($('#donGiaNhap').val());

        if (!vatTuId || soLuong <= 0 || donGia <= 0) {
            alert('Du lieu khong hop le');
            return;
        }

        let thanhTien = soLuong * donGia;

        items.push({
            index,
            vatTuId,
            vatTuText,
            soLuong,
            donGia,
            thanhTien
        });

        renderTable();
        renderHidden();

        index++;
        $('#modalVatTu').modal('hide');
        $('#soLuong').val('');
        $('#donGiaNhap').val('');
        $('#vatTuId').val(null).trigger('change');
    });
});

// ===== RENDER TABLE =====
function renderTable() {
    let html = '';
    let tong = 0;

    items.forEach(i => {
        tong += i.thanhTien;

        html += `
        <tr>
            <td>${i.vatTuText}</td>
            <td>${i.soLuong}</td>
            <td>${i.donGia.toLocaleString()}</td>
            <td>${i.thanhTien.toLocaleString()}</td>
            <td>
                <button type="button"
                        class="btn btn-sm btn-danger"
                        onclick="removeItem(${i.index})">
                    Xoa
                </button>
            </td>
        </tr>`;
    });

    $('#tblVatTu tbody').html(html);
    $('#tongTien').text(tong.toLocaleString());
}

// ===== RENDER HIDDEN INPUT =====
function renderHidden() {
    let html = '';

    items.forEach((i, idx) => {
        html += `
        <input type="hidden" name="Items[${idx}].VatTuId" value="${i.vatTuId}" />
        <input type="hidden" name="Items[${idx}].SoLuong" value="${i.soLuong}" />
        <input type="hidden" name="Items[${idx}].DonGiaNhap" value="${i.donGia}" />`;
    });

    $('#itemsContainer').html(html);
}

// ===== REMOVE =====
function removeItem(idx) {
    items = items.filter(x => x.index !== idx);
    renderTable();
    renderHidden();
}
