using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class LoaiSanhRepository : ILoaiSanhRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public LoaiSanhRepository()
        {
            _context = DataProvider.Ins.DB;
        }

        public IEnumerable<LOAISANH> GetAll()
        {
            return _context.LOAISANHs.ToList();
        }

        public LOAISANH GetById(int maLoaiSanh)
        {
            return _context.LOAISANHs.Find(maLoaiSanh);
        }

        public void Create(LOAISANH loaiSanh)
        {
            _context.LOAISANHs.Add(loaiSanh);
            _context.SaveChanges();
        }

        public void Update(LOAISANH loaiSanh)
        {
            var existing = _context.LOAISANHs.Find(loaiSanh.MaLoaiSanh);
            if (existing != null)
            {
                existing.TenLoaiSanh = loaiSanh.TenLoaiSanh;
                existing.DonGiaBanToiThieu = loaiSanh.DonGiaBanToiThieu;
                _context.SaveChanges();
            }
        }

        public void Delete(int maLoaiSanh)
        {
            var loaiSanh = _context.LOAISANHs.Find(maLoaiSanh);
            if (loaiSanh != null)
            {
                _context.LOAISANHs.Remove(loaiSanh);
                _context.SaveChanges();
            }
        }
    }
}