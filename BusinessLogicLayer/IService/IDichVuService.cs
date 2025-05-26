using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IDichVuService
    {
        IEnumerable<DICHVUDTO> GetAll();
        DICHVUDTO GetById(int maDichVu);
        void Create(DICHVUDTO dichVuDto);
        void Update(DICHVUDTO dichVuDto);
        void Delete(int maDichVu);
    }
}