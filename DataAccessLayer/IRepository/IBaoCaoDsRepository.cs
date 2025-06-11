using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IBaoCaoDsRepository
    {
        IEnumerable<BAOCAOD> GetAll();
        BAOCAOD GetByMonthYear(int thang, int nam);
    }
}