using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface ICaRepository
    {
        IEnumerable<CA> GetAll();
        CA GetById(int maCa);
        void Create(CA ca);
        void Update(CA ca);
        void Delete(int maCa);
    }
}