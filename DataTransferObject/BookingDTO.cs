using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class BookingDTO
    {
        public int BookingId { get; set; }
        public string GroomName { get; set; }
        public string BrideName { get; set; }
        public string Phone { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? WeddingDate { get; set; }
        public int? ShiftId { get; set; }
        public int? HallId { get; set; }
        public decimal? Deposit { get; set; }
        public int? TableCount { get; set; }
        public int? ReserveTableCount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal? TablePrice { get; set; }
        public decimal? TotalTableAmount { get; set; }
        public decimal? TotalServiceAmount { get; set; }
        public decimal? TotalInvoiceAmount { get; set; }
        public decimal? RemainingAmount { get; set; }
        public decimal? AdditionalCost { get; set; }
        public decimal? PenaltyAmount { get; set; }

        // Navigation properties
        public ShiftDTO Shift { get; set; }
        public HallDTO Hall { get; set; }

        public string Status
        {
            get
            {
                if (PaymentDate != null)
                    return "Paid";
                if (WeddingDate == null)
                    return "";
                var weddingDateValue = WeddingDate.Value.Date;
                var today = DateTime.Now.Date;
                if (weddingDateValue > today)
                    return "Not Organized";
                if (weddingDateValue == today)
                    return "Not Paid";
                if (weddingDateValue < today)
                    return "Late Payment";
                return "";
            }
        }

        public Brush StatusBrush
        {
            get
            {
                if (PaymentDate != null)
                    return Brushes.Green;
                if (WeddingDate == null)
                    return Brushes.Black;
                var weddingDateValue = WeddingDate.Value.Date;
                var today = DateTime.Now.Date;
                if (weddingDateValue > today)
                    return Brushes.Blue;
                if (weddingDateValue == today)
                    return Brushes.Orange;
                if (weddingDateValue < today)
                    return Brushes.Red;
                return Brushes.Black;
            }
        }
    }
}