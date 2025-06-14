using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;
using QuanLyTiecCuoi.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using QuanLyTiecCuoi.Presentation.ViewModel;

namespace QuanLyTiecCuoi.Presentation.ViewModel {
    public class ReportViewModel : BaseViewModel{
        public ICommand BillCommand { get; set; }
        public ReportViewModel() {
            BillCommand = new RelayCommand<object>((p) => true, (p) => {
                BillCommandFunc();
            });
        }
        public void BillCommandFunc() {
            var billView = new Presentation.View.BillView() {
                DataContext = new BillViewModel()
            };
            billView.ShowDialog();
        }
    }
}
