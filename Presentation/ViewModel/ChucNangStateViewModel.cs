using QuanLyTiecCuoi.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xaml;

namespace QuanLyTiecCuoi.ViewModel
{
    public class ChucNangState : BaseViewModel
    {
        public string MaChucNang { get; set; }
        public string TenManHinhDuocLoad { get; set; }
        private bool _isChecked;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged();
                    // Gọi cập nhật phân quyền ở đây
                    UpdatePermission?.Invoke(this, EventArgs.Empty);
                }
            }
        }
        public event EventHandler UpdatePermission;
    }
}