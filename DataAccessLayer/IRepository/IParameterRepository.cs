using System.Collections.Generic;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.DataAccessLayer.IRepository
{
    public interface IParameterRepository
    {
        IEnumerable<Parameter> GetAll();
        Parameter GetByName(string parameterName);
        void Update(Parameter parameter);
    }
}