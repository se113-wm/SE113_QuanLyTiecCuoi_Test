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
        public static Table CreateInfoTable((string Label, string Value)[] data, PdfFont font) {
            var table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            foreach (var item in data) {
                table.AddCell(CreateCell(item.Label, font, true, ColorConstants.LIGHT_GRAY, TextAlignment.LEFT));
                table.AddCell(CreateCell(item.Value, font, false, ColorConstants.WHITE, TextAlignment.LEFT));
            }
            return table;
        }

        public static void AddPaymentRow(Table table, string label, decimal? value, PdfFont font, bool isHighlight = false) {
            var bgColor = isHighlight ? ColorConstants.YELLOW : ColorConstants.WHITE;
            table.AddCell(CreateCell(label, font, true, ColorConstants.LIGHT_GRAY, TextAlignment.LEFT));
            table.AddCell(CreateCell(FormatCurrency(value), font, false, bgColor, TextAlignment.RIGHT));
        }

        public static Cell CreateCell(string text, PdfFont font, bool isHeader = false, Color backgroundColor = null, TextAlignment align = TextAlignment.LEFT) {
            var cell = new Cell().Add(new Paragraph(text).SetFont(font).SetFontSize(isHeader ? 11 : 10));
            cell.SetPadding(5);
            cell.SetTextAlignment(align);
            cell.SetBackgroundColor(backgroundColor ?? ColorConstants.WHITE);
            cell.SetBorder(new SolidBorder(ColorConstants.GRAY, 0.5f));
            if (isHeader) cell.SetFont(PDFFont.BoldFont);
            return cell;
        }

        public static string FormatCurrency(decimal? amount) {
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
