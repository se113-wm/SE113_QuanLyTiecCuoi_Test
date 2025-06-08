using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class ChiTietDVService : IChiTietDVService
    {
        private readonly IChiTietDVRepository _chiTietDVRepository;

        public ChiTietDVService()
        {
            _chiTietDVRepository = new ChiTietDVRepository();
        }

        public IEnumerable<CHITIETDVDTO> GetAll()
        {
            return _chiTietDVRepository.GetAll()
                .Select(x => MapToDto(x));
        }

        public IEnumerable<CHITIETDVDTO> GetByPhieuDat(int maPhieuDat)
        {
            return _chiTietDVRepository.GetByMaPhieuDat(maPhieuDat)
                .Select(x => MapToDto(x));
        }

        public CHITIETDVDTO GetById(int maPhieuDat, int maDichVu)
        {
            var entity = _chiTietDVRepository.GetById(maPhieuDat, maDichVu);
            return entity == null ? null : MapToDto(entity);
        }

        public void Create(CHITIETDVDTO chiTietDVDto)
        {
            var entity = MapToEntity(chiTietDVDto);
            _chiTietDVRepository.Create(entity);
        }

        public void Update(CHITIETDVDTO chiTietDVDto)
        {
            var entity = MapToEntity(chiTietDVDto);
            _chiTietDVRepository.Update(entity);
        }

        public void Delete(int maPhieuDat, int maDichVu)
        {
            _chiTietDVRepository.Delete(maPhieuDat, maDichVu);
        }

        private static CHITIETDVDTO MapToDto(CHITIETDV x)
        {
            return new CHITIETDVDTO
            {
                MaPhieuDat = x.MaPhieuDat,
                MaDichVu = x.MaDichVu,
                SoLuong = x.SoLuong,
                DonGia = x.DonGia,
                ThanhTien = x.ThanhTien,
                GhiChu = x.GhiChu,
                DichVu = x.DICHVU != null
                    ? new DICHVUDTO
                    {
                        MaDichVu = x.DICHVU.MaDichVu,
                        TenDichVu = x.DICHVU.TenDichVu,
                        DonGia = x.DICHVU.DonGia,
                        GhiChu = x.DICHVU.GhiChu
                        // Thêm các thuộc tính khác nếu cần
                    }
                    : null,
                PhieuDatTiec = x.PHIEUDATTIEC != null
                    ? new PHIEUDATTIECDTO
                    {
                        MaPhieuDat = x.PHIEUDATTIEC.MaPhieuDat,
                        // Thêm các thuộc tính khác nếu cần
                    }
                    : null
            };
        }

        private static CHITIETDV MapToEntity(CHITIETDVDTO dto)
        {
            return new CHITIETDV
            {
                MaPhieuDat = dto.MaPhieuDat,
                MaDichVu = dto.MaDichVu,
                SoLuong = dto.SoLuong,
                DonGia = dto.DonGia,
                ThanhTien = dto.ThanhTien,
                GhiChu = dto.GhiChu
            };
        }
    }
}