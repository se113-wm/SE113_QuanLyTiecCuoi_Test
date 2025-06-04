using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IPhieuDatTiecService
    {
        IEnumerable<PHIEUDATTIECDTO> GetAll();
        PHIEUDATTIECDTO GetById(int maPhieuDat);
        void Create(PHIEUDATTIECDTO phieuDatTiecDto);
        void Update(PHIEUDATTIECDTO phieuDatTiecDto);
        void Delete(int maPhieuDat);
    }
}