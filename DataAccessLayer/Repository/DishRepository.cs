using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class DishRepository : IDishRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public DishRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<Dish> GetAll()
        {
            return _context.Dishes.ToList();
        }

        public Dish GetById(int dishId)
        {
            return _context.Dishes.Find(dishId);
        }

        public void Create(Dish dish)
        {
            _context.Dishes.Add(dish);
            _context.SaveChanges();
            _context.Entry(dish).Reload();
        }

        public void Update(Dish dish)
        {
            var existing = _context.Dishes.Find(dish.DishId);
            if (existing != null)
            {
                existing.DishName = dish.DishName;
                existing.UnitPrice = dish.UnitPrice;
                existing.Note = dish.Note;
                _context.SaveChanges();
            }
        }

        public void Delete(int dishId)
        {
            var dish = _context.Dishes.Find(dishId);
            if (dish != null)
            {
                _context.Dishes.Remove(dish);
                _context.SaveChanges();
            }
        }
    }
}