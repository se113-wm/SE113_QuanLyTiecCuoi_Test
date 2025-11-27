using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public IEnumerable<PermissionDTO> GetAll()
        {
            return _permissionRepository.GetAll()
                .Select(x => new PermissionDTO
                {
                    PermissionId = x.PermissionId,
                    PermissionName = x.PermissionName,
                    LoadedScreenName = x.LoadedScreenName
                });
        }

        public PermissionDTO GetById(string permissionId)
        {
            var entity = _permissionRepository.GetById(permissionId);
            if (entity == null) return null;
            return new PermissionDTO
            {
                PermissionId = entity.PermissionId,
                PermissionName = entity.PermissionName,
                LoadedScreenName = entity.LoadedScreenName
            };
        }

        public void Create(PermissionDTO permissionDTO)
        {
            var entity = new Permission
            {
                PermissionId = permissionDTO.PermissionId,
                PermissionName = permissionDTO.PermissionName,
                LoadedScreenName = permissionDTO.LoadedScreenName
            };
            _permissionRepository.Create(entity);
        }

        public void Update(PermissionDTO permissionDTO)
        {
            var entity = new Permission
            {
                PermissionId = permissionDTO.PermissionId,
                PermissionName = permissionDTO.PermissionName,
                LoadedScreenName = permissionDTO.LoadedScreenName
            };
            _permissionRepository.Update(entity);
        }

        public void Delete(string permissionId)
        {
            _permissionRepository.Delete(permissionId);
        }
    }
}