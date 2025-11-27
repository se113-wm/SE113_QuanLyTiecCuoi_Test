using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IServiceRepository
    {
        IEnumerable<Service> GetAll();
        Service GetById(int serviceId);
        void Create(Service service);
        void Update(Service service);
        void Delete(int serviceId);
    }
}