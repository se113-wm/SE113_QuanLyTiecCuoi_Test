using System.Collections.Generic;
using QuanLyTiecCuoi.DataTransferObject;

namespace QuanLyTiecCuoi.BusinessLogicLayer.IService
{
    public interface IParameterService
    {
        IEnumerable<ParameterDTO> GetAll();
        ParameterDTO GetByName(string parameterName);
        void Update(ParameterDTO parameterDTO);
    }
}