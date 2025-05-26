using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class DichVuRepository : IDichVuRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public DichVuRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<DICHVU> GetAll()
        {
            return _context.DICHVUs.ToList();
        }

        public DICHVU GetById(int maDichVu)
        {
            return _context.DICHVUs.Find(maDichVu);
        }

        public void Create(DICHVU dichVu)
        {
            _context.DICHVUs.Add(dichVu);
            _context.SaveChanges();
        }

        public void Update(DICHVU dichVu)
        {
            var existing = _context.DICHVUs.Find(dichVu.MaDichVu);
            if (existing != null)
            {
                existing.TenDichVu = dichVu.TenDichVu;
                existing.DonGia = dichVu.DonGia;
                existing.GhiChu = dichVu.GhiChu;
                _context.SaveChanges();
            }
        }

        public void Delete(int maDichVu)
        {
            var dichVu = _context.DICHVUs.Find(maDichVu);
            if (dichVu != null)
            {
                _context.DICHVUs.Remove(dichVu);
                _context.SaveChanges();
            }
        }
    }
}