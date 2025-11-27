using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public PermissionRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<Permission> GetAll()
        {
            return _context.Permissions.ToList();
        }

        public Permission GetById(string permissionId)
        {
            return _context.Permissions.Find(permissionId);
        }

        public void Create(Permission permission)
        {
            _context.Permissions.Add(permission);
            _context.SaveChanges();
        }

        public void Update(Permission permission)
        {
            var existing = _context.Permissions.Find(permission.PermissionId);
            if (existing != null)
            {
                existing.PermissionName = permission.PermissionName;
                existing.LoadedScreenName = permission.LoadedScreenName;
                _context.SaveChanges();
            }
        }

        public void Delete(string permissionId)
        {
            var permission = _context.Permissions.Find(permissionId);
            if (permission != null)
            {
                _context.Permissions.Remove(permission);
                _context.SaveChanges();
            }
        }
    }
}