using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class BaoCaoDsRepository : IBaoCaoDsRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public BaoCaoDsRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<BAOCAOD> GetAll()
        {
            // Bao gồm cả danh sách chi tiết báo cáo doanh thu
            return _context.BAOCAODS.Include("CTBAOCAODS").ToList();
        }

        public BAOCAOD GetByMonthYear(int thang, int nam)
        {
            // Lấy báo cáo doanh thu theo tháng và năm, bao gồm chi tiết
            return _context.BAOCAODS
                .Include("CTBAOCAODS")
                .FirstOrDefault(bc => bc.Thang == thang && bc.Nam == nam);
        }
    }
}