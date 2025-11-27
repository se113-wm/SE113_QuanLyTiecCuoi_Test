using QuanLyTiecCuoi.Infrastructure;
using QuanLyTiecCuoi.ViewModel;
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

namespace QuanLyTiecCuoi
{
    /// <summary>
    /// Interaction logic for PermissionView.xaml
    /// </summary>
    public partial class PermissionView : UserControl
    {
        public PermissionView()
        {
            InitializeComponent();
            DataContext = ServiceContainer.GetService<PermissionViewModel>();
        }
    }
}
