using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IPermissionService
    {
        IEnumerable<PermissionDTO> GetAll();
        PermissionDTO GetById(string permissionId);
        void Create(PermissionDTO permissionDTO);
        void Update(PermissionDTO permissionDTO);
        void Delete(string permissionId);
    }
}