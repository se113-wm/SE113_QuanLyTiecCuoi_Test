using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class MonAnRepository : IMonAnRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public MonAnRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<MONAN> GetAll()
        {
            return _context.MONANs.ToList();
        }

        public MONAN GetById(int maMonAn)
        {
            return _context.MONANs.Find(maMonAn);
        }

        public void Create(MONAN monAn)
        {
            _context.MONANs.Add(monAn);
            _context.SaveChanges();
            _context.Entry(monAn).Reload(); // Reload to get the latest values from the database
        }

        public void Update(MONAN monAn)
        {
            var existing = _context.MONANs.Find(monAn.MaMonAn);
            if (existing != null)
            {
                existing.TenMonAn = monAn.TenMonAn;
                existing.DonGia = monAn.DonGia;
                existing.GhiChu = monAn.GhiChu;
                _context.SaveChanges();
            }
        }

        public void Delete(int maMonAn)
        {
            var monAn = _context.MONANs.Find(maMonAn);
            if (monAn != null)
            {
                _context.MONANs.Remove(monAn);
                _context.SaveChanges();
            }
        }
    }
}