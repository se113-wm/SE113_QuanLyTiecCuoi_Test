using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IAppUserService
    {
        IEnumerable<AppUserDTO> GetAll();
        AppUserDTO GetById(int userId);
        void Create(AppUserDTO appUserDTO);
        void Update(AppUserDTO appUserDTO);
        void Delete(int userId);
    }
}