using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class MenuRepository : IMenuRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public MenuRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<Menu> GetAll()
        {
            return _context.Menus.ToList();
        }

        public Menu GetById(int bookingId, int dishId)
        {
            return _context.Menus.FirstOrDefault(x => x.BookingId == bookingId && x.DishId == dishId);
        }

        public IEnumerable<Menu> GetByBookingId(int bookingId)
        {
            return _context.Menus.Where(x => x.BookingId == bookingId).ToList();
        }

        public void Create(Menu menu)
        {
            _context.Menus.Add(menu);
            _context.SaveChanges();
        }

        public void Update(Menu menu)
        {
            var existing = _context.Menus.FirstOrDefault(x => x.BookingId == menu.BookingId && x.DishId == menu.DishId);
            if (existing != null)
            {
                existing.Quantity = menu.Quantity;
                existing.UnitPrice = menu.UnitPrice;
                existing.Note = menu.Note;
                _context.SaveChanges();
            }
        }

        public void Delete(int bookingId, int dishId)
        {
            var menu = _context.Menus.FirstOrDefault(x => x.BookingId == bookingId && x.DishId == dishId);
            if (menu != null)
            {
                _context.Menus.Remove(menu);
                _context.SaveChanges();
            }
        }
    }
}