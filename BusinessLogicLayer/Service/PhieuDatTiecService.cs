using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class PhieuDatTiecService : IPhieuDatTiecService
    {
        private readonly IPhieuDatTiecRepository _phieuDatTiecRepository;

        public PhieuDatTiecService()
        {
            _phieuDatTiecRepository = new PhieuDatTiecRepository();
        }

        public IEnumerable<PHIEUDATTIECDTO> GetAll()
        {
            return _phieuDatTiecRepository.GetAll()
                .Select(x => new PHIEUDATTIECDTO
                {
                    MaPhieuDat = x.MaPhieuDat,
                    TenChuRe = x.TenChuRe,
                    TenCoDau = x.TenCoDau,
                    DienThoai = x.DienThoai,
                    NgayDatTiec = x.NgayDatTiec,
                    NgayDaiTiec = x.NgayDaiTiec,
                    MaCa = x.MaCa,
                    MaSanh = x.MaSanh,
                    TienDatCoc = x.TienDatCoc,
                    SoLuongBan = x.SoLuongBan,
                    SoBanDuTru = x.SoBanDuTru,
                    NgayThanhToan = x.NgayThanhToan,
                    DonGiaBanTiec = x.DonGiaBanTiec,
                    TongTienBan = x.TongTienBan,
                    TongTienDV = x.TongTienDV,
                    TongTienHoaDon = x.TongTienHoaDon,
                    TienConLai = x.TienConLai,
                    ChiPhiPhatSinh = x.ChiPhiPhatSinh,
                    TienPhat = x.TienPhat,
                    Ca = x.CA != null
                        ? new CADTO
                        {
                            MaCa = x.CA.MaCa,
                            TenCa = x.CA.TenCa,
                            ThoiGianBatDauCa = x.CA.ThoiGianBatDauCa,
                            ThoiGianKetThucCa = x.CA.ThoiGianKetThucCa
                        }
                        : null,
                    Sanh = x.SANH != null
                        ? new SANHDTO
                        {
                            MaSanh = x.SANH.MaSanh,
                            MaLoaiSanh = x.SANH.MaLoaiSanh,
                            TenSanh = x.SANH.TenSanh,
                            SoLuongBanToiDa = x.SANH.SoLuongBanToiDa,
                            GhiChu = x.SANH.GhiChu
                        }
                        : null
                });
        }

        public PHIEUDATTIECDTO GetById(int maPhieuDat)
        {
            var x = _phieuDatTiecRepository.GetById(maPhieuDat);
            if (x == null) return null;
            return new PHIEUDATTIECDTO
            {
                MaPhieuDat = x.MaPhieuDat,
                TenChuRe = x.TenChuRe,
                TenCoDau = x.TenCoDau,
                DienThoai = x.DienThoai,
                NgayDatTiec = x.NgayDatTiec,
                NgayDaiTiec = x.NgayDaiTiec,
                MaCa = x.MaCa,
                MaSanh = x.MaSanh,
                TienDatCoc = x.TienDatCoc,
                SoLuongBan = x.SoLuongBan,
                SoBanDuTru = x.SoBanDuTru,
                NgayThanhToan = x.NgayThanhToan,
                DonGiaBanTiec = x.DonGiaBanTiec,
                TongTienBan = x.TongTienBan,
                TongTienDV = x.TongTienDV,
                TongTienHoaDon = x.TongTienHoaDon,
                TienConLai = x.TienConLai,
                ChiPhiPhatSinh = x.ChiPhiPhatSinh,
                TienPhat = x.TienPhat,
                Ca = x.CA != null
                    ? new CADTO
                    {
                        MaCa = x.CA.MaCa,
                        TenCa = x.CA.TenCa,
                        ThoiGianBatDauCa = x.CA.ThoiGianBatDauCa,
                        ThoiGianKetThucCa = x.CA.ThoiGianKetThucCa
                    }
                    : null,
                Sanh = x.SANH != null
                    ? new SANHDTO
                    {
                        MaSanh = x.SANH.MaSanh,
                        MaLoaiSanh = x.SANH.MaLoaiSanh,
                        TenSanh = x.SANH.TenSanh,
                        SoLuongBanToiDa = x.SANH.SoLuongBanToiDa,
                        GhiChu = x.SANH.GhiChu
                    }
                    : null
            };
        }

        public void Create(PHIEUDATTIECDTO dto)
        {
            var entity = new PHIEUDATTIEC
            {
                MaPhieuDat = dto.MaPhieuDat,
                TenChuRe = dto.TenChuRe,
                TenCoDau = dto.TenCoDau,
                DienThoai = dto.DienThoai,
                NgayDatTiec = dto.NgayDatTiec,
                NgayDaiTiec = dto.NgayDaiTiec,
                MaCa = dto.MaCa,
                MaSanh = dto.MaSanh,
                TienDatCoc = dto.TienDatCoc,
                SoLuongBan = dto.SoLuongBan,
                SoBanDuTru = dto.SoBanDuTru,
                NgayThanhToan = dto.NgayThanhToan,
                DonGiaBanTiec = dto.DonGiaBanTiec,
                TongTienBan = dto.TongTienBan,
                TongTienDV = dto.TongTienDV,
                TongTienHoaDon = dto.TongTienHoaDon,
                TienConLai = dto.TienConLai,
                ChiPhiPhatSinh = dto.ChiPhiPhatSinh,
                TienPhat = dto.TienPhat
            };
            _phieuDatTiecRepository.Create(entity);
        }

        public void Update(PHIEUDATTIECDTO dto)
        {
            var entity = new PHIEUDATTIEC
            {
                MaPhieuDat = dto.MaPhieuDat,
                TenChuRe = dto.TenChuRe,
                TenCoDau = dto.TenCoDau,
                DienThoai = dto.DienThoai,
                NgayDatTiec = dto.NgayDatTiec,
                NgayDaiTiec = dto.NgayDaiTiec,
                MaCa = dto.MaCa,
                MaSanh = dto.MaSanh,
                TienDatCoc = dto.TienDatCoc,
                SoLuongBan = dto.SoLuongBan,
                SoBanDuTru = dto.SoBanDuTru,
                NgayThanhToan = dto.NgayThanhToan,
                DonGiaBanTiec = dto.DonGiaBanTiec,
                TongTienBan = dto.TongTienBan,
                TongTienDV = dto.TongTienDV,
                TongTienHoaDon = dto.TongTienHoaDon,
                TienConLai = dto.TienConLai,
                ChiPhiPhatSinh = dto.ChiPhiPhatSinh,
                TienPhat = dto.TienPhat
            };
            _phieuDatTiecRepository.Update(entity);
        }

        public void Delete(int maPhieuDat)
        {
            _phieuDatTiecRepository.Delete(maPhieuDat);
        }
    }
}