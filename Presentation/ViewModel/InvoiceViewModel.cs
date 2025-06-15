using QuanLyTiecCuoi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyTiecCuoi.Presentation.ViewModel
{
    public class InvoiceViewModel : BaseViewModel
    {
       private int _invoiceId;
        private string _GhiChu;
        public string GhiChu { get => _GhiChu; set { _GhiChu = value; OnPropertyChanged(); } }
        public InvoiceViewModel(int invoiceId)
        {
            _invoiceId = invoiceId;
            GhiChu = invoiceId.ToString();
        }
    }
}
