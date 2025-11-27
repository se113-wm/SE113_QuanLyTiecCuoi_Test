using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IBookingRepository
    {
        IEnumerable<Booking> GetAll();
        Booking GetById(int bookingId);
        void Create(Booking booking);
        void Update(Booking booking);
        void Delete(int bookingId);
    }
}