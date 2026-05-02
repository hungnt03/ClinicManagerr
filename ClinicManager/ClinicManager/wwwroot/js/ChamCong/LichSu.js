$(document).ready(function () {

    var calendar;
    var $calendarEl = $('#calendar');
    var $nhanVienSelect = $('#nhanVienSelect');

    // Đọc tham số từ URL khi vừa load trang
    var urlParams = new URLSearchParams(window.location.search);
    var initialNhanVienId = urlParams.get('nhanVienId');
    var initialDate = urlParams.get('viewDate'); // Định dạng YYYY-MM-DD

    // =========================
    // LOAD DANH SACH NHAN VIEN (ADMIN)
    // =========================
    if ($nhanVienSelect.length) {
        $.getJSON('/ChamCong/DanhSachNhanVien', function (data) {
            $.each(data, function (i, nv) {
                $nhanVienSelect.append(
                    $('<option>', {
                        value: nv.id,
                        text: nv.text,
                        selected: nv.id == initialNhanVienId // Khôi phục nhân viên đã chọn
                    })
                );
            });

            initCalendar();
        });

        $nhanVienSelect.on('change', function () {
            updateUrl(); // Cập nhật URL khi đổi nhân viên
            if (calendar) calendar.refetchEvents();
        });
    } else {
        initCalendar();
    }

    // =========================
    // INIT FULLCALENDAR
    // =========================
    function initCalendar() {
        calendar = new FullCalendar.Calendar($calendarEl[0], {
            locale: 'vi',
            buttonText: {
                today: 'Hôm nay',
                month: 'Tháng',
                week: 'Tuần',
                day: 'Ngày'
            },
            initialView: 'dayGridMonth',
            // 2. Khôi phục ngày đã xem trước đó
            initialDate: initialDate || new Date(),
            height: 'auto',

            // Cập nhật URL mỗi khi người dùng bấm Next/Prev tháng
            datesSet: function () {
                updateUrl();
            },

            // ========================
            // CLICK VÀO EVENT (CÓ CHẤM CÔNG)
            // ========================
            eventClick: function (info) {
                // Trước khi đi, lưu lại URL hiện tại để nút "Back" của trình duyệt hoạt động đúng
                updateUrl();
                window.location.href =
                    '/ChamCong/ChiTiet/' + info.event.id;
            },

            // ========================
            // CLICK VÀO NGÀY (KỂ CẢ CHƯA CÓ EVENT)
            // ========================
            dateClick: function (info) {

                var ngay = info.dateStr;
                var nhanVienId = null;

                if ($('#nhanVienSelect').length) {
                    nhanVienId = $('#nhanVienSelect').val();
                }

                var url = '/ChamCong/ChiTietTheoNgay?ngay=' + ngay;

                if (nhanVienId) {
                    url += '&nhanVienId=' + nhanVienId;
                }

                window.location.href = url;
            },

            events: function (info, successCallback, failureCallback) {
                // Dùng info.start và cộng thêm vài ngày để chắc chắn nằm trong tháng đang xem
                // Vì info.start có thể là ngày của tháng trước (hiển thị ở đầu lưới)
                var midDate = new Date(info.start);
                midDate.setDate(midDate.getDate() + 10);
                var month = midDate.getMonth() + 1;
                var year = midDate.getFullYear();

                var url = '/ChamCong/CalendarData' + '?month=' + month + '&year=' + year;

                if ($nhanVienSelect.length) {
                    url += '&nhanVienId=' + $nhanVienSelect.val();
                }

                $.ajax({
                    url: url,
                    type: 'GET',
                    success: function (data) {
                        successCallback(data);
                    },
                    error: function () {
                        failureCallback();
                    }
                });
            },

            eventContent: function (arg) {
                var smallScreen = $(window).width() < 576;

                if (smallScreen) {
                    return {
                        html: '<div>' + (arg.event.title || '') + '</div>'
                    };
                }

                var gioVao = arg.event.extendedProps.gioVao || '';
                var gioRa = arg.event.extendedProps.gioRa || '';

                return {
                    html: '<div>' + gioVao + ' - ' + gioRa + '</div>'
                };
            },

            eventDidMount: function (info) {
                var props = info.event.extendedProps;
                var content = `
                    Vào: ${props.gioVao} 
                    Ra: ${props.gioRa}
                    Ăn trưa: ${props.anTrua}
                `;

                // Khởi tạo tooltip theo cách của Bootstrap 5 (không phụ thuộc jQuery hoàn toàn)
                new bootstrap.Tooltip(info.el, {
                    title: content,
                    placement: 'top',
                    trigger: 'hover',
                    container: 'body',
                    html: true
                });
            },
        });

        calendar.render();
    }

    // Hàm cập nhật URL mà không reload trang
    function updateUrl() {
        if (!calendar) return;

        var viewDate = calendar.getDate();

        // Trích xuất Năm, Tháng, Ngày theo giờ địa phương
        var year = viewDate.getFullYear();
        var month = (viewDate.getMonth() + 1).toString().padStart(2, '0');
        var day = viewDate.getDate().toString().padStart(2, '0');
        // Tạo chuỗi YYYY-MM-DD thủ công để không bị lệch múi giờ
        var localDate = year + '-' + month + '-' + day;

        var nvId = $nhanVienSelect.val() || "";

        var newUrl = window.location.pathname + "?nhanVienId=" + nvId + "&viewDate=" + localDate;

        // Thay đổi URL trên thanh địa chỉ mà không làm tải lại trang
        window.history.replaceState({ path: newUrl }, '', newUrl);
    }
});
