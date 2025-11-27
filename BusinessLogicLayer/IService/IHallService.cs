using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IHallService
    {
        IEnumerable<HallDTO> GetAll();
        HallDTO GetById(int hallId);
        void Create(HallDTO hallDto);
        void Update(HallDTO hallDto);
        void Delete(int hallId);
    }
}