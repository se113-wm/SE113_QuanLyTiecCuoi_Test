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
using System.Globalization;

namespace QuanLyTiecCuoi.ViewModel {
    public class BillViewModel : BaseViewModel {
        private IPhieuDatTiecService _phieuDatTiecService;
        private ICaService _caService;
        private ISanhService _sanhService;

        private ObservableCollection<PHIEUDATTIECDTO> _List;
        public ObservableCollection<PHIEUDATTIECDTO> List { get => _List; set { _List = value; OnPropertyChanged(); } }

        private ObservableCollection<PHIEUDATTIECDTO> _OriginalList;
        public ObservableCollection<PHIEUDATTIECDTO> OriginalList { get => _OriginalList; set { _OriginalList = value; OnPropertyChanged(); } }

        private PHIEUDATTIECDTO _SelectedItem;
        public PHIEUDATTIECDTO SelectedItem {
            get => _SelectedItem;
            set { _SelectedItem = value; OnPropertyChanged(); }
        }

        // Filter properties
        public ObservableCollection<string> TenChuReFilterList { get; set; }
        public string SelectedTenChuRe { get => _selectedTenChuRe; set { _selectedTenChuRe = value; OnPropertyChanged(); PerformSearch(); } }
        private string _selectedTenChuRe;

        public ObservableCollection<string> TenCoDauFilterList { get; set; }
        public string SelectedTenCoDau { get => _selectedTenCoDau; set { _selectedTenCoDau = value; OnPropertyChanged(); PerformSearch(); } }
        private string _selectedTenCoDau;

        public ObservableCollection<string> TenSanhFilterList { get; set; }
        public string SelectedTenSanh { get => _selectedTenSanh; set { _selectedTenSanh = value; OnPropertyChanged(); PerformSearch(); } }
        private string _selectedTenSanh;

        public ObservableCollection<int> SoLuongBanFilterList { get; set; }
        public int? SelectedSoLuongBan { get => _selectedSoLuongBan; set { _selectedSoLuongBan = value; OnPropertyChanged(); PerformSearch(); } }
        private int? _selectedSoLuongBan;

        public DateTime? SelectedNgayDaiTiec { get => _selectedNgayDaiTiec; set { _selectedNgayDaiTiec = value; OnPropertyChanged(); PerformSearch(); } }
        private DateTime? _selectedNgayDaiTiec;

        // Search properties
        public ObservableCollection<string> SearchProperties { get; set; }
        public string SelectedSearchProperty { get => _selectedSearchProperty; set { _selectedSearchProperty = value; OnPropertyChanged(); PerformSearch(); } }
        private string _selectedSearchProperty;
        public string SearchText { get => _searchText; set { _searchText = value; OnPropertyChanged(); PerformSearch(); } }
        private string _searchText;

        // Command properties
        public ICommand ExportCommand { get; set; }

        public BillViewModel() {
            _phieuDatTiecService = new PhieuDatTiecService();
            _caService = new CaService();
            _sanhService = new SanhService();

            var all = _phieuDatTiecService.GetAll().ToList();
            List = new ObservableCollection<PHIEUDATTIECDTO>(all);
            OriginalList = new ObservableCollection<PHIEUDATTIECDTO>(all);

            // Build filter lists
            TenChuReFilterList = new ObservableCollection<string>(all.Select(x => x.TenChuRe).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x));
            TenCoDauFilterList = new ObservableCollection<string>(all.Select(x => x.TenCoDau).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x));
            TenSanhFilterList = new ObservableCollection<string>(all.Select(x => x.Sanh?.TenSanh).Where(x => !string.IsNullOrEmpty(x)).Distinct().OrderBy(x => x));
            SoLuongBanFilterList = new ObservableCollection<int>(all.Select(x => x.SoLuongBan ?? 0).Where(x => x > 0).Distinct().OrderBy(x => x));

            SearchProperties = new ObservableCollection<string>
            {
                "Tên chú rể",
                "Tên cô dâu",
                "Tên sảnh",
                "Ngày đãi tiệc",
                "Số lượng bàn"
            };
            SelectedSearchProperty = SearchProperties.FirstOrDefault();

