using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface ILoaiSanhService
    {
        IEnumerable<LOAISANHDTO> GetAll();
        LOAISANHDTO GetById(int maLoaiSanh);
        void Create(LOAISANHDTO loaiSanhDto);
        void Update(LOAISANHDTO loaiSanhDto);
        void Delete(int maLoaiSanh);
    }
}