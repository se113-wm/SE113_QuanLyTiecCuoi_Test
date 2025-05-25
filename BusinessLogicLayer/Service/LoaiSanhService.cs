using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;
using QuanLyTiecCuoi.DataAccessLayer.Repository;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class LoaiSanhService : ILoaiSanhService
    {
        private readonly ILoaiSanhRepository _loaiSanhRepository;

        public LoaiSanhService()
        {
            _loaiSanhRepository = new LoaiSanhRepository();
        }

        public IEnumerable<LOAISANHDTO> GetAll()
        {
            return _loaiSanhRepository.GetAll()
                .Select(x => new LOAISANHDTO
                {
                    MaLoaiSanh = x.MaLoaiSanh,
                    TenLoaiSanh = x.TenLoaiSanh,
                    DonGiaBanToiThieu = x.DonGiaBanToiThieu
                });
        }

        public LOAISANHDTO GetById(int maLoaiSanh)
        {
            var entity = _loaiSanhRepository.GetById(maLoaiSanh);
            if (entity == null) return null;
            return new LOAISANHDTO
            {
                MaLoaiSanh = entity.MaLoaiSanh,
                TenLoaiSanh = entity.TenLoaiSanh,
                DonGiaBanToiThieu = entity.DonGiaBanToiThieu
            };
        }

        public void Create(LOAISANHDTO loaiSanhDto)
        {
            var entity = new LOAISANH
            {
                MaLoaiSanh = loaiSanhDto.MaLoaiSanh,
                TenLoaiSanh = loaiSanhDto.TenLoaiSanh,
                DonGiaBanToiThieu = loaiSanhDto.DonGiaBanToiThieu
            };
            _loaiSanhRepository.Create(entity);
        }

        public void Update(LOAISANHDTO loaiSanhDto)
        {
            var entity = new LOAISANH
            {
                MaLoaiSanh = loaiSanhDto.MaLoaiSanh,
                TenLoaiSanh = loaiSanhDto.TenLoaiSanh,
                DonGiaBanToiThieu = loaiSanhDto.DonGiaBanToiThieu
            };
            _loaiSanhRepository.Update(entity);
        }

        public void Delete(int maLoaiSanh)
        {
            _loaiSanhRepository.Delete(maLoaiSanh);
        }
    }
}