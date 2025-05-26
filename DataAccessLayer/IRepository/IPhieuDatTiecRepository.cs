using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IPhieuDatTiecRepository
    {
        IEnumerable<PHIEUDATTIEC> GetAll();
        PHIEUDATTIEC GetById(int maPhieuDat);
        void Create(PHIEUDATTIEC phieuDatTiec);
        void Update(PHIEUDATTIEC phieuDatTiec);
        void Delete(int maPhieuDat);
    }
}