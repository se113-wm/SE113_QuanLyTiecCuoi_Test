using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IServiceDetailRepository
    {
        IEnumerable<ServiceDetail> GetAll();
        ServiceDetail GetById(int bookingId, int serviceId);
        IEnumerable<ServiceDetail> GetByBookingId(int bookingId);
        void Create(ServiceDetail serviceDetail);
        void Update(ServiceDetail serviceDetail);
        void Delete(int bookingId, int serviceId);
    }
}