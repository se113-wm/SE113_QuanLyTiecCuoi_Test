using DocumentFormat.OpenXml.Drawing.Charts;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using MaterialDesignThemes.Wpf.Converters;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Helpers;
using QuanLyTiecCuoi.Model;
using QuanLyTiecCuoi.Presentation.View;
using QuanLyTiecCuoi.Presentation.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace QuanLyTiecCuoi.ViewModel {
    public class InvoiceViewModel : BaseViewModel {
        private readonly IBookingService _bookingService;
        private readonly IShiftService _shiftService;
        private readonly IHallService _hallService;
        private readonly IServiceDetailService _serviceDetailService;
        private readonly IMenuService _menuService;
        private readonly IParameterService _parameterService;

        private BookingDTO _selectedInvoice;
        public BookingDTO SelectedInvoice { get => _selectedInvoice; set { _selectedInvoice = value; OnPropertyChanged(); } }
        
        private int _invoiceId;
        public int InvoiceId { get => _invoiceId; set { _invoiceId = value; OnPropertyChanged(); } }

        private decimal? _tablePrice;
        public decimal? TablePrice { get => _tablePrice; set { _tablePrice = value; OnPropertyChanged(); } }
        
        private bool _isPaid = false;
        public bool IsPaid { get => _isPaid; set { _isPaid = value; OnPropertyChanged(); } }
        
        private DateTime? _paymentDate = DateTime.Now;
        public DateTime? PaymentDate { get => _paymentDate; set { _paymentDate = value; OnPropertyChanged(); } }
        
        private string _tableQuantity;
        public string TableQuantity { 
            get => _tableQuantity; 
            set { 
                _tableQuantity = value; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(TotalInvoiceAmount)); 
                OnPropertyChanged(nameof(RemainingAmount)); 
            } 
        }
        
        private string _tableQuantityMax = "Maximum table count is ";
        public string TableQuantityMax { get => _tableQuantityMax; set { _tableQuantityMax = value; OnPropertyChanged(); } }
        
        public decimal? RemainingAmount { 
            get {
                decimal? sum = null;
                if(Deposit is null) {
                    return null;
                }
                if (TotalInvoiceAmount != null) {
                    sum = TotalInvoiceAmount;
                    if (Fine != null) {
                        sum += Fine;
                    }
                    sum -= Deposit;
                }
                return sum;
            }
        } 
        
        private string _tableQuantityMessage = "Pre-booked table count is ";
        public string TableQuantityMessage { get => _tableQuantityMessage; set { _tableQuantityMessage = value; OnPropertyChanged(); } }
        
        private string _additionalCost;
        public string AdditionalCost { 
            get => _additionalCost; 
            set { 
                _additionalCost = value; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(TotalInvoiceAmount)); 
                OnPropertyChanged(nameof(RemainingAmount)); 
            } 
        }
        
        private decimal? _deposit;
        public decimal? Deposit { get => _deposit; set { _deposit = value; OnPropertyChanged(); } }
        
        private decimal? _fine;
        public decimal? Fine { 
            get => _fine; 
            set { 
                _fine = value; 
                OnPropertyChanged(); 
                OnPropertyChanged(nameof(RemainingAmount)); 
            } 
        }
        
        public decimal? TotalInvoiceAmount
        {
            get
            {
                if (SelectedInvoice != null && int.TryParse(TableQuantity, out int quantity))
                {
                    var menuList = _menuService.GetByBookingId(SelectedInvoice.BookingId);
                    decimal totalDishAmount = menuList.Sum(m => (m.UnitPrice ?? 0) * (m.Quantity ?? 0));
                    decimal totalTableAmount = quantity * ((SelectedInvoice.TablePrice ?? 0) + totalDishAmount);

                    if (decimal.TryParse(AdditionalCost, out decimal additionalCost))
                        return totalTableAmount + (SelectedInvoice.TotalServiceAmount ?? 0) + additionalCost;
                    else
                        return totalTableAmount + (SelectedInvoice.TotalServiceAmount ?? 0);
                }
                return 0;
            }
            set
            {
                TotalInvoiceAmount = value;
            }
        }
        
        private string _paymentText = "Confirm Payment";
        public string PaymentText { get => _paymentText; set { _paymentText = value; OnPropertyChanged(); } }
        
        public ObservableCollection<ServiceDetailDTO> ServiceList { get; set; } = new ObservableCollection<ServiceDetailDTO>();
        private bool _canExport = false;

        // Command properties
        public ICommand ExportCommand { get; set; }
        public ICommand ConfirmPaymentCommand { get; set; }
        
        private string _confirmMessage;
        public string ConfirmMessage { get => _confirmMessage; set { _confirmMessage = value; OnPropertyChanged(); } }

        // Constructor with Dependency Injection
        public InvoiceViewModel(
            int invoiceId,
            IBookingService bookingService,
            IShiftService shiftService,
            IHallService hallService,
            IServiceDetailService serviceDetailService,
            IMenuService menuService,
            IParameterService parameterService)
        {
            InvoiceId = invoiceId;
            
            // Inject services
            _bookingService = bookingService;
            _shiftService = shiftService;
            _hallService = hallService;
            _serviceDetailService = serviceDetailService;
            _menuService = menuService;
            _parameterService = parameterService;
            
            SelectedInvoice = _bookingService.GetById(invoiceId);
            ServiceList = new ObservableCollection<ServiceDetailDTO>(_serviceDetailService.GetByBookingId(invoiceId));

            TableQuantity = SelectedInvoice.TableCount.ToString();
            TableQuantityMessage += $"{SelectedInvoice.TableCount}";
            TableQuantityMax += SelectedInvoice.Hall.MaxTableCount.ToString();
            Deposit = SelectedInvoice.Deposit;

            var menuList = _menuService.GetByBookingId(invoiceId);
            decimal totalDishAmount = menuList.Sum(m => (m.UnitPrice ?? 0) * (m.Quantity ?? 0));
            TablePrice = (SelectedInvoice.TablePrice ?? 0) + totalDishAmount;

            if (SelectedInvoice.PaymentDate != null) {
                IsPaid = true;
                _canExport = true;
                PaymentText = "Paid";

                PaymentDate = SelectedInvoice.PaymentDate;
                TableQuantity = SelectedInvoice.TableCount.ToString();
                Fine = SelectedInvoice.PenaltyAmount;
                AdditionalCost = SelectedInvoice.AdditionalCost.ToString();

                TableQuantityMessage = string.Empty;
            }
            else {
                decimal? tmpTotalInvoiceAmount = TotalInvoiceAmount;
                // Read parameter values using database names
                decimal? penaltyRate = _parameterService.GetByName("PenaltyRate")?.Value ?? 0m;
                decimal? penaltyCheck = _parameterService.GetByName("EnablePenalty")?.Value ?? 0m;
                int dayDiff = (DateTime.Now - SelectedInvoice.WeddingDate.GetValueOrDefault()).Days;
                if (dayDiff < 0) {
                    dayDiff = 0;
                }
                Fine = penaltyRate * penaltyCheck * (tmpTotalInvoiceAmount - SelectedInvoice.Deposit) * (decimal)dayDiff;
            }
            
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            ExportCommand = new RelayCommand<Window>((p) => { return _canExport; }, (p) => {
                try
                {
                    if (SelectedInvoice == null)
                    {
                        return;
                    }
                    var dialog = new Microsoft.Win32.SaveFileDialog
                    {
                        FileName = $"Invoice_{SelectedInvoice.BookingId}",
                        DefaultExt = ".pdf",
                        Filter = "PDF documents (.pdf)|*.pdf"
                    };
                    bool? result = dialog.ShowDialog();
                    if (result == true)
                    {
                        string filePath = dialog.FileName;
                        ExportInvoiceToPdf(SelectedInvoice, filePath);
                        MessageBox.Show("Invoice exported successfully!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                        System.Diagnostics.Process.Start(filePath);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting invoice: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
            
            ConfirmPaymentCommand = new RelayCommand<Window>((p) => {
                if (SelectedInvoice == null) {
                    return false;
                }
                if (IsPaid) {
                    _canExport = true;
                    ConfirmMessage = "Invoice has been paid";
                    return false;
                }
                if (!int.TryParse(TableQuantity, out int tableQuantity)) {
                    _canExport = false;
                    ConfirmMessage = "Table count must be an integer";
                    return false;
                }
                if (int.Parse(TableQuantity) < SelectedInvoice.TableCount) {
                    _canExport = false;
                    ConfirmMessage = "Used table count cannot be less than booked count";
                    return false;
                }
                if(int.Parse(TableQuantity) > SelectedInvoice.Hall.MaxTableCount) {
                    _canExport = false;
                    ConfirmMessage = "Used table count cannot exceed hall maximum";
                    return false;
                }
                if (string.IsNullOrWhiteSpace(AdditionalCost)) {
                    _canExport = false;
                    ConfirmMessage = "Enter additional cost";
                    return false;
                }
                if (SelectedInvoice.WeddingDate.HasValue && DateTime.Now < SelectedInvoice.WeddingDate.Value) {
                    _canExport = false;
                    ConfirmMessage = "Cannot pay before wedding date";
                    return false;
                }
                ConfirmMessage = string.Empty;
                return true;
            }, (p) => {
                var window = p as Window;
                try
                {
                    var result = window != null
                        ? MessageBox.Show(window, "Are you sure you want to confirm payment? Note: Payment can only be made once.", "Confirm Payment", MessageBoxButton.YesNo, MessageBoxImage.Question)
                        : MessageBox.Show("Are you sure you want to confirm payment? Note: Payment can only be made once.", "Confirm Payment", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            ProcessPayment();
                            SelectedInvoice = _bookingService.GetById(_invoiceId);
                            if (window != null)
                                MessageBox.Show(window, "Payment successful!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            else
                                MessageBox.Show("Payment successful!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            if (window != null)
                                MessageBox.Show(window, $"Payment error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            else
                                MessageBox.Show($"Payment error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (window != null)
                        MessageBox.Show(window, $"Confirmation error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    else
                        MessageBox.Show($"Confirmation error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }

        private void ProcessPayment() {
            int tableQuantity = 0;
            decimal totalTableAmount = 0;
            decimal additionalCost = 0;
            
            if (int.TryParse(TableQuantity, out int parsedQuantity))
            {
                tableQuantity = parsedQuantity;
                var menuList = _menuService.GetByBookingId(SelectedInvoice.BookingId);
                decimal totalDishAmount = menuList.Sum(m => (m.UnitPrice ?? 0) * (m.Quantity ?? 0));
                totalTableAmount = tableQuantity * ((SelectedInvoice.TablePrice ?? 0) + totalDishAmount);
            }
            additionalCost = decimal.Parse(AdditionalCost);
            
            try {
                var shift = _shiftService.GetById(SelectedInvoice.ShiftId.GetValueOrDefault());
                var hall = _hallService.GetById(SelectedInvoice.HallId.GetValueOrDefault());
                
                SelectedInvoice = _bookingService.GetById(_invoiceId);
                BookingDTO invoice = new BookingDTO {
                    BookingId = SelectedInvoice.BookingId,
                    GroomName = SelectedInvoice.GroomName,
                    BrideName = SelectedInvoice.BrideName,
                    Phone = SelectedInvoice.Phone,
                    WeddingDate = SelectedInvoice.WeddingDate.Value,
                    BookingDate = SelectedInvoice.BookingDate,
                    Shift = shift,
                    Hall = hall,
                    Deposit = SelectedInvoice.Deposit,
                    ReserveTableCount = SelectedInvoice.ReserveTableCount,
                    ShiftId = SelectedInvoice.ShiftId,
                    HallId = SelectedInvoice.HallId,
                    TablePrice = SelectedInvoice.TablePrice,
                    TotalTableAmount = SelectedInvoice.TotalTableAmount,
                    TotalInvoiceAmount = SelectedInvoice.TotalInvoiceAmount,
                    RemainingAmount = SelectedInvoice.RemainingAmount,
                    TotalServiceAmount = SelectedInvoice.TotalServiceAmount,
                    PaymentDate = DateTime.Now,
                    TableCount = tableQuantity,
                    AdditionalCost = additionalCost,
                };
                _bookingService.Update(invoice);
                
                invoice = _bookingService.GetAll().LastOrDefault();

                Deposit = invoice.Deposit.GetValueOrDefault();
                Fine = invoice.PenaltyAmount.GetValueOrDefault();
                OnPropertyChanged();
                IsPaid = true;
            }
            catch (Exception e) {
                MessageBox.Show(e.Message);
            }
        }

        private void ExportInvoiceToPdf(BookingDTO bill, string outputPath) {
            var regularFont = PDFFont.RegularFont;
            var boldFont = PDFFont.BoldFont;
            var italicFont = PDFFont.ItalicFont;
            var textAlignmentR = iText.Layout.Properties.TextAlignment.RIGHT;
            var textAlignmentC = iText.Layout.Properties.TextAlignment.CENTER;

            var writer = new PdfWriter(outputPath);
            var pdf = new PdfDocument(writer);
            var document = new Document(pdf, iText.Kernel.Geom.PageSize.A4);
            document.SetMargins(40, 30, 40, 30);

            // Header
            var header = new Paragraph("WEDDING BANQUET PAYMENT INVOICE")
                .SetFontSize(20)
                .SetFont(PDFFont.BoldFont)
                .SetTextAlignment(textAlignmentC)
                .SetMarginBottom(10);
            document.Add(header);

            document.Add(new LineSeparator(new SolidLine(1f)).SetMarginBottom(15));

            // Wedding information
            document.Add(PdfExportHelper.CreateInfoTable(new[] {
                ("Groom name:", bill.GroomName ?? ""),
                ("Bride name:", bill.BrideName ?? ""),
                ("Wedding date:", bill.WeddingDate?.ToString("dd'/'MM'/'yyyy") ?? ""),
                ("Table count:", bill.TableCount?.ToString() ?? ""),
                ("Shift:", _shiftService.GetById(bill.ShiftId ?? 1).ShiftName),
                ("Hall:", _hallService.GetById(bill.HallId ?? 1).HallName),
            }, regularFont));
            document.Add(new Paragraph("\n"));
            
            // Service list
            document.Add(new Paragraph("Service List").SetFont(boldFont).SetFontSize(11));

            Table serviceTable = new Table(UnitValue.CreatePercentArray(new float[] { 1, 3, 2, 1, 3 }))
                .UseAllAvailableWidth();

            string[] headers = { "No.", "Service name", "Unit price", "Quantity", "Note" };
            foreach (var h in headers)
                serviceTable.AddHeaderCell(PdfExportHelper.CreateCell(h, boldFont, align: textAlignmentC));
            
            for (int i = 1; i <= ServiceList.Count; i++) {
                var service = ServiceList[i-1];
                serviceTable.AddCell(PdfExportHelper.CreateCell(i.ToString(), regularFont, align: textAlignmentC));
                serviceTable.AddCell(PdfExportHelper.CreateCell(service.Service.ServiceName, regularFont));
                serviceTable.AddCell(PdfExportHelper.CreateCell(service.Service.UnitPrice.ToString(), regularFont, align: textAlignmentR, isCurrency:true));
                serviceTable.AddCell(PdfExportHelper.CreateCell(service.Quantity.ToString(), regularFont, align: textAlignmentC));
                serviceTable.AddCell(PdfExportHelper.CreateCell(service.Note??"", regularFont));
            }
            document.Add(serviceTable);
            document.Add(new Paragraph("\n"));
            
            // Payment information
            PdfExportHelper.AddPaymentRow(document, "Total table amount:", bill.TotalTableAmount, boldFont, regularFont);
            PdfExportHelper.AddPaymentRow(document, "Total service amount:", bill.TotalServiceAmount, boldFont, regularFont);
            PdfExportHelper.AddPaymentRow(document, "Additional cost:", bill.AdditionalCost, boldFont, regularFont);
            PdfExportHelper.AddPaymentRow(document, "Total invoice:", bill.TotalInvoiceAmount, boldFont, regularFont);
            PdfExportHelper.AddPaymentRow(document, "Penalty amount:", bill.PenaltyAmount, boldFont, regularFont);
            PdfExportHelper.AddPaymentRow(document, "Deposit:", bill.Deposit, boldFont, regularFont);
            PdfExportHelper.AddPaymentRow(document, "Remaining amount:", bill.RemainingAmount, boldFont, regularFont);

            document.Add(new LineSeparator(new SolidLine(1f)).SetMarginTop(15).SetMarginBottom(15));

            // Footer - invoice date
            document.Add(new Paragraph("Invoice date: " + DateTime.Now.ToString("dd'/'MM'/'yyyy"))
                .SetFont(italicFont)
                .SetFontSize(11)
                .SetTextAlignment(textAlignmentR)
                .SetMarginBottom(40));

            // Footer - signature
            document.Add(new Paragraph("Invoice issuer").SetFont(regularFont).SetTextAlignment(textAlignmentR).SetFontSize(11));
            document.Add(new Paragraph("(Signature)").SetFont(italicFont).SetTextAlignment(textAlignmentR).SetFontSize(10));

            document.Close();
        }
    }
}
