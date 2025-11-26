using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class CaService : ICaService
    {
        private readonly ICaRepository _caRepository;

        // Constructor với Dependency Injection
        public CaService(ICaRepository caRepository)
        {
            _caRepository = caRepository;
        }

        public IEnumerable<CADTO> GetAll()
        {
            return _caRepository.GetAll()
                .Select(x => new CADTO
                {
                    MaCa = x.MaCa,
                    TenCa = x.TenCa,
                    ThoiGianBatDauCa = x.ThoiGianBatDauCa,
                    ThoiGianKetThucCa = x.ThoiGianKetThucCa
                });
        }

        public CADTO GetById(int maCa)
        {
            var entity = _caRepository.GetById(maCa);
            if (entity == null) return null;
            return new CADTO
            {
                MaCa = entity.MaCa,
                TenCa = entity.TenCa,
                ThoiGianBatDauCa = entity.ThoiGianBatDauCa,
                ThoiGianKetThucCa = entity.ThoiGianKetThucCa
            };
        }

        public void Create(CADTO caDto)
        {
            var entity = new CA
            {
                MaCa = caDto.MaCa,
                TenCa = caDto.TenCa,
                ThoiGianBatDauCa = caDto.ThoiGianBatDauCa,
                ThoiGianKetThucCa = caDto.ThoiGianKetThucCa
            };
            _caRepository.Create(entity);
        }

        public void Update(CADTO caDto)
        {
            var entity = new CA
            {
                MaCa = caDto.MaCa,
                TenCa = caDto.TenCa,
                ThoiGianBatDauCa = caDto.ThoiGianBatDauCa,
                ThoiGianKetThucCa = caDto.ThoiGianKetThucCa
            };
            _caRepository.Update(entity);
        }

        public void Delete(int maCa)
        {
            _caRepository.Delete(maCa);
        }
    }
}