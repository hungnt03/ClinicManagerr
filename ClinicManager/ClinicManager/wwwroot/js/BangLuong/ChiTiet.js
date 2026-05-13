$(document).ready(function () {
    // Sự kiện khi bấm nút "Thêm phát sinh"
    $('.btn-them-phat-sinh').on('click', function () {
        // Lấy dữ liệu từ thuộc tính data- của nút bấm
        const nhanVienId = $(this).data('id');
        const tenNhanVien = $(this).data('ten');

        // Đổ dữ liệu vào Modal
        const $modal = $('#modalPhatSinh');
        $modal.find('#inputNhanVienId').val(nhanVienId);
        $modal.find('#spanTenNV').text(tenNhanVien);

        // Reset form để tránh lưu dữ liệu cũ của lần nhập trước
        $modal.find('form')[0].reset();

        // Đảm bảo ID nhân viên vẫn được giữ sau khi reset form
        $modal.find('#inputNhanVienId').val(nhanVienId);

        // Hiển thị Modal (Sử dụng Bootstrap 5 Native hoặc jQuery tùy bạn)
        var myModal = new bootstrap.Modal(document.getElementById('modalPhatSinh'));
        myModal.show();
    });
});