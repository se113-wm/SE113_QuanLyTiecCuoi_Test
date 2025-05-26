using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class NhomNguoiDungService : INhomNguoiDungService
    {
        private readonly INhomNguoiDungRepository _nhomNguoiDungRepository;

        public NhomNguoiDungService()
        {
            _nhomNguoiDungRepository = new NhomNguoiDungRepository();
        }

        public IEnumerable<NHOMNGUOIDUNGDTO> GetAll()
        {
            return _nhomNguoiDungRepository.GetAll()
                .Select(x => new NHOMNGUOIDUNGDTO
                {
                    MaNhom = x.MaNhom,
                    TenNhom = x.TenNhom
                });
        }

        public NHOMNGUOIDUNGDTO GetById(string maNhom)
        {
            var entity = _nhomNguoiDungRepository.GetById(maNhom);
            if (entity == null) return null;
            return new NHOMNGUOIDUNGDTO
            {
                MaNhom = entity.MaNhom,
                TenNhom = entity.TenNhom
            };
        }

        public void Create(NHOMNGUOIDUNGDTO nhomNguoiDungDto)
        {
            var entity = new NHOMNGUOIDUNG
            {
                MaNhom = nhomNguoiDungDto.MaNhom,
                TenNhom = nhomNguoiDungDto.TenNhom
            };
            _nhomNguoiDungRepository.Create(entity);
        }

        public void Update(NHOMNGUOIDUNGDTO nhomNguoiDungDto)
        {
            var entity = new NHOMNGUOIDUNG
            {
                MaNhom = nhomNguoiDungDto.MaNhom,
                TenNhom = nhomNguoiDungDto.TenNhom
            };
            _nhomNguoiDungRepository.Update(entity);
        }

        public void Delete(string maNhom)
        {
            _nhomNguoiDungRepository.Delete(maNhom);
        }
    }
}