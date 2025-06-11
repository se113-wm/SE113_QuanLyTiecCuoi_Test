using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class BaoCaoDsService : IBaoCaoDsService
    {
        private readonly IBaoCaoDsRepository _baoCaoDsRepository;

        public BaoCaoDsService()
        {
            _baoCaoDsRepository = new BaoCaoDsRepository();
        }

        public IEnumerable<BAOCAODDTO> GetAll()
        {
            return _baoCaoDsRepository.GetAll()
                .Select(x => new BAOCAODDTO
                {
                    Thang = x.Thang,
                    Nam = x.Nam,
                    TongDoanhThu = x.TongDoanhThu,
                    CTBAOCAODS = x.CTBAOCAODS?.Select(ct => new CTBAOCAODDTO
                    {
                        Ngay = ct.Ngay,
                        Thang = ct.Thang,
                        Nam = ct.Nam,
                        SoLuongTiec = ct.SoLuongTiec,
                        DoanhThu = ct.DoanhThu,
                        TiLe = ct.TiLe
                    }).ToList()
                });
        }

        public BAOCAODDTO GetByMonthYear(int thang, int nam)
        {
            var entity = _baoCaoDsRepository.GetByMonthYear(thang, nam);
            if (entity == null) return null;
            return new BAOCAODDTO
            {
                Thang = entity.Thang,
                Nam = entity.Nam,
                TongDoanhThu = entity.TongDoanhThu,
                CTBAOCAODS = entity.CTBAOCAODS?.Select(ct => new CTBAOCAODDTO
                {
                    Ngay = ct.Ngay,
                    Thang = ct.Thang,
                    Nam = ct.Nam,
                    SoLuongTiec = ct.SoLuongTiec,
                    DoanhThu = ct.DoanhThu,
                    TiLe = ct.TiLe
                }).ToList()
            };
        }
    }
}