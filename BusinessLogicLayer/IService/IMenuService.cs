using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IMenuService
    {
        IEnumerable<MenuDTO> GetAll();
        IEnumerable<MenuDTO> GetByBookingId(int bookingId);
        MenuDTO GetById(int bookingId, int dishId);
        void Create(MenuDTO menuDTO);
        void Update(MenuDTO menuDTO);
        void Delete(int bookingId, int dishId);
    }
}