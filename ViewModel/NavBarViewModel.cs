using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace QuanLyTiecCuoi.ViewModel
{
    public class NavBarViewModel : BaseViewModel
    {
        #region commands
        public ICommand HomeCommand { get; set; }
        public ICommand MaximizeWindowCommand { get; set; }
        public ICommand MinimizeWindowCommand { get; set; }
        public ICommand MouseMoveWindowCommand { get; set; }
        #endregion

        public NavBarViewModel()
        {
            //UnitCommand = new RelayCommand<object>((p) => { return true; }, (p) => { UnitWindow wd = new UnitWindow(); wd.ShowDialog(); });
        }

    }
}
