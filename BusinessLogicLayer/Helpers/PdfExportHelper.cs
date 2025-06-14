using System;
using System.Globalization;
using System.IO;
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
using QuanLyTiecCuoi.DataTransferObject;
using iText.Kernel.Pdf.Canvas.Draw;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;

namespace QuanLyTiecCuoi.Helpers {
    public static class PdfExportHelper {
        public static void ExportInvoice(PHIEUDATTIECDTO bill, string outputPath) {
            var sanhService = new SanhService();
            var caService = new CaService();

            var writer = new PdfWriter(outputPath);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf, iText.Kernel.Geom.PageSize.A4);
            document.SetMargins(40, 30, 40, 30);

            /*// Logo (nếu có)
            var logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png");
            if (File.Exists(logoPath)) {
                var logo = new Image(ImageDataFactory.Create(logoPath)).ScaleToFit(80, 80).SetHorizontalAlignment(HorizontalAlignment.CENTER);
                document.Add(logo);
            }*/

            // Tiêu đề
            var header = new Paragraph("HÓA ĐƠN THANH TOÁN TIỆC CƯỚI")
                .SetFontSize(20)
                .SetFont(PDFFont.BoldFont)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetMarginBottom(10);
            document.Add(header);

            // Dòng kẻ
            document.Add(new LineSeparator(new SolidLine(1f)).SetMarginBottom(15));

            // Thông tin tiệc cưới
            document.Add(CreateInfoTable(new[] {
                ("Tên chú rể:", bill.TenChuRe ?? ""),
                ("Tên cô dâu:", bill.TenCoDau ?? ""),
                ("Ngày đãi tiệc:", bill.NgayDaiTiec?.ToString("dd'/'MM'/'yyyy") ?? ""),
                ("Số lượng bàn:", bill.SoLuongBan?.ToString() ?? ""),
                ("Ca:", caService.GetById(bill.MaCa ?? 1).TenCa),
                ("Sảnh:", sanhService.GetById(bill.MaSanh ?? 1).TenSanh),
            }));
            document.Add(new Paragraph("\n")); // Line break

            // Thông tin thanh toán
            var paymentTable = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            paymentTable.AddHeaderCell(CreateCell("TỔNG HÓA ĐƠN:", PDFFont.BoldFont, true, ColorConstants.LIGHT_GRAY, TextAlignment.LEFT));
            AddPaymentRow(paymentTable, "Tổng tiền bàn:", bill.TongTienBan);
            AddPaymentRow(paymentTable, "Tổng tiền dịch vụ:", bill.TongTienDV);
            AddPaymentRow(paymentTable, "Chi phí phát sinh:", bill.ChiPhiPhatSinh);
            AddPaymentRow(paymentTable, "Tiền phạt:", bill.TienPhat);
            AddPaymentRow(paymentTable, "Tổng hóa đơn:", bill.TongTienHoaDon);
            AddPaymentRow(paymentTable, "Tiền đặt cọc:", bill.TienDatCoc);
            AddPaymentRow(paymentTable, "Số tiền còn lại:", bill.TienConLai, isHighlight: true);

            document.Add(paymentTable);

            // Dòng kẻ dưới
            document.Add(new LineSeparator(new SolidLine(1f)).SetMarginTop(15).SetMarginBottom(15));

            // Footer ngày lập hóa đơn
            document.Add(new Paragraph("Ngày lập hóa đơn: " + DateTime.Now.ToString("dd'/'MM'/'yyyy"))
                .SetFont(PDFFont.ItalicFont)
                .SetFontSize(11)
                .SetTextAlignment(TextAlignment.RIGHT)
                .SetMarginBottom(40));

            // Footer chữ ký
            document.Add(new Paragraph("Người lập hóa đơn").SetFont(PDFFont.RegularFont).SetTextAlignment(TextAlignment.RIGHT).SetFontSize(11));
            document.Add(new Paragraph("(Ký tên)").SetFont(PDFFont.ItalicFont).SetTextAlignment(TextAlignment.RIGHT).SetFontSize(10));

            document.Close();
        }

        private static Table CreateInfoTable((string Label, string Value)[] data) {
            var table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            foreach (var item in data) {
                table.AddCell(CreateCell(item.Label, PDFFont.RegularFont, true, ColorConstants.LIGHT_GRAY, TextAlignment.LEFT));
                table.AddCell(CreateCell(item.Value, PDFFont.RegularFont, false, ColorConstants.WHITE, TextAlignment.LEFT));
            }
            return table;
        }

        private static void AddPaymentRow(Table table, string label, decimal? value, bool isHighlight = false) {
            var bgColor = isHighlight ? ColorConstants.YELLOW : ColorConstants.WHITE;
            table.AddCell(CreateCell(label, PDFFont.RegularFont, true, ColorConstants.LIGHT_GRAY, TextAlignment.LEFT));
            table.AddCell(CreateCell(FormatCurrency(value), PDFFont.RegularFont, false, bgColor, TextAlignment.RIGHT));
        }

        private static Cell CreateCell(string text, PdfFont font, bool isHeader = false, Color backgroundColor = null, TextAlignment align = TextAlignment.LEFT) {
            var cell = new Cell().Add(new Paragraph(text).SetFont(font).SetFontSize(isHeader ? 11 : 10));
            cell.SetPadding(5);
            cell.SetTextAlignment(align);
            cell.SetBackgroundColor(backgroundColor ?? ColorConstants.WHITE);
            cell.SetBorder(new SolidBorder(ColorConstants.GRAY, 0.5f));
            if (isHeader) cell.SetFont(PDFFont.BoldFont);
            return cell;
        }

        private static string FormatCurrency(decimal? amount) {
            return (amount ?? 0).ToString("N0", CultureInfo.GetCultureInfo("vi-VN")) + " VNĐ";
        }
    }

    public static class PDFFont {
        private static readonly string fontsFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts");

        private static readonly Lazy<PdfFont> _font = new Lazy<PdfFont>(() => PdfFontFactory.CreateFont(Path.Combine(fontsFolderPath, "TIMES.TTF"), PdfEncodings.IDENTITY_H));
        private static readonly Lazy<PdfFont> _boldFont = new Lazy<PdfFont>(() => PdfFontFactory.CreateFont(Path.Combine(fontsFolderPath, "TIMESBD.TTF"), PdfEncodings.IDENTITY_H));
        private static readonly Lazy<PdfFont> _italicFont = new Lazy<PdfFont>(() => PdfFontFactory.CreateFont(Path.Combine(fontsFolderPath, "TIMESI.TTF"), PdfEncodings.IDENTITY_H));

        public static PdfFont RegularFont => _font.Value;
        public static PdfFont BoldFont => _boldFont.Value;
        public static PdfFont ItalicFont => _italicFont.Value;
    }
}
