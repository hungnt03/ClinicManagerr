FLOW KHÁM & ĐIỀU TRỊ
1️⃣ Khám ≠ Điều trị

Khám
→ tạo ĐỢT ĐIỀU TRỊ (DotDieuTri)

Điều trị
→ tạo BUỔI ĐIỀU TRỊ (BuoiDieuTri)

👉 Đây là nguyên tắc sống còn.
Không bao giờ gộp 2 cái này làm 1.
2️⃣ Gói điều trị gắn với ĐỢT, không gắn với BUỔI
Gói = số buổi + giá
Gói được chọn khi khám
Buổi chỉ là thực hiện, không biết gì về gói

II. FLOW CHUẨN – LẦN KHÁM ĐẦU TIÊN
🧩 NGOÀI ĐỜI

Bệnh nhân tới
⇩
Khám
⇩
Chẩn đoán
⇩
Tư vấn
⇩
Chọn gói
⇩
Thanh toán
⇩
Bắt đầu điều trị

🧠 TRONG HỆ THỐNG (CHUẨN 100%)
🔹 BƯỚC 1 – CHỌN / TẠO BỆNH NHÂN
BenhNhan/Index
Nếu bệnh nhân mới → tạo
Nếu cũ → chọn
📌 CHƯA có khám, CHƯA có buổi

🔹 BƯỚC 2 – KHÁM & TẠO ĐỢT ĐIỀU TRỊ
DotDieuTri/Create
LƯU CÁC THÔNG TIN SAU:
Bác sĩ khám
Ngày khám
Chẩn đoán
Phác đồ
Gói điều trị (1–5–10…)
Tổng số buổi
Tổng tiền
Đã thanh toán
Trạng thái = MoiTao
👉 Kết quả duy nhất của bước này:
1 record DotDieuTri
⛔ CHƯA có buổi điều trị

🔹 BƯỚC 3 – TẠO BUỔI ĐIỀU TRỊ ĐẦU TIÊN
BuoiDieuTri/Create?dotDieuTriId=...
TRƯỚC KHI CHO TẠO:
Check:
trangThai != HoanThanh
soBuoiDaDung < tongSoBuoi
KHI TẠO BUỔI:
Ngày điều trị
Bác sĩ điều trị tay
KTV tập
Nội dung điều trị
Vật tư / thuốc (nếu có)
chiPhiThuocVatTu = snapshot
SAU KHI LƯU:
soBuoiDaDung += 1
Nếu soBuoiDaDung == 1
→ trangThai = DangDieuTri

III. FLOW CÁC LẦN ĐIỀU TRỊ TIẾP THEO (TRONG GÓI)
🧩 NGOÀI ĐỜI
Bệnh nhân quay lại
Điều trị theo phác đồ
Ghi nhận buổi
Trừ buổi
🧠 TRONG HỆ THỐNG
🔹 BƯỚC 1 – CHỌN BỆNH NHÂN
BenhNhan/Index
🔹 BƯỚC 2 – XEM ĐỢT ĐIỀU TRỊ HIỆN TẠI
DotDieuTri/ChiTiet/{id}

HIỂN THỊ RÕ:
Chẩn đoán
Phác đồ
Gói
Tổng buổi / Đã dùng / Còn lại
Trạng thái
Cảnh báo sắp hết buổi

🔹 BƯỚC 3 – TẠO BUỔI ĐIỀU TRỊ
BuoiDieuTri/Create?dotDieuTriId=...
LOGIC BẮT BUỘC:
Nếu hết buổi → KHÔNG cho tạo
Nếu còn buổi → cho tạo
KHI TẠO:
Mỗi buổi = trừ đúng 1
Không cộng trừ tay

IV. FLOW MUA THÊM BUỔI (RẤT QUAN TRỌNG)
❗ CHỐT NGUYÊN TẮC
Mua thêm buổi KHÔNG tạo đợt mới
🔹 MÀN HÌNH
DotDieuTri/MuaThemBuoi/{id}
LÀM GÌ:
Nhập số buổi thêm
Nhập số tiền
Log lịch sử mua thêm
SAU KHI LƯU:
tongSoBuoi += soBuoiThem
Nếu đang HoanThanh → chuyển về DangDieuTri
V. KẾT THÚC ĐỢT ĐIỀU TRỊ
🔒 TỰ ĐỘNG
Khi:
soBuoiDaDung == tongSoBuoi
→ trangThai = HoanThanh
⛔ Không cho tạo thêm buổi
⛔ Không cho sửa buổi (trừ admin)






CHỐT LẠI NGHIỆP VỤ TĂNG CA (OT)
1️⃣ Giờ làm chuẩn (config được)

Từ CauHinhLuong:

Sáng: 08:00 – 12:00
Chiều: 13:30 – 17:30
→ Tổng 8 giờ/ngày


📌 Nghỉ trưa KHÔNG tính OT

2️⃣ Khi nào được tính tăng ca?
✅ Có OT nếu:

Đến sớm hơn giờ chuẩn ≥ 60 phút

Về muộn hơn giờ chuẩn ≥ 60 phút

❌ Không tính nếu:

Chỉ lệch < 60 phút

Đi làm nhưng đúng giờ

3️⃣ Cách làm tròn

Làm tròn 30 phút

Ví dụ:

1h10 → 1h

1h40 → 1.5h

2h20 → 2h

(config bằng SoPhutLamTronTangCa = 30)

4️⃣ Hệ số OT
Ngày	Hệ số
Ngày thường	125%
Chủ nhật	150%
Ngày lễ (được định nghĩa)	150%

