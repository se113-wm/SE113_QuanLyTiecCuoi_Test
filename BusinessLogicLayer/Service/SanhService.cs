using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class HallService : IHallService
    {
        private readonly IHallRepository _hallRepository;

        public HallService(IHallRepository hallRepository)
        {
            _hallRepository = hallRepository;
        }

        public IEnumerable<HallDTO> GetAll()
        {
            return _hallRepository.GetAll()
                .Select(x => new HallDTO
                {
                    HallId = x.HallId,
                    HallTypeId = x.HallTypeId,
                    HallName = x.HallName,
                    MaxTableCount = x.MaxTableCount,
                    Note = x.Note,
                    HallType = x.HallType != null
                        ? new HallTypeDTO
                        {
                            HallTypeId = x.HallType.HallTypeId,
                            HallTypeName = x.HallType.HallTypeName,
                            MinTablePrice = x.HallType.MinTablePrice
                        }
                        : null
                });
        }

        public HallDTO GetById(int hallId)
        {
            var entity = _hallRepository.GetById(hallId);
            if (entity == null) return null;
            return new HallDTO
            {
                HallId = entity.HallId,
                HallTypeId = entity.HallTypeId,
                HallName = entity.HallName,
                MaxTableCount = entity.MaxTableCount,
                Note = entity.Note,
                HallType = entity.HallType != null
                    ? new HallTypeDTO
                    {
                        HallTypeId = entity.HallType.HallTypeId,
                        HallTypeName = entity.HallType.HallTypeName,
                        MinTablePrice = entity.HallType.MinTablePrice
                    }
                    : null
            };
        }

        public void Create(HallDTO hallDto)
        {
            var entity = new Hall
            {
                HallId = hallDto.HallId,
                HallTypeId = hallDto.HallTypeId,
                HallName = hallDto.HallName,
                MaxTableCount = hallDto.MaxTableCount,
                Note = hallDto.Note
            };
            _hallRepository.Create(entity);
            hallDto.HallId = entity.HallId;
        }

        public void Update(HallDTO hallDto)
        {
            var entity = new Hall
            {
                HallId = hallDto.HallId,
                HallTypeId = hallDto.HallTypeId,
                HallName = hallDto.HallName,
                MaxTableCount = hallDto.MaxTableCount,
                Note = hallDto.Note
            };
            _hallRepository.Update(entity);
        }

        public void Delete(int hallId)
        {
            _hallRepository.Delete(hallId);
        }
    }
}