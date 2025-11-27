using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IPermissionRepository
    {
        IEnumerable<Permission> GetAll();
        Permission GetById(string permissionId);
        void Create(Permission permission);
        void Update(Permission permission);
        void Delete(string permissionId);
    }
}