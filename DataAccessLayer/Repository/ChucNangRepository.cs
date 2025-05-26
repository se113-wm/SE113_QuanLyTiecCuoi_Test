using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class ChucNangRepository : IChucNangRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public ChucNangRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<CHUCNANG> GetAll()
        {
            return _context.CHUCNANGs.ToList();
        }

        public CHUCNANG GetById(string maChucNang)
        {
            return _context.CHUCNANGs.Find(maChucNang);
        }

        public void Create(CHUCNANG chucNang)
        {
            _context.CHUCNANGs.Add(chucNang);
            _context.SaveChanges();
        }

        public void Update(CHUCNANG chucNang)
        {
            var existing = _context.CHUCNANGs.Find(chucNang.MaChucNang);
            if (existing != null)
            {
                existing.TenChucNang = chucNang.TenChucNang;
                existing.TenManHinhDuocLoad = chucNang.TenManHinhDuocLoad;
                // Nếu cần cập nhật navigation property thì xử lý thêm ở đây
                _context.SaveChanges();
            }
        }

        public void Delete(string maChucNang)
        {
            var chucNang = _context.CHUCNANGs.Find(maChucNang);
            if (chucNang != null)
            {
                _context.CHUCNANGs.Remove(chucNang);
                _context.SaveChanges();
            }
        }
    }
}