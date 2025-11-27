using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class HallTypeRepository : IHallTypeRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public HallTypeRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<HallType> GetAll()
        {
            return _context.HallTypes.ToList();
        }

        public HallType GetById(int hallTypeId)
        {
            return _context.HallTypes.Find(hallTypeId);
        }

        public void Create(HallType hallType)
        {
            _context.HallTypes.Add(hallType);
            _context.SaveChanges();
        }

        public void Update(HallType hallType)
        {
            var existing = _context.HallTypes.Find(hallType.HallTypeId);
            if (existing != null)
            {
                existing.HallTypeName = hallType.HallTypeName;
                existing.MinTablePrice = hallType.MinTablePrice;
                _context.SaveChanges();
            }
        }

        public void Delete(int hallTypeId)
        {
            var hallType = _context.HallTypes.Find(hallTypeId);
            if (hallType != null)
            {
                _context.HallTypes.Remove(hallType);
                _context.SaveChanges();
            }
        }
    }
}