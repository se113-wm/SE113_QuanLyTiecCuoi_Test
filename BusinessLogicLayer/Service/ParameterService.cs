using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class ParameterService : IParameterService
    {
        private readonly IParameterRepository _parameterRepository;

        // Constructor với Dependency Injection
        public ParameterService(IParameterRepository parameterRepository)
        {
            _parameterRepository = parameterRepository;
        }

        public IEnumerable<ParameterDTO> GetAll()
        {
            return _parameterRepository.GetAll()
                .Select(x => new ParameterDTO
                {
                    ParameterName = x.ParameterName,
                    Value = x.Value
                });
        }

        public ParameterDTO GetByName(string parameterName)
        {
            var entity = _parameterRepository.GetByName(parameterName);
            if (entity == null) return null;
            return new ParameterDTO
            {
                ParameterName = entity.ParameterName,
                Value = entity.Value
            };
        }

        public void Update(ParameterDTO parameterDTO)
        {
            var entity = new Parameter
            {
                ParameterName = parameterDTO.ParameterName,
                Value = parameterDTO.Value
            };
            _parameterRepository.Update(entity);
        }
    }
}