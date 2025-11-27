using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IServiceService
    {
        IEnumerable<ServiceDTO> GetAll();
        ServiceDTO GetById(int serviceId);
        void Create(ServiceDTO serviceDTO);
        void Update(ServiceDTO serviceDTO);
        void Delete(int serviceId);
    }
}