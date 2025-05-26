using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class NhomNguoiDungRepository : INhomNguoiDungRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public NhomNguoiDungRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<NHOMNGUOIDUNG> GetAll()
        {
            return _context.NHOMNGUOIDUNGs.ToList();
        }

        public NHOMNGUOIDUNG GetById(string maNhom)
        {
            return _context.NHOMNGUOIDUNGs.Find(maNhom);
        }

        public void Create(NHOMNGUOIDUNG nhomNguoiDung)
        {
            _context.NHOMNGUOIDUNGs.Add(nhomNguoiDung);
            _context.SaveChanges();
        }

        public void Update(NHOMNGUOIDUNG nhomNguoiDung)
        {
            var existing = _context.NHOMNGUOIDUNGs.Find(nhomNguoiDung.MaNhom);
            if (existing != null)
            {
                existing.TenNhom = nhomNguoiDung.TenNhom;
                // Nếu cần cập nhật các navigation property thì xử lý thêm ở đây
                _context.SaveChanges();
            }
        }

        public void Delete(string maNhom)
        {
            var nhom = _context.NHOMNGUOIDUNGs.Find(maNhom);
            if (nhom != null)
            {
                _context.NHOMNGUOIDUNGs.Remove(nhom);
                _context.SaveChanges();
            }
        }
    }
}