using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class CtBaoCaoDsRepository : ICtBaoCaoDsRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public CtBaoCaoDsRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<CTBAOCAOD> GetAll()
        {
            return _context.CTBAOCAODS.ToList();
        }

        public IEnumerable<CTBAOCAOD> GetByMonthYear(int thang, int nam)
        {
            return _context.CTBAOCAODS
                .Where(ct => ct.Thang == thang && ct.Nam == nam)
                .ToList();
        }

        public CTBAOCAOD GetByDate(int ngay, int thang, int nam)
        {
            return _context.CTBAOCAODS
                .FirstOrDefault(ct => ct.Ngay == ngay && ct.Thang == thang && ct.Nam == nam);
        }
    }
}