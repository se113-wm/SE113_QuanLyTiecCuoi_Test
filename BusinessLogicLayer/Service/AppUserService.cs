using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class AppUserService : IAppUserService
    {
        private readonly IAppUserRepository _appUserRepository;

        public AppUserService(IAppUserRepository appUserRepository)
        {
            _appUserRepository = appUserRepository;
        }

        public IEnumerable<AppUserDTO> GetAll()
        {
            return _appUserRepository.GetAll()
                .Select(x => new AppUserDTO
                {
                    UserId = x.UserId,
                    Username = x.Username,
                    PasswordHash = x.PasswordHash,
                    FullName = x.FullName,
                    Email = x.Email,
                    PhoneNumber = x.PhoneNumber,
                    Address = x.Address,
                    BirthDate = x.BirthDate,
                    Gender = x.Gender,
                    GroupId = x.GroupId,
                    UserGroup = x.UserGroup != null
                        ? new UserGroupDTO
                        {
                            GroupId = x.UserGroup.GroupId,
                            GroupName = x.UserGroup.GroupName
                        }
                        : null
                });
        }

        public AppUserDTO GetById(int userId)
        {
            var entity = _appUserRepository.GetById(userId);
            if (entity == null) return null;
            return new AppUserDTO
            {
                UserId = entity.UserId,
                Username = entity.Username,
                PasswordHash = entity.PasswordHash,
                FullName = entity.FullName,
                Email = entity.Email,
                PhoneNumber = entity.PhoneNumber,
                Address = entity.Address,
                BirthDate = entity.BirthDate,
                Gender = entity.Gender,
                GroupId = entity.GroupId,
                UserGroup = entity.UserGroup != null
                    ? new UserGroupDTO
                    {
                        GroupId = entity.UserGroup.GroupId,
                        GroupName = entity.UserGroup.GroupName
                    }
                    : null
            };
        }

        public void Create(AppUserDTO appUserDTO)
        {
            var entity = new AppUser
            {
                UserId = appUserDTO.UserId,
                Username = appUserDTO.Username,
                PasswordHash = appUserDTO.PasswordHash,
                FullName = appUserDTO.FullName,
                Email = appUserDTO.Email,
                PhoneNumber = appUserDTO.PhoneNumber,
                Address = appUserDTO.Address,
                BirthDate = appUserDTO.BirthDate,
                Gender = appUserDTO.Gender,
                GroupId = appUserDTO.GroupId
            };
            _appUserRepository.Create(entity);
        }

        public void Update(AppUserDTO appUserDTO)
        {
            var entity = new AppUser
            {
                UserId = appUserDTO.UserId,
                Username = appUserDTO.Username,
                PasswordHash = appUserDTO.PasswordHash,
                FullName = appUserDTO.FullName,
                Email = appUserDTO.Email,
                PhoneNumber = appUserDTO.PhoneNumber,
                Address = appUserDTO.Address,
                BirthDate = appUserDTO.BirthDate,
                Gender = appUserDTO.Gender,
                GroupId = appUserDTO.GroupId
            };
            _appUserRepository.Update(entity);
        }

        public void Delete(int userId)
        {
            _appUserRepository.Delete(userId);
        }
    }
}