using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IThamSoRepository
    {
        IEnumerable<THAMSO> GetAll();
        THAMSO GetByName(string tenThamSo);
        void Update(THAMSO thamSo);
    }
}