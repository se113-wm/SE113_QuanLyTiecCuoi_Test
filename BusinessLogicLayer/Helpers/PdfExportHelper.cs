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
using System.Windows.Controls;

namespace QuanLyTiecCuoi.Helpers {
    public static class PdfExportHelper {
        public static Table CreateInfoTable((string Label, string Value)[] data, PdfFont font) {
            var table = new Table(UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            foreach (var item in data) {
                table.AddCell(CreateCell(item.Label, font, align:TextAlignment.LEFT));
                table.AddCell(CreateCell(item.Value, font, align:TextAlignment.LEFT));
            }
            return table;
        }

        public static void AddPaymentRow(Document doc, string label, decimal? value, PdfFont boldFont, PdfFont regularFont) {
            Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 1 }))
                .UseAllAvailableWidth()
                .SetMarginBottom(5);
            table.AddCell(CreateCell(label, boldFont));
            table.AddCell(CreateCell(FormatCurrency(value), regularFont, align:TextAlignment.RIGHT));
            doc.Add(table);
        }

        public static Cell CreateCell(string text, PdfFont font, int colspan = 1, Color color = null, TextAlignment align = TextAlignment.LEFT, bool isCurrency = false) {
            Cell cell = new Cell();
            if (isCurrency) 
                cell = new Cell(1, colspan).Add(new Paragraph(FormatCurrency(decimal.Parse(text))).SetFont(font).SetFontSize(10)).SetTextAlignment(align);
            else
                cell = new Cell(1, colspan).Add(new Paragraph(text).SetFont(font).SetFontSize(10)).SetTextAlignment(align);
            if (color != null) cell.SetBackgroundColor(color ?? ColorConstants.WHITE);
            return cell.SetPadding(5).SetBorder(new SolidBorder(0.5f));
        }
        public static string FormatCurrency(decimal? amount) {
            return (amount ?? 0).ToString("#,0", CultureInfo.InvariantCulture);
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
