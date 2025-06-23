using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class DichVuService : IDichVuService
    {
        private readonly IDichVuRepository _dichVuRepository;

        public DichVuService()
        {
            _dichVuRepository = new DichVuRepository();
        }

        public IEnumerable<DICHVUDTO> GetAll()
        {
            return _dichVuRepository.GetAll()
                .Select(x => new DICHVUDTO
                {
                    MaDichVu = x.MaDichVu,
                    TenDichVu = x.TenDichVu,
                    DonGia = x.DonGia,
                    GhiChu = x.GhiChu
                });
        }

        public DICHVUDTO GetById(int maDichVu)
        {
            var entity = _dichVuRepository.GetById(maDichVu);
            if (entity == null) return null;
            return new DICHVUDTO
            {
                MaDichVu = entity.MaDichVu,
                TenDichVu = entity.TenDichVu,
                DonGia = entity.DonGia,
                GhiChu = entity.GhiChu
            };
        }

        public void Create(DICHVUDTO dichVuDto)
        {
            var entity = new DICHVU
            {
                MaDichVu = dichVuDto.MaDichVu,
                TenDichVu = dichVuDto.TenDichVu,
                DonGia = dichVuDto.DonGia,
                GhiChu = dichVuDto.GhiChu
            };
            _dichVuRepository.Create(entity);
            dichVuDto.MaDichVu = entity.MaDichVu; // Update DTO with generated ID
        }

        public void Update(DICHVUDTO dichVuDto)
        {
            var entity = new DICHVU
            {
                MaDichVu = dichVuDto.MaDichVu,
                TenDichVu = dichVuDto.TenDichVu,
                DonGia = dichVuDto.DonGia,
                GhiChu = dichVuDto.GhiChu
            };
            _dichVuRepository.Update(entity);
        }

        public void Delete(int maDichVu)
        {
            _dichVuRepository.Delete(maDichVu);
        }
    }
}