using System;

namespace QuanLyTiecCuoi.DataTransferObject
{
    public class MenuDTO
    {
        public int BookingId { get; set; }
        public int DishId { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string Note { get; set; }

        // Navigation properties
        public DishDTO Dish { get; set; }
        public BookingDTO Booking { get; set; }
    }
}