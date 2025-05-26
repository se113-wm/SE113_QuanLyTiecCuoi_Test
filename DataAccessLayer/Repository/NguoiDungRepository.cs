using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class NguoiDungRepository : INguoiDungRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public NguoiDungRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<NGUOIDUNG> GetAll()
        {
            return _context.NGUOIDUNGs.ToList();
        }

        public NGUOIDUNG GetById(int maNguoiDung)
        {
            return _context.NGUOIDUNGs.Find(maNguoiDung);
        }

        public void Create(NGUOIDUNG nguoiDung)
        {
            _context.NGUOIDUNGs.Add(nguoiDung);
            _context.SaveChanges();
        }

        public void Update(NGUOIDUNG nguoiDung)
        {
            var existing = _context.NGUOIDUNGs.Find(nguoiDung.MaNguoiDung);
            if (existing != null)
            {
                existing.TenDangNhap = nguoiDung.TenDangNhap;
                existing.MatKhauHash = nguoiDung.MatKhauHash;
                existing.HoTen = nguoiDung.HoTen;
                existing.Email = nguoiDung.Email;
                existing.TrangThai = nguoiDung.TrangThai;
                existing.MaNhom = nguoiDung.MaNhom;
                _context.SaveChanges();
            }
        }

        public void Delete(int maNguoiDung)
        {
            var nguoiDung = _context.NGUOIDUNGs.Find(maNguoiDung);
            if (nguoiDung != null)
            {
                _context.NGUOIDUNGs.Remove(nguoiDung);
                _context.SaveChanges();
            }
        }
    }
}