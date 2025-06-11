using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IBaoCaoDsService
    {
        IEnumerable<BAOCAODDTO> GetAll();
        BAOCAODDTO GetByMonthYear(int thang, int nam);
    }
}