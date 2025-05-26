using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface INhomNguoiDungService
    {
        IEnumerable<NHOMNGUOIDUNGDTO> GetAll();
        NHOMNGUOIDUNGDTO GetById(string maNhom);
        void Create(NHOMNGUOIDUNGDTO nhomNguoiDungDto);
        void Update(NHOMNGUOIDUNGDTO nhomNguoiDungDto);
        void Delete(string maNhom);
    }
}