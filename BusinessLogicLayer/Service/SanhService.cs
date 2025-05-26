using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class SanhService : ISanhService
    {
        private readonly ISanhRepository _sanhRepository;

        public SanhService()
        {
            _sanhRepository = new SanhRepository();
        }

        public IEnumerable<SANHDTO> GetAll()
        {
            return _sanhRepository.GetAll()
                .Select(x => new SANHDTO
                {
                    MaSanh = x.MaSanh,
                    MaLoaiSanh = x.MaLoaiSanh,
                    TenSanh = x.TenSanh,
                    SoLuongBanToiDa = x.SoLuongBanToiDa,
                    GhiChu = x.GhiChu,
                    LoaiSanh = x.LOAISANH != null
                        ? new LOAISANHDTO
                        {
                            MaLoaiSanh = x.LOAISANH.MaLoaiSanh,
                            TenLoaiSanh = x.LOAISANH.TenLoaiSanh,
                            DonGiaBanToiThieu = x.LOAISANH.DonGiaBanToiThieu
                        }
                        : null
                });
        }

        public SANHDTO GetById(int maSanh)
        {
            var entity = _sanhRepository.GetById(maSanh);
            if (entity == null) return null;
            return new SANHDTO
            {
                MaSanh = entity.MaSanh,
                MaLoaiSanh = entity.MaLoaiSanh,
                TenSanh = entity.TenSanh,
                SoLuongBanToiDa = entity.SoLuongBanToiDa,
                GhiChu = entity.GhiChu,
                LoaiSanh = entity.LOAISANH != null
                    ? new LOAISANHDTO
                    {
                        MaLoaiSanh = entity.LOAISANH.MaLoaiSanh,
                        TenLoaiSanh = entity.LOAISANH.TenLoaiSanh,
                        DonGiaBanToiThieu = entity.LOAISANH.DonGiaBanToiThieu
                    }
                    : null
            };
        }

        public void Create(SANHDTO sanhDto)
        {
            var entity = new SANH
            {
                MaSanh = sanhDto.MaSanh,
                MaLoaiSanh = sanhDto.MaLoaiSanh,
                TenSanh = sanhDto.TenSanh,
                SoLuongBanToiDa = sanhDto.SoLuongBanToiDa,
                GhiChu = sanhDto.GhiChu
            };
            _sanhRepository.Create(entity);
        }

        public void Update(SANHDTO sanhDto)
        {
            var entity = new SANH
            {
                MaSanh = sanhDto.MaSanh,
                MaLoaiSanh = sanhDto.MaLoaiSanh,
                TenSanh = sanhDto.TenSanh,
                SoLuongBanToiDa = sanhDto.SoLuongBanToiDa,
                GhiChu = sanhDto.GhiChu
            };
            _sanhRepository.Update(entity);
        }

        public void Delete(int maSanh)
        {
            _sanhRepository.Delete(maSanh);
        }
    }
}