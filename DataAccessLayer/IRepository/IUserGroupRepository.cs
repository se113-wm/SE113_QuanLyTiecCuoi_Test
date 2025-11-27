using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IUserGroupRepository
    {
        IEnumerable<UserGroup> GetAll();
        UserGroup GetById(string groupId);
        void Create(UserGroup userGroup);
        void Update(UserGroup userGroup);
        void Delete(string groupId);
    }
}