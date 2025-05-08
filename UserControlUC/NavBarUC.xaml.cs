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

namespace QuanLyTiecCuoi.UserControlUC
{
    /// <summary>
    /// Interaction logic for NavBarUC.xaml
    /// </summary>
    public partial class NavBarUC : UserControl
    {
        public NavBarViewModel Viewmodel { get; set; }
        public NavBarUC()
        {
            InitializeComponent();
            this.DataContext = Viewmodel = new NavBarViewModel();
        }
    }
}
