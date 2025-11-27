using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class ServiceDetailService : IServiceDetailService
    {
        private readonly IServiceDetailRepository _serviceDetailRepository;

        public ServiceDetailService(IServiceDetailRepository serviceDetailRepository)
        {
            _serviceDetailRepository = serviceDetailRepository;
        }

        public IEnumerable<ServiceDetailDTO> GetAll()
        {
            return _serviceDetailRepository.GetAll()
                .Select(x => MapToDto(x));
        }

        public IEnumerable<ServiceDetailDTO> GetByBookingId(int bookingId)
        {
            return _serviceDetailRepository.GetByBookingId(bookingId)
                .Select(x => MapToDto(x));
        }

        public ServiceDetailDTO GetById(int bookingId, int serviceId)
        {
            var entity = _serviceDetailRepository.GetById(bookingId, serviceId);
            return entity == null ? null : MapToDto(entity);
        }

        public void Create(ServiceDetailDTO serviceDetailDTO)
        {
            var entity = MapToEntity(serviceDetailDTO);
            _serviceDetailRepository.Create(entity);
        }

        public void Update(ServiceDetailDTO serviceDetailDTO)
        {
            var entity = MapToEntity(serviceDetailDTO);
            _serviceDetailRepository.Update(entity);
        }

        public void Delete(int bookingId, int serviceId)
        {
            _serviceDetailRepository.Delete(bookingId, serviceId);
        }

        private static ServiceDetailDTO MapToDto(ServiceDetail x)
        {
            return new ServiceDetailDTO
            {
                BookingId = x.BookingId,
                ServiceId = x.ServiceId,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice,
                TotalAmount = x.TotalAmount,
                Note = x.Note,
                Service = x.Service != null
                    ? new ServiceDTO
                    {
                        ServiceId = x.Service.ServiceId,
                        ServiceName = x.Service.ServiceName,
                        UnitPrice = x.Service.UnitPrice,
                        Note = x.Service.Note
                    }
                    : null,
                Booking = x.Booking != null
                    ? new BookingDTO
                    {
                        BookingId = x.Booking.BookingId,
                    }
                    : null
            };
        }

        private static ServiceDetail MapToEntity(ServiceDetailDTO dto)
        {
            return new ServiceDetail
            {
                BookingId = dto.BookingId,
                ServiceId = dto.ServiceId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                TotalAmount = dto.TotalAmount,
                Note = dto.Note
            };
        }
    }
}