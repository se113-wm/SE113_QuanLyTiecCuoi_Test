using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class HallTypeService : IHallTypeService
    {
        private readonly IHallTypeRepository _hallTypeRepository;

        public HallTypeService(IHallTypeRepository hallTypeRepository)
        {
            _hallTypeRepository = hallTypeRepository;
        }

        public IEnumerable<HallTypeDTO> GetAll()
        {
            return _hallTypeRepository.GetAll()
                .Select(x => new HallTypeDTO
                {
                    HallTypeId = x.HallTypeId,
                    HallTypeName = x.HallTypeName,
                    MinTablePrice = x.MinTablePrice
                });
        }

        public HallTypeDTO GetById(int hallTypeId)
        {
            var entity = _hallTypeRepository.GetById(hallTypeId);
            if (entity == null) return null;
            return new HallTypeDTO
            {
                HallTypeId = entity.HallTypeId,
                HallTypeName = entity.HallTypeName,
                MinTablePrice = entity.MinTablePrice
            };
        }

        public void Create(HallTypeDTO hallTypeDTO)
        {
            var entity = new HallType
            {
                HallTypeId = hallTypeDTO.HallTypeId,
                HallTypeName = hallTypeDTO.HallTypeName,
                MinTablePrice = hallTypeDTO.MinTablePrice
            };
            _hallTypeRepository.Create(entity);
        }

        public void Update(HallTypeDTO hallTypeDTO)
        {
            var entity = new HallType
            {
                HallTypeId = hallTypeDTO.HallTypeId,
                HallTypeName = hallTypeDTO.HallTypeName,
                MinTablePrice = hallTypeDTO.MinTablePrice
            };
            _hallTypeRepository.Update(entity);
        }

        public void Delete(int hallTypeId)
        {
            _hallTypeRepository.Delete(hallTypeId);
        }
    }
}