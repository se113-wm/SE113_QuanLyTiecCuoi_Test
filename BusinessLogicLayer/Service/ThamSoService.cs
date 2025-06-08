using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class ThamSoService : IThamSoService
    {
        private readonly IThamSoRepository _thamSoRepository;

        public ThamSoService()
        {
            _thamSoRepository = new ThamSoRepository();
        }

        public IEnumerable<THAMSODTO> GetAll()
        {
            return _thamSoRepository.GetAll()
                .Select(x => new THAMSODTO
                {
                    TenThamSo = x.TenThamSo,
                    GiaTri = x.GiaTri
                });
        }

        public THAMSODTO GetByName(string tenThamSo)
        {
            var entity = _thamSoRepository.GetByName(tenThamSo);
            if (entity == null) return null;
            return new THAMSODTO
            {
                TenThamSo = entity.TenThamSo,
                GiaTri = entity.GiaTri
            };
        }

        public void Create(THAMSODTO thamSoDto)
        {
            var entity = new THAMSO
            {
                TenThamSo = thamSoDto.TenThamSo,
                GiaTri = thamSoDto.GiaTri
            };
            _thamSoRepository.Create(entity);
        }

        public void Update(THAMSODTO thamSoDto)
        {
            var entity = new THAMSO
            {
                TenThamSo = thamSoDto.TenThamSo,
                GiaTri = thamSoDto.GiaTri
            };
            _thamSoRepository.Update(entity);
        }

        public void Delete(string tenThamSo)
        {
            _thamSoRepository.Delete(tenThamSo);
        }
    }
}