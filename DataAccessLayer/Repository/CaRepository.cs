using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class CaRepository : ICaRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        // Constructor với Dependency Injection (nếu context được inject từ bên ngoài)
        public CaRepository(QuanLyTiecCuoiEntities context)
        {
            _context = context ?? new QuanLyTiecCuoiEntities();
        }

        // Constructor mặc định để backward compatibility
        public CaRepository() : this(null)
        {
        }

        public IEnumerable<CA> GetAll()
        {
            return _context.CAs.ToList();
        }

        public CA GetById(int maCa)
        {
            return _context.CAs.Find(maCa);
        }

        public void Create(CA ca)
        {
            _context.CAs.Add(ca);
            _context.SaveChanges();
        }

        public void Update(CA ca)
        {
            var existing = _context.CAs.Find(ca.MaCa);
            if (existing != null)
            {
                existing.TenCa = ca.TenCa;
                existing.ThoiGianBatDauCa = ca.ThoiGianBatDauCa;
                existing.ThoiGianKetThucCa = ca.ThoiGianKetThucCa;
                _context.SaveChanges();
            }
        }

        public void Delete(int maCa)
        {
            var ca = _context.CAs.Find(maCa);
            if (ca != null)
            {
                _context.CAs.Remove(ca);
                _context.SaveChanges();
            }
        }
    }
}