using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface ICtBaoCaoDsService
    {
        IEnumerable<CTBAOCAODDTO> GetAll();
        IEnumerable<CTBAOCAODDTO> GetByMonthYear(int thang, int nam);
        CTBAOCAODDTO GetByDate(int ngay, int thang, int nam);
    }
}