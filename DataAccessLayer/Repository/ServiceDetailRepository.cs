using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class ServiceDetailRepository : IServiceDetailRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public ServiceDetailRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<ServiceDetail> GetAll()
        {
            return _context.ServiceDetails.ToList();
        }

        public ServiceDetail GetById(int bookingId, int serviceId)
        {
            return _context.ServiceDetails.FirstOrDefault(x => x.BookingId == bookingId && x.ServiceId == serviceId);
        }

        public IEnumerable<ServiceDetail> GetByBookingId(int bookingId)
        {
            return _context.ServiceDetails.Where(x => x.BookingId == bookingId).ToList();
        }

        public void Create(ServiceDetail serviceDetail)
        {
            _context.ServiceDetails.Add(serviceDetail);
            _context.SaveChanges();
        }

        public void Update(ServiceDetail serviceDetail)
        {
            var existing = _context.ServiceDetails.FirstOrDefault(x => x.BookingId == serviceDetail.BookingId && x.ServiceId == serviceDetail.ServiceId);
            if (existing != null)
            {
                existing.Quantity = serviceDetail.Quantity;
                existing.UnitPrice = serviceDetail.UnitPrice;
                existing.TotalAmount = serviceDetail.TotalAmount;
                existing.Note = serviceDetail.Note;
                _context.SaveChanges();
            }
        }

        public void Delete(int bookingId, int serviceId)
        {
            var serviceDetail = _context.ServiceDetails.FirstOrDefault(x => x.BookingId == bookingId && x.ServiceId == serviceId);
            if (serviceDetail != null)
            {
                _context.ServiceDetails.Remove(serviceDetail);
                _context.SaveChanges();
            }
        }
    }
}