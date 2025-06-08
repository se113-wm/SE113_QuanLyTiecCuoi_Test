using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IThucDonService
    {
        IEnumerable<THUCDONDTO> GetAll();
        IEnumerable<THUCDONDTO> GetByPhieuDat(int maPhieuDat);
        THUCDONDTO GetById(int maPhieuDat, int maMonAn);
        void Create(THUCDONDTO thucDonDto);
        void Update(THUCDONDTO thucDonDto);
        void Delete(int maPhieuDat, int maMonAn);
    }
}