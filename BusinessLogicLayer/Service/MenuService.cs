using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public IEnumerable<MenuDTO> GetAll()
        {
            return _menuRepository.GetAll()
                .Select(x => MapToDto(x));
        }

        public IEnumerable<MenuDTO> GetByBookingId(int bookingId)
        {
            return _menuRepository.GetByBookingId(bookingId)
                .Select(x => MapToDto(x));
        }

        public MenuDTO GetById(int bookingId, int dishId)
        {
            var entity = _menuRepository.GetById(bookingId, dishId);
            return entity == null ? null : MapToDto(entity);
        }

        public void Create(MenuDTO menuDTO)
        {
            var entity = MapToEntity(menuDTO);
            _menuRepository.Create(entity);
        }

        public void Update(MenuDTO menuDTO)
        {
            var entity = MapToEntity(menuDTO);
            _menuRepository.Update(entity);
        }

        public void Delete(int bookingId, int dishId)
        {
            _menuRepository.Delete(bookingId, dishId);
        }

        private static MenuDTO MapToDto(Menu x)
        {
            return new MenuDTO
            {
                BookingId = x.BookingId,
                DishId = x.DishId,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                Note = x.Note,
                Dish = x.Dish != null
                    ? new DishDTO
                    {
                        DishId = x.Dish.DishId,
                        DishName = x.Dish.DishName,
                        UnitPrice = x.Dish.UnitPrice
                    }
                    : null,
                Booking = x.Booking != null
                    ? new BookingDTO
                    {
                        BookingId = x.Booking.BookingId,
                    }
                    : null
            };
        }

        private static Menu MapToEntity(MenuDTO dto)
        {
            return new Menu
            {
                BookingId = dto.BookingId,
                DishId = dto.DishId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                Note = dto.Note
            };
        }
    }
}