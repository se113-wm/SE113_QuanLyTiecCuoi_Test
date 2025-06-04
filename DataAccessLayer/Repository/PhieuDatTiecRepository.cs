using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class PhieuDatTiecRepository : IPhieuDatTiecRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public PhieuDatTiecRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<PHIEUDATTIEC> GetAll()
        {
            return _context.PHIEUDATTIECs.ToList();
        }

        public PHIEUDATTIEC GetById(int maPhieuDat)
        {
            return _context.PHIEUDATTIECs.Find(maPhieuDat);
        }

        public void Create(PHIEUDATTIEC phieuDatTiec)
        {
            _context.PHIEUDATTIECs.Add(phieuDatTiec);
            _context.SaveChanges();
            _context.Entry(phieuDatTiec).Reload(); // Reload để lấy giá trị mới nhất từ CSDL
        }

        public void Update(PHIEUDATTIEC phieuDatTiec)
        {
            var existing = _context.PHIEUDATTIECs.Find(phieuDatTiec.MaPhieuDat);
            if (existing != null)
            {
                existing.TenChuRe = phieuDatTiec.TenChuRe;
                existing.TenCoDau = phieuDatTiec.TenCoDau;
                existing.DienThoai = phieuDatTiec.DienThoai;
                existing.NgayDatTiec = phieuDatTiec.NgayDatTiec;
                existing.NgayDaiTiec = phieuDatTiec.NgayDaiTiec;
                existing.MaCa = phieuDatTiec.MaCa;
                existing.MaSanh = phieuDatTiec.MaSanh;
                existing.TienDatCoc = phieuDatTiec.TienDatCoc;
                existing.SoLuongBan = phieuDatTiec.SoLuongBan;
                existing.SoBanDuTru = phieuDatTiec.SoBanDuTru;
                existing.NgayThanhToan = phieuDatTiec.NgayThanhToan;
                existing.DonGiaBanTiec = phieuDatTiec.DonGiaBanTiec;
                existing.TongTienBan = phieuDatTiec.TongTienBan;
                existing.TongTienDV = phieuDatTiec.TongTienDV;
                existing.TongTienHoaDon = phieuDatTiec.TongTienHoaDon;
                existing.TienConLai = phieuDatTiec.TienConLai;
                existing.ChiPhiPhatSinh = phieuDatTiec.ChiPhiPhatSinh;
                existing.TienPhat = phieuDatTiec.TienPhat;
                existing.MaCa = phieuDatTiec.MaCa;
                existing.MaSanh = phieuDatTiec.MaSanh;
                _context.SaveChanges();
                _context.Entry(existing).Reload(); // Reload để lấy giá trị mới nhất từ CSDL
            }
        }

        public void Delete(int maPhieuDat)
        {
            var phieu = _context.PHIEUDATTIECs.Find(maPhieuDat);
            if (phieu != null)
            {
                _context.PHIEUDATTIECs.Remove(phieu);
                _context.SaveChanges();
            }
        }
    }
}