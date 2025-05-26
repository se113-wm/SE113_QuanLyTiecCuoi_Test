using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface ICaService
    {
        IEnumerable<CADTO> GetAll();
        CADTO GetById(int maCa);
        void Create(CADTO caDto);
        void Update(CADTO caDto);
        void Delete(int maCa);
    }
}