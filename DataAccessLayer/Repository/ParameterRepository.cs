using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class ParameterRepository : IParameterRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public ParameterRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<Parameter> GetAll()
        {
            return _context.Parameters.ToList();
        }

        public Parameter GetByName(string parameterName)
        {
            return _context.Parameters.FirstOrDefault(p => p.ParameterName == parameterName);
        }

        public void Update(Parameter parameter)
        {
            var existing = _context.Parameters.FirstOrDefault(p => p.ParameterName == parameter.ParameterName);
            if (existing != null)
            {
                existing.Value = parameter.Value;
                _context.SaveChanges();
            }
        }
    }
}