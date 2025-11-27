using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public AppUserRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<AppUser> GetAll()
        {
            return _context.AppUsers.ToList();
        }

        public AppUser GetById(int userId)
        {
            return _context.AppUsers.Find(userId);
        }

        public void Create(AppUser appUser)
        {
            _context.AppUsers.Add(appUser);
            _context.SaveChanges();
        }

        public void Update(AppUser appUser)
        {
            var existing = _context.AppUsers.Find(appUser.UserId);
            if (existing != null)
            {
                existing.Username = appUser.Username;
                existing.PasswordHash = appUser.PasswordHash;
                existing.FullName = appUser.FullName;
                existing.Email = appUser.Email;
                existing.PhoneNumber = appUser.PhoneNumber;
                existing.Address = appUser.Address;
                existing.BirthDate = appUser.BirthDate;
                existing.Gender = appUser.Gender;
                existing.GroupId = appUser.GroupId;
                _context.SaveChanges();
            }
        }

        public void Delete(int userId)
        {
            var appUser = _context.AppUsers.Find(userId);
            if (appUser != null)
            {
                _context.AppUsers.Remove(appUser);
                _context.SaveChanges();
            }
        }
    }
}