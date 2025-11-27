using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class ShiftRepository : IShiftRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public ShiftRepository(QuanLyTiecCuoiEntities context)
        {
            _context = context ?? new QuanLyTiecCuoiEntities();
        }

        public ShiftRepository() : this(null)
        {
        }

        public IEnumerable<Shift> GetAll()
        {
            return _context.Shifts.ToList();
        }

        public Shift GetById(int shiftId)
        {
            return _context.Shifts.Find(shiftId);
        }

        public void Create(Shift shift)
        {
            _context.Shifts.Add(shift);
            _context.SaveChanges();
        }

        public void Update(Shift shift)
        {
            var existing = _context.Shifts.Find(shift.ShiftId);
            if (existing != null)
            {
                existing.ShiftName = shift.ShiftName;
                existing.StartTime = shift.StartTime;
                existing.EndTime = shift.EndTime;
                _context.SaveChanges();
            }
        }

        public void Delete(int shiftId)
        {
            var shift = _context.Shifts.Find(shiftId);
            if (shift != null)
            {
                _context.Shifts.Remove(shift);
                _context.SaveChanges();
            }
        }
    }
}