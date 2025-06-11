using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface ICtBaoCaoDsRepository
    {
        IEnumerable<CTBAOCAOD> GetAll();
        IEnumerable<CTBAOCAOD> GetByMonthYear(int thang, int nam);
        CTBAOCAOD GetByDate(int ngay, int thang, int nam);
    }
}