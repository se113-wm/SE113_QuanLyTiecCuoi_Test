using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class MonAnService : IMonAnService
    {
        private readonly IMonAnRepository _monAnRepository;

        public MonAnService()
        {
            _monAnRepository = new MonAnRepository();
        }

        public IEnumerable<MONANDTO> GetAll()
        {
            return _monAnRepository.GetAll()
                .Select(x => new MONANDTO
                {
                    MaMonAn = x.MaMonAn,
                    TenMonAn = x.TenMonAn,
                    DonGia = x.DonGia,
                    GhiChu = x.GhiChu
                });
        }

        public MONANDTO GetById(int maMonAn)
        {
            var entity = _monAnRepository.GetById(maMonAn);
            if (entity == null) return null;
            return new MONANDTO
            {
                MaMonAn = entity.MaMonAn,
                TenMonAn = entity.TenMonAn,
                DonGia = entity.DonGia,
                GhiChu = entity.GhiChu
            };
        }

        public void Create(MONANDTO monAnDto)
        {
            var entity = new MONAN
            {
                MaMonAn = monAnDto.MaMonAn,
                TenMonAn = monAnDto.TenMonAn,
                DonGia = monAnDto.DonGia,
                GhiChu = monAnDto.GhiChu
            };
            _monAnRepository.Create(entity);
            monAnDto.MaMonAn = entity.MaMonAn; // Update DTO with generated ID
        }

        public void Update(MONANDTO monAnDto)
        {
            var entity = new MONAN
            {
                MaMonAn = monAnDto.MaMonAn,
                TenMonAn = monAnDto.TenMonAn,
                DonGia = monAnDto.DonGia,
                GhiChu = monAnDto.GhiChu
            };
            _monAnRepository.Update(entity);
        }

        public void Delete(int maMonAn)
        {
            _monAnRepository.Delete(maMonAn);
        }
    }
}