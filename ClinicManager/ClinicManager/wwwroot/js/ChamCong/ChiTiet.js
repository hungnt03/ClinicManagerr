// ===============================
// ChamCong - ChiTiet.js
// ===============================

$(function () {

    // ===============================
    // EVENT: MO MODAL SUA CHAM CONG
    // ===============================
    $('#btnEditChamCong').on('click', function () {

        var chamCongId = $(this).data('id');

        $.get('/ChamCong/GetChamCongForEdit', { id: chamCongId })
            .done(function (res) {

                // set hidden id
                $('#ChamCongId').val(res.chamCongId);

                // set gio vao / ra
                if (res.thoiGianVao) {
                    $('#ThoiGianVao').val(formatDateTimeLocal(res.thoiGianVao));
                } else {
                    $('#ThoiGianVao').val('');
                }

                if (res.thoiGianRa) {
                    $('#ThoiGianRa').val(formatDateTimeLocal(res.thoiGianRa));
                } else {
                    $('#ThoiGianRa').val('');
                }

                // set nghi phep
                $('#NghiPhep').prop('checked', res.nghiPhep);
                $('#NghiPhepCoLuong').prop('checked', res.nghiPhepCoLuong);

                // ⭐ BẮT BUỘC: đồng bộ lại UI
                toggleNghiPhep();

                // show modal
                $('#editChamCongModal').modal('show');
            })
            .fail(function () {
                alert('Khong tai duoc du lieu cham cong');
            });
    });

    // ===============================
    // EVENT: TOGGLE NGHI PHEP
    // ===============================
    $('#NghiPhep').on('change', function () {
        toggleNghiPhep();
    });

    // ===============================
    // EVENT: SAVE CHAM CONG
    // ===============================
    $('#btnSaveChamCong').on('click', function () {

        var data = collectFormData();

        // ===== VALIDATE CLIENT =====
        if (!data.lyDo || data.lyDo.trim() === '') {
            alert('Bat buoc nhap ly do sua');
            return;
        }

        // nghi phep ma co gio -> sai
        if (data.nghiPhep &&
            (data.thoiGianVao || data.thoiGianRa)) {
            alert('Nghi phep khong duoc co gio vao / gio ra');
            return;
        }

        // di lam ma khong co gio vao -> sai
        if (!data.nghiPhep && !data.thoiGianVao) {
            alert('Di lam phai co gio vao');
            return;
        }

        // ===== SUBMIT =====
        $.ajax({
            url: '/ChamCong/UpdateChamCong',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function () {
                location.reload();
            },
            error: function (xhr) {
                alert(xhr.responseText || 'Loi khi luu cham cong');
            }
        });
    });
});


// ===============================
// HAM DONG BO UI NGHI PHEP
// ===============================
function toggleNghiPhep() {

    var nghiPhep = $('#NghiPhep').is(':checked');

    if (nghiPhep) {
        // nghi phep -> xoa & khoa gio
        $('#ThoiGianVao').val('').prop('disabled', true);
        $('#ThoiGianRa').val('').prop('disabled', true);

        // cho phep tick co luong
        $('#NghiPhepCoLuong').prop('disabled', false);
    } else {
        // di lam -> mo gio
        $('#ThoiGianVao').prop('disabled', false);
        $('#ThoiGianRa').prop('disabled', false);

        // khong nghi phep -> khong co luong
        $('#NghiPhepCoLuong')
            .prop('checked', false)
            .prop('disabled', true);
    }
}


// ===============================
// HAM THU THAP DATA FORM
// ===============================
function collectFormData() {
    return {
        chamCongId: $('#ChamCongId').val(),
        thoiGianVao: $('#ThoiGianVao').val() || null,
        thoiGianRa: $('#ThoiGianRa').val() || null,
        nghiPhep: $('#NghiPhep').is(':checked'),
        nghiPhepCoLuong: $('#NghiPhepCoLuong').is(':checked'),
        lyDo: $('#LyDo').val()
    };
}


// ===============================
// FORMAT DATETIME -> input[type=datetime-local]
// ===============================
function formatDateTimeLocal(value) {
    // value có thể là ISO string hoặc /Date(...)/
    var d = new Date(value);

    var pad = function (n) {
        return n < 10 ? '0' + n : n;
    };

    return d.getFullYear() + '-' +
        pad(d.getMonth() + 1) + '-' +
        pad(d.getDate()) + 'T' +
        pad(d.getHours()) + ':' +
        pad(d.getMinutes());
}
