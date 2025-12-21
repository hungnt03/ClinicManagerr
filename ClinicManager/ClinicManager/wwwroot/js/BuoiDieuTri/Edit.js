$(function () {
    $('#btnThuTienThuoc').click(function () {
        let buoiId = $(this).data('buoiid');

        $.get('/ThanhToan/ThuTienThuoc', { buoiDieuTriId: buoiId }, function (html) {
            $('#modalThuocContainer').html(html);
            $('#modalThuTienThuoc').modal('show');
        });
    });

    $(document).on('submit', '#formThuTienThuoc', function (e) {
        e.preventDefault();

        $.ajax({
            url: '/ThanhToan/ThuTienThuoc',
            method: 'POST',
            data: $(this).serialize(),
            success: function (res) {
                showToast('success', 'Thu tien thuoc/vat tu thanh cong');
                // mở phiếu thu
                window.open('/ThanhToan/PhieuThu?id=' + res.thanhToanId, '_blank');
                setTimeout(() => location.reload(), 1000);
            },
            error: function (xhr) {
                showToast('error', xhr.responseText || 'Thu tien that bai');
            }
        });
    });
});