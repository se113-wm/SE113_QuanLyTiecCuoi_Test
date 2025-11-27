using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;

        // Constructor với Dependency Injection
        public ServiceService(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        public IEnumerable<ServiceDTO> GetAll()
        {
            return _serviceRepository.GetAll()
                .Select(x => new ServiceDTO
                {
                    ServiceId = x.ServiceId,
                    ServiceName = x.ServiceName,
                    UnitPrice = x.UnitPrice,
                    Note = x.Note
                });
        }

        public ServiceDTO GetById(int serviceId)
        {
            var entity = _serviceRepository.GetById(serviceId);
            if (entity == null) return null;
            return new ServiceDTO
            {
                ServiceId = entity.ServiceId,
                ServiceName = entity.ServiceName,
                UnitPrice = entity.UnitPrice,
                Note = entity.Note
            };
        }

        public void Create(ServiceDTO serviceDTO)
        {
            var entity = new Model.Service
            {
                ServiceId = serviceDTO.ServiceId,
                ServiceName = serviceDTO.ServiceName,
                UnitPrice = serviceDTO.UnitPrice,
                Note = serviceDTO.Note
            };
            _serviceRepository.Create(entity);
            serviceDTO.ServiceId = entity.ServiceId; // Update DTO with generated ID
        }

        public void Update(ServiceDTO serviceDTO)
        {
            var entity = new Model.Service
            {
                ServiceId = serviceDTO.ServiceId,
                ServiceName = serviceDTO.ServiceName,
                UnitPrice = serviceDTO.UnitPrice,
                Note = serviceDTO.Note
            };
            _serviceRepository.Update(entity);
        }

        public void Delete(int serviceId)
        {
            _serviceRepository.Delete(serviceId);
        }
    }
}