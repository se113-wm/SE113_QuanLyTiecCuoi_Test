using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class ThamSoRepository : IThamSoRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public ThamSoRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<THAMSO> GetAll()
        {
            return _context.THAMSOes.ToList();
        }

        public THAMSO GetByName(string tenThamSo)
        {
            return _context.THAMSOes.FirstOrDefault(ts => ts.TenThamSo == tenThamSo);
        }

        public void Update(THAMSO thamSo)
        {
            var existing = _context.THAMSOes.FirstOrDefault(ts => ts.TenThamSo == thamSo.TenThamSo);
            if (existing != null)
            {
                existing.GiaTri = thamSo.GiaTri;
                _context.SaveChanges();
            }
        }
    }
}