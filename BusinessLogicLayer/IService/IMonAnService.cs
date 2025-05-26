using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IMonAnService
    {
        IEnumerable<MONANDTO> GetAll();
        MONANDTO GetById(int maMonAn);
        void Create(MONANDTO monAnDto);
        void Update(MONANDTO monAnDto);
        void Delete(int maMonAn);
    }
}