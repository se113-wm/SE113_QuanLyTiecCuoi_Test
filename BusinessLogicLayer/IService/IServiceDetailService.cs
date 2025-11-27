using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IServiceDetailService
    {
        IEnumerable<ServiceDetailDTO> GetAll();
        IEnumerable<ServiceDetailDTO> GetByBookingId(int bookingId);
        ServiceDetailDTO GetById(int bookingId, int serviceId);
        void Create(ServiceDetailDTO serviceDetailDTO);
        void Update(ServiceDetailDTO serviceDetailDTO);
        void Delete(int bookingId, int serviceId);
    }
}