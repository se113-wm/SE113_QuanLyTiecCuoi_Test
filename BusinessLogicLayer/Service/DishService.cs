using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class DishService : IDishService
    {
        private readonly IDishRepository _dishRepository;

        // Constructor với Dependency Injection
        public DishService(IDishRepository dishRepository)
        {
            _dishRepository = dishRepository;
        }

        public IEnumerable<DishDTO> GetAll()
        {
            return _dishRepository.GetAll()
                .Select(x => new DishDTO
                {
                    DishId = x.DishId,
                    DishName = x.DishName,
                    UnitPrice = x.UnitPrice,
                    Note = x.Note
                });
        }

        public DishDTO GetById(int dishId)
        {
            var entity = _dishRepository.GetById(dishId);
            if (entity == null) return null;
            return new DishDTO
            {
                DishId = entity.DishId,
                DishName = entity.DishName,
                UnitPrice = entity.UnitPrice,
                Note = entity.Note
            };
        }

        public void Create(DishDTO dishDTO)
        {
            var entity = new Dish
            {
                DishId = dishDTO.DishId,
                DishName = dishDTO.DishName,
                UnitPrice = dishDTO.UnitPrice,
                Note = dishDTO.Note
            };
            _dishRepository.Create(entity);
            dishDTO.DishId = entity.DishId; // Update DTO with generated ID
        }

        public void Update(DishDTO dishDTO)
        {
            var entity = new Dish
            {
                DishId = dishDTO.DishId,
                DishName = dishDTO.DishName,
                UnitPrice = dishDTO.UnitPrice,
                Note = dishDTO.Note
            };
            _dishRepository.Update(entity);
        }

        public void Delete(int dishId)
        {
            _dishRepository.Delete(dishId);
        }
    }
}