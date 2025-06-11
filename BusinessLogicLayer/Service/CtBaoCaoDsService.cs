using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class CtBaoCaoDsService : ICtBaoCaoDsService
    {
        private readonly ICtBaoCaoDsRepository _ctBaoCaoDsRepository;

        public CtBaoCaoDsService()
        {
            _ctBaoCaoDsRepository = new CtBaoCaoDsRepository();
        }

        public IEnumerable<CTBAOCAODDTO> GetAll()
        {
            return _ctBaoCaoDsRepository.GetAll()
                .Select(x => new CTBAOCAODDTO
                {
                    Ngay = x.Ngay,
                    Thang = x.Thang,
                    Nam = x.Nam,
                    SoLuongTiec = x.SoLuongTiec,
                    DoanhThu = x.DoanhThu,
                    TiLe = x.TiLe
                });
        }

        public IEnumerable<CTBAOCAODDTO> GetByMonthYear(int thang, int nam)
        {
            return _ctBaoCaoDsRepository.GetByMonthYear(thang, nam)
                .Select(x => new CTBAOCAODDTO
                {
                    Ngay = x.Ngay,
                    Thang = x.Thang,
                    Nam = x.Nam,
                    SoLuongTiec = x.SoLuongTiec,
                    DoanhThu = x.DoanhThu,
                    TiLe = x.TiLe
                });
        }

        public CTBAOCAODDTO GetByDate(int ngay, int thang, int nam)
        {
            var entity = _ctBaoCaoDsRepository.GetByDate(ngay, thang, nam);
            if (entity == null) return null;
            return new CTBAOCAODDTO
            {
                Ngay = entity.Ngay,
                Thang = entity.Thang,
                Nam = entity.Nam,
                SoLuongTiec = entity.SoLuongTiec,
                DoanhThu = entity.DoanhThu,
                TiLe = entity.TiLe
            };
        }
    }
}