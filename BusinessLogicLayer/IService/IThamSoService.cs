using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IThamSoService
    {
        IEnumerable<THAMSODTO> GetAll();
        THAMSODTO GetByName(string tenThamSo);
        void Create(THAMSODTO thamSoDto);
        void Update(THAMSODTO thamSoDto);
        void Delete(string tenThamSo);
    }
}