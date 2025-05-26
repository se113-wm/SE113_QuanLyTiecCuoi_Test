using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface ISanhService
    {
        IEnumerable<SANHDTO> GetAll();
        SANHDTO GetById(int maSanh);
        void Create(SANHDTO sanhDto);
        void Update(SANHDTO sanhDto);
        void Delete(int maSanh);
    }
}