using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyTiecCuoi.Presentation.ViewModel
{
    public class InvoiceViewModel
    {
       private int _invoiceId;
        public InvoiceViewModel(int invoiceId)
        {
            _invoiceId = invoiceId;
        }
    }
}
