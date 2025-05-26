using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface INguoiDungService
    {
        IEnumerable<NGUOIDUNGDTO> GetAll();
        NGUOIDUNGDTO GetById(int maNguoiDung);
        void Create(NGUOIDUNGDTO nguoiDungDto);
        void Update(NGUOIDUNGDTO nguoiDungDto);
        void Delete(int maNguoiDung);
    }
}