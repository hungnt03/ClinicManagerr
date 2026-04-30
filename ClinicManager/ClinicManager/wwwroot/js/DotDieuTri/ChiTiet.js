
$(function () {
    $('#btnThuTien').click(function () {
        let dotId = $(this).data('dotid');

        $.get('/ThanhToan/ThuTienGoi', { dotDieuTriId: dotId }, function (html) {
            $('#modalContainer').html(html);
            $('#modalThuTien').modal('show');
        });
    });

    $('#btnPhieuKham').click(function () {
        let dotId = $(this).data('dotid');

        $.get('/ThanhToan/PhieuKham', { dotDieuTriId: dotId }, function (html) {
            $('#modalContainer').html(html);
            $('#modalPhieuKham').modal('show');
        });
    });

    $(document).on('submit', '#formThuTien', function (e) {
        e.preventDefault();

        $.ajax({
            url: '/ThanhToan/ThuTienGoi',
            method: 'POST',
            data: $(this).serialize(),
            success: function (res) {
                showToast('success', 'Thu tien goi dieu tri thanh cong');
                // mở phiếu thu
                window.open('/ThanhToan/PhieuThu?id=' + res.thanhToanId, '_blank');
                setTimeout(() => location.reload(), 1000);
            },
            error: function (xhr) {
                showToast('error', xhr.responseText || 'Thu tien that bai');
            }
        });
    });

    $('#btnPhieuKham').click(function () {
        let dotId = $('#btnPhieuKham').data('dotid');
        window.open('/ThanhToan/PhieuKham?id=' + dotId, '_blank');
        setTimeout(() => location.reload(), 1000);
    });
});