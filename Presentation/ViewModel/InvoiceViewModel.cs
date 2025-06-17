using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Helpers;
using QuanLyTiecCuoi.Model;
using QuanLyTiecCuoi.Presentation.View;
using QuanLyTiecCuoi.Presentation.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas.Draw;
using System.Windows.Controls;
using System.ComponentModel.DataAnnotations;

namespace QuanLyTiecCuoi.ViewModel {
    public class InvoiceViewModel : BaseViewModel {
        private  IPhieuDatTiecService _phieuDatTiecService;
        private readonly IDichVuService _dichVuService;
        private readonly ICaService _caService;
        private readonly ISanhService _sanhService;
        private readonly IChiTietDVService _chiTietDichVuService;

        private PHIEUDATTIECDTO _SelectedInvoice;
        public PHIEUDATTIECDTO SelectedInvoice { get => _SelectedInvoice; set { _SelectedInvoice = value; OnPropertyChanged(); } }
        private int _InvoiceId;
        public int InvoiceId { get => _InvoiceId; set { _InvoiceId = value; OnPropertyChanged(); } }

        private decimal? _TotalTableAmount;
        public decimal? TotalTableAmount { get => _TotalTableAmount; set { _TotalTableAmount = value; OnPropertyChanged(); } }
        private DateTime _PaymentDate = DateTime.Now;
        public DateTime PaymentDate { get => _PaymentDate; set { _PaymentDate = value; OnPropertyChanged(); } }

        private string _TableQuantity;
        public string TableQuantity { get => _TableQuantity; set { _TableQuantity = value; OnPropertyChanged(); } }
        private decimal _RemainingAmount;
        public decimal RemainingAmount { get => _RemainingAmount; set { _RemainingAmount = value; OnPropertyChanged(); } }
        private string _TableQuantityMessage = "Số lượng bàn đã đặt trước là ";
        public string TableQuantityMessage { get => _TableQuantityMessage; set { _TableQuantityMessage = value; OnPropertyChanged(); } }
        private decimal? _DamageEquipmentCost;
        public decimal? DamageEquipmentCost { get => _DamageEquipmentCost; set { _DamageEquipmentCost = value; OnPropertyChanged(); } }
        private decimal? _Deposit;
        public decimal? Deposit { get => _Deposit; set { _Deposit = value; OnPropertyChanged(); } }
        private decimal? _Fine;
        public decimal? Fine { get => _Fine; set { _Fine = value; OnPropertyChanged(); } }
        private decimal? _TotalInvoiceAmount;
        public decimal? TotalInvoiceAmount { get => _TotalInvoiceAmount; set { _TotalInvoiceAmount = value; OnPropertyChanged(); } }

        public ObservableCollection<CHITIETDVDTO> ServiceList { get; set; } = new ObservableCollection<CHITIETDVDTO>();
        private bool CanExport = false;

        // Command properties
        public ICommand ExportCommand { get; set; }
        public ICommand ConfirmPaymentCommand { get; set; }
        private string _ConfirmMessage;
        public string ConfirmMessage { get => _ConfirmMessage; set { _ConfirmMessage = value; OnPropertyChanged(); } }

