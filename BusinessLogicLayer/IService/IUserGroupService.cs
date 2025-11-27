using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IUserGroupService
    {
        IEnumerable<UserGroupDTO> GetAll();
        UserGroupDTO GetById(string groupId);
        void Create(UserGroupDTO userGroupDTO);
        void Update(UserGroupDTO userGroupDTO);
        void Delete(string groupId);
    }
}