using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class NguoiDungService : INguoiDungService
    {
        private readonly INguoiDungRepository _nguoiDungRepository;

        public NguoiDungService()
        {
            _nguoiDungRepository = new NguoiDungRepository();
        }

        public IEnumerable<NGUOIDUNGDTO> GetAll()
        {
            return _nguoiDungRepository.GetAll()
                .Select(x => new NGUOIDUNGDTO
                {
                    MaNguoiDung = x.MaNguoiDung,
                    TenDangNhap = x.TenDangNhap,
                    MatKhauHash = x.MatKhauHash,
                    HoTen = x.HoTen,
                    Email = x.Email,
                    TrangThai = x.TrangThai,
                    MaNhom = x.MaNhom,
                    NhomNguoiDung = x.NHOMNGUOIDUNG != null
                        ? new NHOMNGUOIDUNGDTO
                        {
                            MaNhom = x.NHOMNGUOIDUNG.MaNhom,
                            TenNhom = x.NHOMNGUOIDUNG.TenNhom
                        }
                        : null
                });
        }

        public NGUOIDUNGDTO GetById(int maNguoiDung)
        {
            var entity = _nguoiDungRepository.GetById(maNguoiDung);
            if (entity == null) return null;
            return new NGUOIDUNGDTO
            {
                MaNguoiDung = entity.MaNguoiDung,
                TenDangNhap = entity.TenDangNhap,
                MatKhauHash = entity.MatKhauHash,
                HoTen = entity.HoTen,
                Email = entity.Email,
                TrangThai = entity.TrangThai,
                MaNhom = entity.MaNhom,
                NhomNguoiDung = entity.NHOMNGUOIDUNG != null
                    ? new NHOMNGUOIDUNGDTO
                    {
                        MaNhom = entity.NHOMNGUOIDUNG.MaNhom,
                        TenNhom = entity.NHOMNGUOIDUNG.TenNhom
                    }
                    : null
            };
        }

        public void Create(NGUOIDUNGDTO nguoiDungDto)
        {
            var entity = new NGUOIDUNG
            {
                MaNguoiDung = nguoiDungDto.MaNguoiDung,
                TenDangNhap = nguoiDungDto.TenDangNhap,
                MatKhauHash = nguoiDungDto.MatKhauHash,
                HoTen = nguoiDungDto.HoTen,
                Email = nguoiDungDto.Email,
                TrangThai = nguoiDungDto.TrangThai,
                MaNhom = nguoiDungDto.MaNhom
            };
            _nguoiDungRepository.Create(entity);
        }

        public void Update(NGUOIDUNGDTO nguoiDungDto)
        {
            var entity = new NGUOIDUNG
            {
                MaNguoiDung = nguoiDungDto.MaNguoiDung,
                TenDangNhap = nguoiDungDto.TenDangNhap,
                MatKhauHash = nguoiDungDto.MatKhauHash,
                HoTen = nguoiDungDto.HoTen,
                Email = nguoiDungDto.Email,
                TrangThai = nguoiDungDto.TrangThai,
                MaNhom = nguoiDungDto.MaNhom
            };
            _nguoiDungRepository.Update(entity);
        }

        public void Delete(int maNguoiDung)
        {
            _nguoiDungRepository.Delete(maNguoiDung);
        }
    }
}