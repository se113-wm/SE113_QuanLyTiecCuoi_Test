using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class HallRepository : IHallRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public HallRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<Hall> GetAll()
        {
            return _context.Halls.Include(h => h.HallType).ToList();
        }

        public Hall GetById(int hallId)
        {
            return _context.Halls.Include(h => h.HallType).FirstOrDefault(h => h.HallId == hallId);
        }

        public void Create(Hall hall)
        {
            _context.Halls.Add(hall);
            _context.SaveChanges();
            // reload hall to get the generated ID if applicable
            _context.Entry(hall).Reload();
        }

        public void Update(Hall hall)
        {
            var existing = _context.Halls.Find(hall.HallId);
            if (existing != null)
            {
                existing.HallName = hall.HallName;
                existing.HallTypeId = hall.HallTypeId;
                existing.MaxTableCount = hall.MaxTableCount;
                existing.Note = hall.Note;
                _context.SaveChanges();
            }
        }

        public void Delete(int hallId)
        {
            var hall = _context.Halls.Find(hallId);
            if (hall != null)
            {
                _context.Halls.Remove(hall);
                _context.SaveChanges();
            }
        }
    }
}