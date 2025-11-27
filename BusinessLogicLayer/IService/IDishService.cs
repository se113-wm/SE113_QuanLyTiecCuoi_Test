using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IDishService
    {
        IEnumerable<DishDTO> GetAll();
        DishDTO GetById(int dishId);
        void Create(DishDTO dishDTO);
        void Update(DishDTO dishDTO);
        void Delete(int dishId);
    }
}