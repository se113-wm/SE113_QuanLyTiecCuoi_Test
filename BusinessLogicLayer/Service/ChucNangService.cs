using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class ChucNangService : IChucNangService
    {
        private readonly IChucNangRepository _chucNangRepository;

        public ChucNangService()
        {
            _chucNangRepository = new ChucNangRepository();
        }

        public IEnumerable<CHUCNANGDTO> GetAll()
        {
            return _chucNangRepository.GetAll()
                .Select(x => new CHUCNANGDTO
                {
                    MaChucNang = x.MaChucNang,
                    TenChucNang = x.TenChucNang,
                    TenManHinhDuocLoad = x.TenManHinhDuocLoad
                });
        }

        public CHUCNANGDTO GetById(string maChucNang)
        {
            var entity = _chucNangRepository.GetById(maChucNang);
            if (entity == null) return null;
            return new CHUCNANGDTO
            {
                MaChucNang = entity.MaChucNang,
                TenChucNang = entity.TenChucNang,
                TenManHinhDuocLoad = entity.TenManHinhDuocLoad
            };
        }

        public void Create(CHUCNANGDTO chucNangDto)
        {
            var entity = new CHUCNANG
            {
                MaChucNang = chucNangDto.MaChucNang,
                TenChucNang = chucNangDto.TenChucNang,
                TenManHinhDuocLoad = chucNangDto.TenManHinhDuocLoad
            };
            _chucNangRepository.Create(entity);
        }

        public void Update(CHUCNANGDTO chucNangDto)
        {
            var entity = new CHUCNANG
            {
                MaChucNang = chucNangDto.MaChucNang,
                TenChucNang = chucNangDto.TenChucNang,
                TenManHinhDuocLoad = chucNangDto.TenManHinhDuocLoad
            };
            _chucNangRepository.Update(entity);
        }

        public void Delete(string maChucNang)
        {
            _chucNangRepository.Delete(maChucNang);
        }
    }
}