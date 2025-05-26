using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IChucNangRepository
    {
        IEnumerable<CHUCNANG> GetAll();
        CHUCNANG GetById(string maChucNang);
        void Create(CHUCNANG chucNang);
        void Update(CHUCNANG chucNang);
        void Delete(string maChucNang);
    }
}