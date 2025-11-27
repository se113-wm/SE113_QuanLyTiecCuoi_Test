using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class UserGroupService : IUserGroupService
    {
        private readonly IUserGroupRepository _userGroupRepository;

        public UserGroupService(IUserGroupRepository userGroupRepository)
        {
            _userGroupRepository = userGroupRepository;
        }

        public IEnumerable<UserGroupDTO> GetAll()
        {
            return _userGroupRepository.GetAll()
                .Select(x => new UserGroupDTO
                {
                    GroupId = x.GroupId,
                    GroupName = x.GroupName
                });
        }

        public UserGroupDTO GetById(string groupId)
        {
            var entity = _userGroupRepository.GetById(groupId);
            if (entity == null) return null;
            return new UserGroupDTO
            {
                GroupId = entity.GroupId,
                GroupName = entity.GroupName
            };
        }

        public void Create(UserGroupDTO userGroupDTO)
        {
            var entity = new UserGroup
            {
                GroupId = userGroupDTO.GroupId,
                GroupName = userGroupDTO.GroupName
            };
            _userGroupRepository.Create(entity);
        }

        public void Update(UserGroupDTO userGroupDTO)
        {
            var entity = new UserGroup
            {
                GroupId = userGroupDTO.GroupId,
                GroupName = userGroupDTO.GroupName
            };
            _userGroupRepository.Update(entity);
        }

        public void Delete(string groupId)
        {
            _userGroupRepository.Delete(groupId);
        }
    }
}