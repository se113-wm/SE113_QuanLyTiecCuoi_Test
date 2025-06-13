using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class SanhRepository : ISanhRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public SanhRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<SANH> GetAll()
        {
            return _context.SANHs.ToList();
        }

        public SANH GetById(int maSanh)
        {
            return _context.SANHs.Find(maSanh);
        }

        public void Create(SANH sanh)
        {
            _context.SANHs.Add(sanh);
            _context.SaveChanges();
            // reload sanh to get the generated ID if applicable
            _context.Entry(sanh).Reload();
        }

        public void Update(SANH sanh)
        {
            var existing = _context.SANHs.Find(sanh.MaSanh);
            if (existing != null)
            {
                existing.TenSanh = sanh.TenSanh;
                existing.MaLoaiSanh = sanh.MaLoaiSanh;
                existing.SoLuongBanToiDa = sanh.SoLuongBanToiDa;
                existing.GhiChu = sanh.GhiChu;
                _context.SaveChanges();
            }
        }

        public void Delete(int maSanh)
        {
            var sanh = _context.SANHs.Find(maSanh);
            if (sanh != null)
            {
                _context.SANHs.Remove(sanh);
                _context.SaveChanges();
            }
        }
    }
}