        public InvoiceViewModel(int invoiceId) {
            InvoiceId = invoiceId;
            _phieuDatTiecService = new PhieuDatTiecService();
            _caService = new CaService();
            _sanhService = new SanhService();
            _chiTietDichVuService = new ChiTietDVService();
            SelectedInvoice = _phieuDatTiecService.GetById(invoiceId);
            ServiceList = new ObservableCollection<CHITIETDVDTO>(_chiTietDichVuService.GetByPhieuDat(invoiceId));
            TableQuantityMessage += $"{SelectedInvoice.SoLuongBan}";

            ExportCommand = new RelayCommand<Window>((p) => { return CanExport; }, (p) => {
                try {
                    if (SelectedInvoice == null) {
                        return;
                    }
                    var dialog = new Microsoft.Win32.SaveFileDialog {
                        FileName = $"HoaDon_{SelectedInvoice.MaPhieuDat}",
                        DefaultExt = ".pdf",
                        Filter = "PDF documents (.pdf)|*.pdf"
                    };
                    bool? result = dialog.ShowDialog();
                    if (result == true) {
                        string filePath = dialog.FileName;
                        ExportInvoice(SelectedInvoice, filePath);
                        MessageBox.Show("Xuất hóa đơn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show($"Lỗi khi xuất hóa đơn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
            ConfirmPaymentCommand = new RelayCommand<Window>((p) => {
                if (SelectedInvoice == null) {
                    return false;
                }
                if (!int.TryParse(TableQuantity, out int _tableQuantiy)) {
                    CanExport = false;
                    ConfirmMessage = "Nhập số bàn là số nguyên";
                    return false;
                }
                if (int.Parse(TableQuantity) < SelectedInvoice.SoLuongBan) {
                    CanExport = false;
                    ConfirmMessage = "Số bàn đã dùng không được nhỏ hơn số bàn đã đặt";
                    return false;
                }
                if(DamageEquipmentCost == null) {
                    CanExport = false;
                    ConfirmMessage = "Nhập chi phí thiết bị hỏng hóc";
                    return false;
                }
                ConfirmMessage = string.Empty;
                return true;
            }, (p) => {
                ConfirmPayment();
            });
        }
        private void ConfirmPayment() {
            int tableQuantity = 0;
            decimal totalTableAmount = 0;
            decimal damageEquipmentCost = 0;
            if (int.TryParse(TableQuantity, out int _tableQuantiy)) {
                tableQuantity = _tableQuantiy;
                totalTableAmount = tableQuantity * SelectedInvoice.DonGiaBanTiec.GetValueOrDefault();
            }
            damageEquipmentCost = DamageEquipmentCost.GetValueOrDefault();
            try {
                var ca = _caService.GetById(SelectedInvoice.MaCa.GetValueOrDefault());
                var sanh = _sanhService.GetById(SelectedInvoice.MaSanh.GetValueOrDefault());
                PHIEUDATTIECDTO invoice = new PHIEUDATTIECDTO {
                    MaPhieuDat = SelectedInvoice.MaPhieuDat,
                    TenChuRe = SelectedInvoice.TenChuRe,
                    TenCoDau = SelectedInvoice.TenCoDau,
                    DienThoai = SelectedInvoice.DienThoai,
                    NgayDaiTiec = SelectedInvoice.NgayDaiTiec.Value,
                    NgayDatTiec = SelectedInvoice.NgayDatTiec,
                    Ca = ca,
                    Sanh = sanh,
                    TienDatCoc = SelectedInvoice.TienDatCoc,
                    SoBanDuTru = SelectedInvoice.SoBanDuTru,
                    MaCa = SelectedInvoice.MaCa,
                    MaSanh = SelectedInvoice.MaSanh,
                    DonGiaBanTiec = SelectedInvoice.DonGiaBanTiec,
                    TongTienDV = SelectedInvoice.TongTienDV,
                    NgayThanhToan = DateTime.Now,
                    SoLuongBan = tableQuantity,
                    ChiPhiPhatSinh = damageEquipmentCost,
                    TongTienBan = totalTableAmount
                };
                _phieuDatTiecService.Update(invoice);
                _phieuDatTiecService = new PhieuDatTiecService();
                invoice = _phieuDatTiecService.GetAll().LastOrDefault();
                Deposit = invoice.TienDatCoc.GetValueOrDefault();
                TotalInvoiceAmount = invoice.TongTienHoaDon.GetValueOrDefault();
                Fine = invoice.TienPhat.GetValueOrDefault();
                RemainingAmount = invoice.TienConLai.GetValueOrDefault();
                OnPropertyChanged();
                CanExport = true;
            }
            catch (Exception e) {
                MessageBox.Show(e.Message);
            }
        }
        private void ExportInvoice(PHIEUDATTIECDTO bill, string outputPath) {
            var regularFont = PDFFont.RegularFont;
            var boldFont = PDFFont.BoldFont;
            var italicFont = PDFFont.ItalicFont;
            var textAlignmentR = iText.Layout.Properties.TextAlignment.RIGHT;
            var textAlignmentC = iText.Layout.Properties.TextAlignment.CENTER;

            var writer = new PdfWriter(outputPath);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf, iText.Kernel.Geom.PageSize.A4);
            document.SetMargins(40, 30, 40, 30);

            /* Logo (nếu có)
            var logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png");
            if (File.Exists(logoPath)) {
                var logo = new Image(ImageDataFactory.Create(logoPath)).ScaleToFit(80, 80).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                document.Add(logo);
            }*/

            // Tiêu đề
            var header = new Paragraph("HÓA ĐƠN THANH TOÁN TIỆC CƯỚI")
                .SetFontSize(20)
                .SetFont(PDFFont.BoldFont)
                .SetTextAlignment(textAlignmentC)
                .SetMarginBottom(10);
            document.Add(header);

            // Dòng kẻ
            document.Add(new LineSeparator(new SolidLine(1f)).SetMarginBottom(15));

            // Thông tin tiệc cưới
            document.Add(PdfExportHelper.CreateInfoTable(new[] {
                ("Tên chú rể:", bill.TenChuRe ?? ""),
                ("Tên cô dâu:", bill.TenCoDau ?? ""),
                ("Ngày đãi tiệc:", bill.NgayDaiTiec?.ToString("dd'/'MM'/'yyyy") ?? ""),
                ("Số lượng bàn:", bill.SoLuongBan?.ToString() ?? ""),
                ("Ca:", _caService.GetById(bill.MaCa ?? 1).TenCa),
                ("Sảnh:", _sanhService.GetById(bill.MaSanh ?? 1).TenSanh),
            }, regularFont));
            document.Add(new Paragraph("\n")); // Line break
            // Danh sách dịch vụ
            document.Add(new Paragraph("Danh sách dịch vụ").SetFont(boldFont).SetFontSize(11));

            Table serviceTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 3, 2, 1, 3 }))
                .UseAllAvailableWidth();

            string[] headers = { "STT", "Tên dịch vụ", "Đơn giá", "Số lượng", "Ghi chú" };
            foreach (var h in headers)
                serviceTable.AddHeaderCell(PdfExportHelper.CreateCell(h, boldFont, align: textAlignmentC));
            for (int i = 1; i <= ServiceList.Count; i++) {
                var service = ServiceList[i-1];
                serviceTable.AddCell(PdfExportHelper.CreateCell(i.ToString(), regularFont, align: textAlignmentC));
                serviceTable.AddCell(PdfExportHelper.CreateCell(service.DichVu.TenDichVu, regularFont));
                serviceTable.AddCell(PdfExportHelper.CreateCell(service.DichVu.DonGia.ToString(), regularFont, align: textAlignmentR, isCurrency:true));
                serviceTable.AddCell(PdfExportHelper.CreateCell(service.SoLuong.ToString(), regularFont, align: textAlignmentC));
                serviceTable.AddCell(PdfExportHelper.CreateCell(service.GhiChu, regularFont));
            }
            document.Add(serviceTable);
            document.Add(new Paragraph("\n"));
            // Thông tin thanh toán
            PdfExportHelper.AddPaymentRow(document, "Tổng tiền bàn:", bill.TongTienBan, boldFont, regularFont);
            PdfExportHelper.AddPaymentRow(document, "Tổng tiền dịch vụ:", bill.TongTienDV, boldFont, regularFont);
            PdfExportHelper.AddPaymentRow(document, "Chi phí phát sinh:", bill.ChiPhiPhatSinh, boldFont, regularFont);
            PdfExportHelper.AddPaymentRow(document, "Tiền phạt:", bill.TienPhat, boldFont, regularFont);
            PdfExportHelper.AddPaymentRow(document, "Tổng hóa đơn:", bill.TongTienHoaDon, boldFont, regularFont);
            PdfExportHelper.AddPaymentRow(document, "Tiền đặt cọc:", bill.TienDatCoc, boldFont, regularFont);
            PdfExportHelper.AddPaymentRow(document, "Số tiền còn lại:", bill.TienConLai, boldFont, regularFont);

            // Dòng kẻ dưới
            document.Add(new LineSeparator(new SolidLine(1f)).SetMarginTop(15).SetMarginBottom(15));

            // Footer ngày lập hóa đơn
            document.Add(new Paragraph("Ngày lập hóa đơn: " + DateTime.Now.ToString("dd'/'MM'/'yyyy"))
                .SetFont(italicFont)
                .SetFontSize(11)
                .SetTextAlignment(textAlignmentR)
                .SetMarginBottom(40));

            // Footer chữ ký
            document.Add(new Paragraph("Người lập hóa đơn").SetFont(regularFont).SetTextAlignment(textAlignmentR).SetFontSize(11));
            document.Add(new Paragraph("(Ký tên)").SetFont(italicFont).SetTextAlignment(textAlignmentR).SetFontSize(10));

            document.Close();
        }
    }
}
