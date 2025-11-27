using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IAppUserRepository
    {
        IEnumerable<AppUser> GetAll();
        AppUser GetById(int userId);
        void Create(AppUser appUser);
        void Update(AppUser appUser);
        void Delete(int userId);
    }
}