            ExportCommand = new RelayCommand<Window>((p) => true, (p) => {
                try {
                    if (SelectedItem == null) {
                        return;
                    }
                    var dialog = new Microsoft.Win32.SaveFileDialog {
                        FileName = $"HoaDon_{SelectedItem.MaPhieuDat}",
                        DefaultExt = ".pdf",
                        Filter = "PDF documents (.pdf)|*.pdf"
                    };
                    bool? result = dialog.ShowDialog();
                    if (result == true) {
                        string filePath = dialog.FileName;
                        ExportInvoice(SelectedItem, filePath);
                        MessageBox.Show("Xuất hóa đơn thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex) {
                    MessageBox.Show($"Lỗi khi xuất hóa đơn: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }
        private void ExportInvoice(PHIEUDATTIECDTO bill, string outputPath) {

            var regularFont = PDFFont.RegularFont;
            var boldFont = PDFFont.BoldFont;
            var italicFont = PDFFont.ItalicFont;
            var textAlignmentR = iText.Layout.Properties.TextAlignment.RIGHT;
            var textAlignmentL = iText.Layout.Properties.TextAlignment.LEFT;
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

            // Thông tin thanh toán
            var paymentTable = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            paymentTable.AddHeaderCell(PdfExportHelper.CreateCell("TỔNG HÓA ĐƠN:", boldFont, true, ColorConstants.LIGHT_GRAY, textAlignmentL));
            PdfExportHelper.AddPaymentRow(paymentTable, "Tổng tiền bàn:", bill.TongTienBan, regularFont);
            PdfExportHelper.AddPaymentRow(paymentTable, "Tổng tiền dịch vụ:", bill.TongTienDV, regularFont);
            PdfExportHelper.AddPaymentRow(paymentTable, "Chi phí phát sinh:", bill.ChiPhiPhatSinh, regularFont);
            PdfExportHelper.AddPaymentRow(paymentTable, "Tiền phạt:", bill.TienPhat, regularFont);
            PdfExportHelper.AddPaymentRow(paymentTable, "Tổng hóa đơn:", bill.TongTienHoaDon, regularFont);
            PdfExportHelper.AddPaymentRow(paymentTable, "Tiền đặt cọc:", bill.TienDatCoc, regularFont);
            PdfExportHelper.AddPaymentRow(paymentTable, "Số tiền còn lại:", bill.TienConLai, regularFont, isHighlight: true);

            document.Add(paymentTable);

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
        private void PerformSearch() {
            try {
                var filtered = OriginalList.AsEnumerable();

                // Filter by ComboBox filters
                if (!string.IsNullOrEmpty(SelectedTenChuRe))
                    filtered = filtered.Where(x => x.TenChuRe == SelectedTenChuRe);
                if (!string.IsNullOrEmpty(SelectedTenCoDau))
                    filtered = filtered.Where(x => x.TenCoDau == SelectedTenCoDau);
                if (!string.IsNullOrEmpty(SelectedTenSanh))
                    filtered = filtered.Where(x => x.Sanh != null && x.Sanh.TenSanh == SelectedTenSanh);
                if (SelectedNgayDaiTiec.HasValue)
                    filtered = filtered.Where(x => x.NgayDaiTiec.HasValue && x.NgayDaiTiec.Value.Date == SelectedNgayDaiTiec.Value.Date);
                if (SelectedSoLuongBan.HasValue)
                    filtered = filtered.Where(x => x.SoLuongBan == SelectedSoLuongBan);

                // Search by text
                if (!string.IsNullOrEmpty(SearchText) && !string.IsNullOrEmpty(SelectedSearchProperty)) {
                    var search = SearchText.Trim().ToLower();
                    switch (SelectedSearchProperty) {
                        case "Tên chú rể":
                            filtered = filtered.Where(x => !string.IsNullOrEmpty(x.TenChuRe) && x.TenChuRe.ToLower().Contains(search));
                            break;
                        case "Tên cô dâu":
                            filtered = filtered.Where(x => !string.IsNullOrEmpty(x.TenCoDau) && x.TenCoDau.ToLower().Contains(search));
                            break;
                        case "Tên sảnh":
                            filtered = filtered.Where(x => x.Sanh != null && !string.IsNullOrEmpty(x.Sanh.TenSanh) && x.Sanh.TenSanh.ToLower().Contains(search));
                            break;
                        case "Ngày đãi tiệc":
                            filtered = filtered.Where(x => x.NgayDaiTiec.HasValue && x.NgayDaiTiec.Value.ToString("dd/MM/yyyy").Contains(search));
                            break;
                        case "Số lượng bàn":
                            filtered = filtered.Where(x => x.SoLuongBan.HasValue && x.SoLuongBan.Value.ToString().Contains(search));
                            break;
                        default:
                            break;
                    }
                }

                List = new ObservableCollection<PHIEUDATTIECDTO>(filtered);
            }
            catch (Exception ex) {
                MessageBox.Show($"Đã xảy ra lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
