using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IHallTypeService
    {
        IEnumerable<HallTypeDTO> GetAll();
        HallTypeDTO GetById(int hallTypeId);
        void Create(HallTypeDTO hallTypeDTO);
        void Update(HallTypeDTO hallTypeDTO);
        void Delete(int hallTypeId);
    }
}