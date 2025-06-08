using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class ThucDonService : IThucDonService
    {
        private readonly IThucDonRepository _thucDonRepository;

        public ThucDonService()
        {
            _thucDonRepository = new ThucDonRepository();
        }

        public IEnumerable<THUCDONDTO> GetAll()
        {
            return _thucDonRepository.GetAll()
                .Select(x => MapToDto(x));
        }

        public IEnumerable<THUCDONDTO> GetByPhieuDat(int maPhieuDat)
        {
            return _thucDonRepository.GetByMaPhieuDat(maPhieuDat)
                .Select(x => MapToDto(x));
        }

        public THUCDONDTO GetById(int maPhieuDat, int maMonAn)
        {
            var entity = _thucDonRepository.GetById(maPhieuDat, maMonAn);
            return entity == null ? null : MapToDto(entity);
        }

        public void Create(THUCDONDTO thucDonDto)
        {
            var entity = MapToEntity(thucDonDto);
            _thucDonRepository.Create(entity);
        }

        public void Update(THUCDONDTO thucDonDto)
        {
            var entity = MapToEntity(thucDonDto);
            _thucDonRepository.Update(entity);
        }

        public void Delete(int maPhieuDat, int maMonAn)
        {
            _thucDonRepository.Delete(maPhieuDat, maMonAn);
        }

        private static THUCDONDTO MapToDto(THUCDON x)
        {
            return new THUCDONDTO
            {
                MaPhieuDat = x.MaPhieuDat,
                MaMonAn = x.MaMonAn,
                SoLuong = x.SoLuong,
                DonGia = x.DonGia,
                ThuTuLenMon = x.ThuTuLenMon,
                GhiChu = x.GhiChu,
                MonAn = x.MONAN != null
                    ? new MONANDTO
                    {
                        MaMonAn = x.MONAN.MaMonAn,
                        TenMonAn = x.MONAN.TenMonAn,
                        DonGia = x.MONAN.DonGia
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

        private static THUCDON MapToEntity(THUCDONDTO dto)
        {
            return new THUCDON
            {
                MaPhieuDat = dto.MaPhieuDat,
                MaMonAn = dto.MaMonAn,
                SoLuong = dto.SoLuong,
                DonGia = dto.DonGia,
                ThuTuLenMon = dto.ThuTuLenMon ?? 0,
                GhiChu = dto.GhiChu
            };
        }
    }
}