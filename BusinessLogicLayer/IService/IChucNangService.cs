using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IChucNangService
    {
        IEnumerable<CHUCNANGDTO> GetAll();
        CHUCNANGDTO GetById(string maChucNang);
        void Create(CHUCNANGDTO chucNangDto);
        void Update(CHUCNANGDTO chucNangDto);
        void Delete(string maChucNang);
    }
}