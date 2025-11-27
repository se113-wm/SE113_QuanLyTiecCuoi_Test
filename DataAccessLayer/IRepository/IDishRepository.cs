using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IDishRepository
    {
        IEnumerable<Dish> GetAll();
        Dish GetById(int dishId);
        void Create(Dish dish);
        void Update(Dish dish);
        void Delete(int dishId);
    }
}