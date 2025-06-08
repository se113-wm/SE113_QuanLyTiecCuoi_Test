using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IChiTietDVService
    {
        IEnumerable<CHITIETDVDTO> GetAll();
        IEnumerable<CHITIETDVDTO> GetByPhieuDat(int maPhieuDat);
        CHITIETDVDTO GetById(int maPhieuDat, int maDichVu);
        void Create(CHITIETDVDTO chiTietDVDto);
        void Update(CHITIETDVDTO chiTietDVDto);
        void Delete(int maPhieuDat, int maDichVu);
    }
}