using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class ShiftService : IShiftService
    {
        private readonly IShiftRepository _shiftRepository;

        // Constructor với Dependency Injection
        public ShiftService(IShiftRepository shiftRepository)
        {
            _shiftRepository = shiftRepository;
        }

        public IEnumerable<ShiftDTO> GetAll()
        {
            return _shiftRepository.GetAll()
                .Select(x => new ShiftDTO
                {
                    ShiftId = x.ShiftId,
                    ShiftName = x.ShiftName,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime
                });
        }

        public ShiftDTO GetById(int shiftId)
        {
            var entity = _shiftRepository.GetById(shiftId);
            if (entity == null) return null;
            return new ShiftDTO
            {
                ShiftId = entity.ShiftId,
                ShiftName = entity.ShiftName,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime
            };
        }

        public void Create(ShiftDTO shiftDTO)
        {
            var entity = new Shift
            {
                ShiftId = shiftDTO.ShiftId,
                ShiftName = shiftDTO.ShiftName,
                StartTime = shiftDTO.StartTime,
                EndTime = shiftDTO.EndTime
            };
            _shiftRepository.Create(entity);
        }

        public void Update(ShiftDTO shiftDTO)
        {
            var entity = new Shift
            {
                ShiftId = shiftDTO.ShiftId,
                ShiftName = shiftDTO.ShiftName,
                StartTime = shiftDTO.StartTime,
                EndTime = shiftDTO.EndTime
            };
            _shiftRepository.Update(entity);
        }

        public void Delete(int shiftId)
        {
            _shiftRepository.Delete(shiftId);
        }
    }
}