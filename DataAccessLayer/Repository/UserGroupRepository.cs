using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class UserGroupRepository : IUserGroupRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public UserGroupRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<UserGroup> GetAll()
        {
            return _context.UserGroups.ToList();
        }

        public UserGroup GetById(string groupId)
        {
            return _context.UserGroups.Find(groupId);
        }

        public void Create(UserGroup userGroup)
        {
            _context.UserGroups.Add(userGroup);
            _context.SaveChanges();
        }

        public void Update(UserGroup userGroup)
        {
            var existing = _context.UserGroups.Find(userGroup.GroupId);
            if (existing != null)
            {
                existing.GroupName = userGroup.GroupName;
                _context.SaveChanges();
            }
        }

        public void Delete(string groupId)
        {
            var userGroup = _context.UserGroups.Find(groupId);
            if (userGroup != null)
            {
                _context.UserGroups.Remove(userGroup);
                _context.SaveChanges();
            }
        }
    }
}