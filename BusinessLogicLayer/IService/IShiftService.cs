using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IShiftService
    {
        IEnumerable<ShiftDTO> GetAll();
        ShiftDTO GetById(int shiftId);
        void Create(ShiftDTO shiftDTO);
        void Update(ShiftDTO shiftDTO);
        void Delete(int shiftId);
    }
}