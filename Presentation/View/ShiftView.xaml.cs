using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QuanLyTiecCuoi.Infrastructure;
using QuanLyTiecCuoi.ViewModel;

namespace QuanLyTiecCuoi.Presentation.View
{
    /// <summary>
    /// Interaction logic for ShiftView.xaml
    /// </summary>
    public partial class ShiftView : UserControl
    {
        public ShiftView()
        {
            InitializeComponent();
            
            // Lấy ShiftViewModel từ DI Container thay vì khởi tạo thủ công
            this.DataContext = ServiceContainer.GetService<ShiftViewModel>();
        }
    }
}
