using DocumentFormat.OpenXml.Bibliography;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public ServiceRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<Service> GetAll()
        {
            return _context.Services.ToList();
        }

        public Service GetById(int serviceId)
        {
            return _context.Services.Find(serviceId);
        }

        public void Create(Service service)
        {
            _context.Services.Add(service);
            _context.SaveChanges();
            _context.Entry(service).Reload();
        }

        public void Update(Service service)
        {
            var existing = _context.Services.Find(service.ServiceId);
            if (existing != null)
            {
                existing.ServiceName = service.ServiceName;
                existing.UnitPrice = service.UnitPrice;
                existing.Note = service.Note;
                _context.SaveChanges();
            }
        }

        public void Delete(int serviceId)
        {
            var service = _context.Services.Find(serviceId);
            if (service != null)
            {
                _context.Services.Remove(service);
                _context.SaveChanges();
            }
        }
    }
}