(config bằng HeSoTangCaNgayThuong, HeSoTangCaNgayLe)

5️⃣ Công thức OT theo giờ
Lương/giờ = Lương CB / (Số ngày làm chuẩn * 8)

Tiền OT =
  Số giờ OT
× Lương/giờ
× Hệ số



TODO
DONE
Audit buổi điều trị

TODO
Sửa / xoá vật tư

Khoá buổi sau khi chốt

Giảm tồn kho khi thêm vật tư

Không cho thêm vượt tồn

Xóa / sửa vật tư trong buổi (audit)

vật tư
Nhập kho / xuất kho (phiếu)

Audit thay đổi tồn kho

Cảnh báo tồn kho thấp

Khoá vật tư theo kỳ kế toán

chức năng thanh toán cho đợt điều trị

1️⃣ Tạo DB + Entity cho cấu hình lương & ngày lễ
2️⃣ Màn hình quản lý cấu hình lương
3️⃣ Service tính lương theo tháng (core logic)
4️⃣ View bảng lương + chi tiết từng nhân viên
5️⃣ Chốt lương & khóa dữ liệu


❓ CÂU HỎI 1 — “25 ngày làm” là tính như thế nào?
ví dụ tháng đó có 30 ngày và có 5 chủ nhật tình được tính 25 ngày làm, số lượng ngày làm trong tháng có thể khác nhau tùy vào số ngày chủ nhật trong tháng, và số ngày trong tháng(28/30/31)
ngày làm  = có chấm công vào
không cần đủ giờ

❓ CÂU HỎI 2 — Nghỉ phép có lương / không lương xử lý thế nào?
nghỉ phép có lương là tính năng trong tương lai, hiện tại chỉ có nghỉ không lương. nghỉ làm đồng nghĩa với nghỉ không lương. Tuy nhiên vẫn có ngoại lệ cho các ngày lễ tết đặc biệt trong năm. Tôi muốn có chức năng quy định các ngày lễ đặc biệt trong năm.

❓ CÂU HỎI 3 — Xăng xe có bị trừ nếu nghỉ nhiều ngày?
xăng xe cố định 500k/tháng

❓ CÂU HỎI 4 — Hoa hồng tính theo “buổi” hay “bệnh nhân”?
hoa hồng tính theo số lượng bệnh nhân đã làm trong ngày. 2 buổi khác nhau tình tính 2 lần. 

❓ CÂU HỎI 5 — Giảm giá lấy từ đâu?
giảm giá này cần được thêm vào đợt điều trị, hiện tại đang thiếu

% giới thiệu là 5% tính theo tổng số tiền thanh toán trong đợt điều trị đó. số tiền này được cộng vào ai là do BenhNhans.nguoiGioiThieuId quyết định

ngoài ra tôi muốn bổ sung cách tính lương
lương = Lương CB + ăn trưa + xăng xe + % giới thiệu + % tập + % điều trị + chuyên cần + hoa hồng bán thuốc/vật tư + tăng ca

các loại hoa hồng tôi muốn có một màn hình define các thông số này
tăng ca được tính như sau: nếu đến sớm/về muộn quá 1 tiếng thì sẽ tính thời gian tăng ca đó x25% theo lương cơ bản theo tiếng. Làm tròn 30 phút

❓ 1. Giờ làm chuẩn 1 ngày là bao nhiêu?
áp dụng giờ hành chính 8 tiếng/ngày (sáng 8h-12h, chiều 13h30-17h30, nghỉ trưa 1.5 tiếng) và làm việc từ thứ Hai đến thứ bảy
tôi muốn giờ vào ra này cũng được define lại để sau nếu có thay đổi thì có thể đối ứng

❓ 2. Hoa hồng bán thuốc/vật tư:
hãy bỏ chức năng này

❓ 3. Chuyên cần:
Nghỉ 1 ngày là mất toàn bộ 200k → chính xác
Ngày lễ không đi làm không mất chuyên cần

❓ 4. Hoa hồng giới thiệu:
Chỉ tính khi đợt điều trị đã thanh toán đủ. nhận vào thời điểm tháng mà bệnh nhân thanh toán hết

👉 BƯỚC 3.1 (BẮT BUỘC)

Hoàn thiện TĂNG CA (OT) chi tiết

theo giờ chuẩn

theo CN / ngày lễ

làm tròn 30 phút

👉 BƯỚC 4

Controller + View bảng lương

Chi tiết từng khoản

Chốt lương & khóa dữ liệu

👉 Bạn chọn tiếp:

“Làm tăng ca chi tiết trước”

hay “Làm màn hình bảng lương trước”


VIII. UI FLOW ĐỀ XUẤT
A. Thu gói

Nằm trong: DotDieuTri/ChiTiet

Hiển thị:

Tổng tiền

Đã thu

Còn lại

Nút: Thu tiền

B. Thu thuốc

Nằm trong: BuoiDieuTri/Edit

Sau khi thêm vật tư

Nút: Thu tiền thuốc

👉 Nếu OK, bước tiếp theo ta nên làm:



👉 BƯỚC KẾ TIẾP BẮT BUỘC
🔴 HOÀN THIỆN TĂNG CA (OT)

Vì:

Tăng ca phụ thuộc:

chấm công

giờ làm chuẩn

ngày lễ

Nếu không làm trước:

UI bảng lương sẽ phải sửa lại

Sau đó mới tới:

1️⃣ Màn hình bảng lương
2️⃣ Chốt lương – khóa dữ liệu