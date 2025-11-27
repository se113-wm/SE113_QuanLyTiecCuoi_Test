using ClosedXML.Excel;
using LiveCharts.Definitions.Charts;
using Microsoft.Win32;
using MigraDocCore.DocumentObjectModel;
using MigraDocCore.DocumentObjectModel.Tables;
using MigraDocCore.Rendering;
using PdfSharpCore.Utils;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Presentation;
using QuanLyTiecCuoi.Presentation.View;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace QuanLyTiecCuoi.Presentation.ViewModel
{
    public class ReportViewModel : INotifyPropertyChanged
    {
        private readonly IRevenueReportDetailService _revenueReportDetailService;

        public ObservableCollection<int> Months { get; set; }
        public ObservableCollection<int> Years { get; set; }
        public ObservableCollection<RevenueReportDetailDTO> ReportList { get; set; }

        private int _selectedMonth;
        public int SelectedMonth
        {
            get => _selectedMonth;
            set { _selectedMonth = value; OnPropertyChanged(); }
        }

        private int _selectedYear;
        public int SelectedYear
        {
            get => _selectedYear;
            set { _selectedYear = value; OnPropertyChanged(); }
        }

        private decimal _totalRevenue;
        public decimal TotalRevenue
        {
            get => _totalRevenue;
            set { _totalRevenue = value; OnPropertyChanged(); }
        }

        public ICommand LoadReportCommand { get; set; }
        public ICommand ExportPdfCommand { get; set; }
        public ICommand ExportExcelCommand { get; set; }
        public ICommand ShowChartCommand { get; set; }

        public ReportViewModel(IRevenueReportDetailService revenueReportDetailService)
        {
            _revenueReportDetailService = revenueReportDetailService;

            Months = new ObservableCollection<int>();
            Years = new ObservableCollection<int>();
            ReportList = new ObservableCollection<RevenueReportDetailDTO>();
         
            LoadReportCommand = new RelayCommand(_ => LoadReport());
            ExportPdfCommand = new RelayCommand(_ => ExportPdf());
            ExportExcelCommand = new RelayCommand(_ => ExportExcel());
            ShowChartCommand = new RelayCommand(_ => ShowChart());

            InitializeTimeOptions();
            LoadReport();
        }

        private void InitializeTimeOptions()
        {
            for (int i = 1; i <= 12; i++) Months.Add(i);
            for (int y = DateTime.Now.Year - 5; y <= DateTime.Now.Year; y++) Years.Add(y);

            SelectedMonth = DateTime.Now.Month;
            SelectedYear = DateTime.Now.Year;
        }

        private void LoadReport()
        {
            ReportList.Clear();
            var data = _revenueReportDetailService.GetByMonthYear(SelectedMonth, SelectedYear)
                .Where(x => x.WeddingCount > 0 && (x.Revenue ?? 0) > 0)
                .ToList();
            Console.WriteLine($"Đã load dòng dữ liệu.");

            decimal total = data.Sum(x => x.Revenue ?? 0);
            int rowNumber = 1;

            foreach (var item in data)
            {
                item.RowNumber = rowNumber++;
                item.Ratio = total == 0 ? 0 : Math.Round(((item.Revenue ?? 0) / total) * 100, 2);
                ReportList.Add(item);
            }

            TotalRevenue = total;
        }

        private void ExportExcel()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "Excel files (*.xlsx)|*.xlsx",
                    FileName = $"BaoCaoDoanhThu_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var workbook = new XLWorkbook();
                    var worksheet = workbook.Worksheets.Add("Báo Cáo Doanh Thu");

                    worksheet.Cell(1, 1).Value = "STT";
                    worksheet.Cell(1, 2).Value = "Ngày";
                    worksheet.Cell(1, 3).Value = "Số lượng tiệc";
                    worksheet.Cell(1, 4).Value = "Doanh thu";
                    worksheet.Cell(1, 5).Value = "Tỉ lệ";

                    var headerRange = worksheet.Range("A1:E1");
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                    headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    headerRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                    int row = 2;
                    int stt = 1;
                    foreach (var item in ReportList)
                    {
                        worksheet.Cell(row, 1).Value = stt;
                        worksheet.Cell(row, 2).Value = $"{item.Day}/{item.Month}/{item.Year}";
                        worksheet.Cell(row, 3).Value = item.WeddingCount ?? 0;

                        for (int col = 1; col <= 5; col++)
                        {
                            worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(row, col).Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                            worksheet.Cell(row, col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        }

                        worksheet.Cell(row, 4).Value = item.Revenue ?? 0;
                        worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0 \"VNĐ\"";

                        worksheet.Cell(row, 5).Value = (item.Ratio ?? 0) / 100m;
                        worksheet.Cell(row, 5).Style.NumberFormat.Format = "0.00%";

                        row++;
                        stt++;
                    }

                    worksheet.Columns().AdjustToContents();

                    workbook.SaveAs(saveFileDialog.FileName);
                    Process.Start("explorer.exe", saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất Excel: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportPdf()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = "PDF files (*.pdf)|*.pdf",
                    FileName = $"BaoCaoDoanhThu_{DateTime.Now:yyyyMMddHHmmss}.pdf"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    var doc = new Document();
                    var section = doc.AddSection();

                    doc.Styles["Normal"].Font.Name = "Times New Roman";
                    doc.Styles["Normal"].Font.Size = 12;

                    var title = section.AddParagraph("BÁO CÁO DOANH THU");
                    title.Format.Font.Size = 16;
                    title.Format.Font.Bold = true;
                    title.Format.SpaceAfter = "0.3cm";
                    title.Format.SpaceBefore = "0.5cm";
                    title.Format.Alignment = ParagraphAlignment.Center;

                    var reportDate = section.AddParagraph($"Ngày lập báo cáo: {DateTime.Now:dd/MM/yyyy}");
                    reportDate.Format.SpaceAfter = "0.2cm";
                    reportDate.Format.Alignment = ParagraphAlignment.Right;
                    reportDate.Format.Font.Size = 12;
                    reportDate.Format.Font.Italic = true;

                    decimal tongDoanhThu = 0;
                    int tongSoTiec = 0;
                    foreach (var item in ReportList)
                    {
                        tongDoanhThu += item.Revenue ?? 0;
                        tongSoTiec += item.WeddingCount ?? 0;
                    }

                    var summary = section.AddParagraph($"Tổng số tiệc: {tongSoTiec} | Tổng doanh thu: {tongDoanhThu:N0} VNĐ");
                    summary.Format.SpaceAfter = "0.5cm";
                    summary.Format.Alignment = ParagraphAlignment.Left;
                    summary.Format.Font.Bold = true;

                    var table = section.AddTable();
                    table.Borders.Width = 0.75;
                    table.Borders.Color = Colors.Black;
                    table.Format.Font.Name = "Times New Roman";

                    table.AddColumn("2cm");
                    table.AddColumn("3cm");
                    table.AddColumn("3cm");
                    table.AddColumn("4cm");
                    table.AddColumn("3cm");

                    var header = table.AddRow();
                    header.Shading.Color = Colors.LightGray;
                    header.HeadingFormat = true;
                    header.Format.Font.Bold = true;
                    header.Format.Alignment = ParagraphAlignment.Center;

                    header.Cells[0].AddParagraph("STT");
                    header.Cells[1].AddParagraph("Ngày");
                    header.Cells[2].AddParagraph("Số lượng");
                    header.Cells[3].AddParagraph("Doanh thu");
                    header.Cells[4].AddParagraph("Tỉ lệ");

                    int stt = 1;
                    foreach (var item in ReportList)
                    {
                        var row = table.AddRow();
                        row.Format.Alignment = ParagraphAlignment.Center;
                        row.Cells[0].AddParagraph(stt.ToString());
                        row.Cells[1].AddParagraph($"{item.Day}/{item.Month}/{item.Year}");
                        row.Cells[2].AddParagraph(item.WeddingCount?.ToString() ?? "0");
                        row.Cells[3].AddParagraph(string.Format("{0:N0} VNĐ", item.Revenue ?? 0));
                        row.Cells[4].AddParagraph((item.Ratio ?? 0).ToString("N2", CultureInfo.InvariantCulture) + " %");
                        stt++;
                    }

                    var renderer = new PdfDocumentRenderer(unicode: true)
                    {
                        Document = doc
                    };
                    renderer.RenderDocument();

                    using (var stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        renderer.PdfDocument.Save(stream);
                    }

                    Process.Start("explorer.exe", saveFileDialog.FileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xuất PDF: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowChart()
        {
            var chartWindow = new ChartView
            {
                DataContext = new ChartViewModel(ReportList)
            };
            chartWindow.ShowDialog();
        }

        private void OnPropertyChanged([CallerMemberName] string property = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public class RelayCommand : ICommand
        {
            private readonly Action<object> _execute;
            private readonly Predicate<object> _canExecute;

            public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
            public void Execute(object parameter) => _execute(parameter);

            public event EventHandler CanExecuteChanged
            {
                add { CommandManager.RequerySuggested += value; }
                remove { CommandManager.RequerySuggested -= value; }
            }
        }
    }
}
