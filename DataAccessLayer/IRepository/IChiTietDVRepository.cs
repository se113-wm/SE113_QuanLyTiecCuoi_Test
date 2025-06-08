using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IChiTietDVRepository
    {
        IEnumerable<CHITIETDV> GetAll();
        CHITIETDV GetById(int maPhieuDat, int maDichVu);
        IEnumerable<CHITIETDV> GetByMaPhieuDat(int maPhieuDat);
        void Create(CHITIETDV chiTietDV);
        void Update(CHITIETDV chiTietDV);
        void Delete(int maPhieuDat, int maDichVu);
    }
}