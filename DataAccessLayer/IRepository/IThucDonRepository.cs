using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IThucDonRepository
    {
        IEnumerable<THUCDON> GetAll();
        THUCDON GetById(int maPhieuDat, int maMonAn);
        IEnumerable<THUCDON> GetByMaPhieuDat(int maPhieuDat);
        void Create(THUCDON thucDon);
        void Update(THUCDON thucDon);
        void Delete(int maPhieuDat, int maMonAn);
    }
}