using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class ChiTietDVRepository : IChiTietDVRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public ChiTietDVRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<CHITIETDV> GetAll()
        {
            return _context.CHITIETDVs.ToList();
        }

        public CHITIETDV GetById(int maPhieuDat, int maDichVu)
        {
            return _context.CHITIETDVs.FirstOrDefault(x => x.MaPhieuDat == maPhieuDat && x.MaDichVu == maDichVu);
        }

        public IEnumerable<CHITIETDV> GetByMaPhieuDat(int maPhieuDat)
        {
            return _context.CHITIETDVs.Where(x => x.MaPhieuDat == maPhieuDat).ToList();
        }

        public void Create(CHITIETDV chiTietDV)
        {
            _context.CHITIETDVs.Add(chiTietDV);
            _context.SaveChanges();
        }

        public void Update(CHITIETDV chiTietDV)
        {
            var existing = _context.CHITIETDVs.FirstOrDefault(x => x.MaPhieuDat == chiTietDV.MaPhieuDat && x.MaDichVu == chiTietDV.MaDichVu);
            if (existing != null)
            {
                existing.SoLuong = chiTietDV.SoLuong;
                existing.DonGia = chiTietDV.DonGia;
                existing.ThanhTien = chiTietDV.ThanhTien;
                existing.GhiChu = chiTietDV.GhiChu;
                _context.SaveChanges();
            }
        }

        public void Delete(int maPhieuDat, int maDichVu)
        {
            var chiTietDV = _context.CHITIETDVs.FirstOrDefault(x => x.MaPhieuDat == maPhieuDat && x.MaDichVu == maDichVu);
            if (chiTietDV != null)
            {
                _context.CHITIETDVs.Remove(chiTietDV);
                _context.SaveChanges();
            }
        }
    }
}