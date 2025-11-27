using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IMenuRepository
    {
        IEnumerable<Menu> GetAll();
        Menu GetById(int bookingId, int dishId);
        IEnumerable<Menu> GetByBookingId(int bookingId);
        void Create(Menu menu);
        void Update(Menu menu);
        void Delete(int bookingId, int dishId);
    }
}