using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace QuanLyTiecCuoi.ViewModel
{
    public class ParameterViewModel : BaseViewModel
    {
        private readonly IThamSoService _thamSoService;

        private decimal _KiemTraPhat;
        public decimal KiemTraPhat { get => _KiemTraPhat; set { _KiemTraPhat = value; OnPropertyChanged(); } }
        private string _KiemTraPhatText;
        public string KiemTraPhatText { get => _KiemTraPhatText; set { _KiemTraPhatText = value; OnPropertyChanged(); } }

        private string _TiLePhat;
        public string TiLePhat { get => _TiLePhat; set { _TiLePhat = value; OnPropertyChanged(); } }

        private string _TiLeTienDatCocToiThieu;
        public string TiLeTienDatCocToiThieu { get => _TiLeTienDatCocToiThieu; set { _TiLeTienDatCocToiThieu = value; OnPropertyChanged(); } }

        private string _TiLeSoBanDatTruocToiThieu;
        public string TiLeSoBanDatTruocToiThieu { get => _TiLeSoBanDatTruocToiThieu; set { _TiLeSoBanDatTruocToiThieu = value; OnPropertyChanged(); } }

        public ICommand EditCommand { get; set; }
        public ICommand ResetCommand { get; set; }

        private string _editMessage;
        public string EditMessage { get => _editMessage; set { _editMessage = value; OnPropertyChanged(); } }

        private bool isInBounds(decimal value, decimal min, decimal max)
        {
            return value >= min && value <= max;
        }

        // Constructor với Dependency Injection
        public ParameterViewModel(IThamSoService thamSoService)
        {
            _thamSoService = thamSoService;

            var thamSoList = _thamSoService.GetAll().ToList();

            var kiemTraPhat = thamSoList.First(x => x.TenThamSo == "KiemTraPhat");
            var tiLePhat = thamSoList.First(x => x.TenThamSo == "TiLePhat");
            var tiLeTienDatCocToiThieu = thamSoList.First(x => x.TenThamSo == "TiLeTienDatCocToiThieu");
            var tiLeSoBanDatTruocToiThieu = thamSoList.First(x => x.TenThamSo == "TiLeSoBanDatTruocToiThieu");

            KiemTraPhat = (decimal)kiemTraPhat.GiaTri;
            TiLePhat = tiLePhat.GiaTri.ToString();
            TiLeTienDatCocToiThieu = tiLeTienDatCocToiThieu.GiaTri.ToString();
            TiLeSoBanDatTruocToiThieu = tiLeSoBanDatTruocToiThieu.GiaTri.ToString();

            if (KiemTraPhat == 1)
                KiemTraPhatText = "Có";
            else 
                KiemTraPhatText = "Không";

            EditCommand = new RelayCommand<object>((p) =>
            {
                if (string.IsNullOrWhiteSpace(TiLePhat) || string.IsNullOrWhiteSpace(TiLeTienDatCocToiThieu) || string.IsNullOrWhiteSpace(TiLeSoBanDatTruocToiThieu))
                {
                    EditMessage = "Vui lòng nhập đầy đủ thông tin.";
                    return false;
                }
                if (!decimal.TryParse(TiLePhat, out _) || !decimal.TryParse(TiLeTienDatCocToiThieu, out _) || !decimal.TryParse(TiLeSoBanDatTruocToiThieu, out _))
                {
                    EditMessage = "Vui lòng nhập đúng định dạng số.";
                    return false;
                }
                if (!isInBounds(decimal.Parse(TiLePhat), 0, 1) || !isInBounds(decimal.Parse(TiLeTienDatCocToiThieu), 0, 1) || !isInBounds(decimal.Parse(TiLeSoBanDatTruocToiThieu), 0, 1))
                {
                    EditMessage = "Vui lòng nhập số trong khoảng từ 0 đến 1.";
                    return false;
                }
                EditMessage = string.Empty;
                return true;
            }, (p) =>
            {
                if (thamSoList != null)
                {
                    try
                    {
                        kiemTraPhat.TenThamSo = "KiemTraPhat";
                        decimal gt = KiemTraPhatText == "Có" ? 1 : 0;
                        kiemTraPhat.GiaTri = gt;

                        tiLePhat.TenThamSo = "TiLePhat";
                        tiLePhat.GiaTri = decimal.Parse(TiLePhat);

                        tiLeTienDatCocToiThieu.TenThamSo = "TiLeTienDatCocToiThieu";
                        tiLeTienDatCocToiThieu.GiaTri = decimal.Parse(TiLeTienDatCocToiThieu);

                        tiLeSoBanDatTruocToiThieu.TenThamSo = "TiLeSoBanDatTruocToiThieu";
                        tiLeSoBanDatTruocToiThieu.GiaTri = decimal.Parse(TiLeSoBanDatTruocToiThieu);

                        _thamSoService.Update(tiLePhat);
                        _thamSoService.Update(tiLeTienDatCocToiThieu);
                        _thamSoService.Update(tiLeSoBanDatTruocToiThieu);
                        _thamSoService.Update(kiemTraPhat);

                        MessageBox.Show($"Cập nhật tham số thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

                        EditMessage = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi cập nhật tham số: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            });
            
            ResetCommand = new RelayCommand<object>((p) => true, (p) =>
            {
                KiemTraPhat = (decimal)thamSoList.First(x => x.TenThamSo == "KiemTraPhat").GiaTri;
                TiLePhat = thamSoList.First(x => x.TenThamSo == "TiLePhat").GiaTri.ToString();
                TiLeTienDatCocToiThieu = thamSoList.First(x => x.TenThamSo == "TiLeTienDatCocToiThieu").GiaTri.ToString();
                TiLeSoBanDatTruocToiThieu = thamSoList.First(x => x.TenThamSo == "TiLeSoBanDatTruocToiThieu").GiaTri.ToString();
                KiemTraPhatText = KiemTraPhat == 1 ? "Có" : "Không";
            });
        }
    }
}