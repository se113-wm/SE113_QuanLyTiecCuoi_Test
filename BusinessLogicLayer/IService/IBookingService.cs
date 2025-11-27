using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IBookingService
    {
        IEnumerable<BookingDTO> GetAll();
        BookingDTO GetById(int bookingId);
        void Create(BookingDTO bookingDto);
        void Update(BookingDTO bookingDto);
        void Delete(int bookingId);
    }
}