using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class ThucDonRepository : IThucDonRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public ThucDonRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<THUCDON> GetAll()
        {
            return _context.THUCDONs.ToList();
        }

        public THUCDON GetById(int maPhieuDat, int maMonAn)
        {
            return _context.THUCDONs.FirstOrDefault(x => x.MaPhieuDat == maPhieuDat && x.MaMonAn == maMonAn);
        }
        public IEnumerable<THUCDON> GetByMaPhieuDat(int maPhieuDat)
        {
            return _context.THUCDONs.Where(x => x.MaPhieuDat == maPhieuDat).ToList();
        }
        public void Create(THUCDON thucDon)
        {
            _context.THUCDONs.Add(thucDon);
            _context.SaveChanges();
        }

        public void Update(THUCDON thucDon)
        {
            var existing = _context.THUCDONs.FirstOrDefault(x => x.MaPhieuDat == thucDon.MaPhieuDat && x.MaMonAn == thucDon.MaMonAn);
            if (existing != null)
            {
                existing.SoLuong = thucDon.SoLuong;
                existing.DonGia = thucDon.DonGia;
                existing.ThuTuLenMon = thucDon.ThuTuLenMon;
                existing.GhiChu = thucDon.GhiChu;
                _context.SaveChanges();
            }
        }

        public void Delete(int maPhieuDat, int maMonAn)
        {
            var thucDon = _context.THUCDONs.FirstOrDefault(x => x.MaPhieuDat == maPhieuDat && x.MaMonAn == maMonAn);
            if (thucDon != null)
            {
                _context.THUCDONs.Remove(thucDon);
                _context.SaveChanges();
            }
        }
    }
}