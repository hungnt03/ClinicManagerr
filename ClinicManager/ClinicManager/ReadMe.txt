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