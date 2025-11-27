using System;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class ServiceDetailDTO
    {
        public int BookingId { get; set; }
        public int ServiceId { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalAmount { get; set; }
        public string Note { get; set; }

        // Navigation properties
        public ServiceDTO Service { get; set; }
        public BookingDTO Booking { get; set; }
    }
}