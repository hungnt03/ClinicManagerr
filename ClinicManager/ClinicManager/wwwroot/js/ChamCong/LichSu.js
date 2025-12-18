$(document).ready(function () {

    var calendar;
    var $calendarEl = $('#calendar');
    var $nhanVienSelect = $('#nhanVienSelect');

    // =========================
    // LOAD DANH SACH NHAN VIEN (ADMIN)
    // =========================
    if ($nhanVienSelect.length) {
        $.getJSON('/ChamCong/DanhSachNhanVien', function (data) {
            $.each(data, function (i, nv) {
                $nhanVienSelect.append(
                    $('<option>', {
                        value: nv.id,
                        text: nv.text
                    })
                );
            });

            initCalendar();
        });

        $nhanVienSelect.on('change', function () {
            if (calendar) {
                calendar.refetchEvents();
            }
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
            height: 'auto',

            // ========================
            // CLICK VÀO EVENT (CÓ CHẤM CÔNG)
            // ========================
            eventClick: function (info) {
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

                var url = '/ChamCong/CalendarData' +
                    '?month=' + (info.start.getMonth() + 1) +
                    '&year=' + info.start.getFullYear();

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
            }
        });

        calendar.render();
    }
});
