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
        #region Service
        private readonly IParameterService _parameterService;
        #endregion

        #region Bindable Fields
        private decimal _enablePenalty;
        public decimal EnablePenalty
        {
            get => _enablePenalty;
            set { _enablePenalty = value; OnPropertyChanged(); }
        }

        private string _enablePenaltyText;
        public string EnablePenaltyText
        {
            get => _enablePenaltyText;
            set { _enablePenaltyText = value; OnPropertyChanged(); }
        }

        private string _penaltyRate;
        public string PenaltyRate
        {
            get => _penaltyRate;
            set { _penaltyRate = value; OnPropertyChanged(); }
        }

        private string _minDepositRate;
        public string MinDepositRate
        {
            get => _minDepositRate;
            set { _minDepositRate = value; OnPropertyChanged(); }
        }

        private string _minAdvanceBookingRate;
        public string MinAdvanceBookingRate
        {
            get => _minAdvanceBookingRate;
            set { _minAdvanceBookingRate = value; OnPropertyChanged(); }
        }
        #endregion

        #region Messages & Commands
        private string _editMessage;
        public string EditMessage
        {
            get => _editMessage;
            set { _editMessage = value; OnPropertyChanged(); }
        }

        public ICommand EditCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        #endregion

        #region Constructor
        public ParameterViewModel(IParameterService parameterService)
        {
            _parameterService = parameterService;

            var parameterList = _parameterService.GetAll().ToList();

            var enablePenalty = parameterList.FirstOrDefault(x => x.ParameterName == "EnablePenalty");
            var penaltyRate = parameterList.FirstOrDefault(x => x.ParameterName == "PenaltyRate");
            var minDepositRate = parameterList.FirstOrDefault(x => x.ParameterName == "MinDepositRate");
            var minAdvanceBookingRate = parameterList.FirstOrDefault(x => x.ParameterName == "MinReserveTableRate");

            if (enablePenalty == null || penaltyRate == null || minDepositRate == null || minAdvanceBookingRate == null)
            {
                MessageBox.Show("Không thể tải thông tin tham số. Vui lòng kiểm tra cơ sở dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                EnablePenalty = 0;
                PenaltyRate = "0";
                MinDepositRate = "0";
                MinAdvanceBookingRate = "0";
                EnablePenaltyText = "Không";
            }
            else
            {
                EnablePenalty = (decimal)enablePenalty.Value;
                PenaltyRate = penaltyRate.Value.ToString();
                MinDepositRate = minDepositRate.Value.ToString();
                MinAdvanceBookingRate = minAdvanceBookingRate.Value.ToString();
                EnablePenaltyText = EnablePenalty == 1 ? "Có" : "Không";
            }

            EditCommand = new RelayCommand<object>(
                (p) => CanEdit(parameterList),
                (p) => EditParameters(parameterList)
            );

            ResetCommand = new RelayCommand<object>(
                (p) => true,
                (p) => Reset(parameterList)
            );
        }
        #endregion

        #region Validation Helpers
        private bool IsInBounds(decimal value, decimal min, decimal max)
        {
            return value >= min && value <= max;
        }
        #endregion

        #region Edit
        private bool CanEdit(System.Collections.Generic.List<ParameterDTO> parameterList)
        {
            if (string.IsNullOrWhiteSpace(PenaltyRate) ||
                string.IsNullOrWhiteSpace(MinDepositRate) ||
                string.IsNullOrWhiteSpace(MinAdvanceBookingRate))
            {
                EditMessage = "Vui lòng nhập đầy đủ thông tin.";
                return false;
            }

            if (!decimal.TryParse(PenaltyRate, out _) ||
                !decimal.TryParse(MinDepositRate, out _) ||
                !decimal.TryParse(MinAdvanceBookingRate, out _))
            {
                EditMessage = "Vui lòng nhập đúng định dạng số.";
                return false;
            }

            if (!IsInBounds(decimal.Parse(PenaltyRate), 0, 1) ||
                !IsInBounds(decimal.Parse(MinDepositRate), 0, 1) ||
                !IsInBounds(decimal.Parse(MinAdvanceBookingRate), 0, 1))
            {
                EditMessage = "Vui lòng nhập số trong khoảng từ 0 đến 1.";
                return false;
            }

            EditMessage = string.Empty;
            return true;
        }

        private void EditParameters(System.Collections.Generic.List<ParameterDTO> parameterList)
        {
            if (parameterList == null)
                return;

            try
            {
                var enablePenalty = parameterList.FirstOrDefault(x => x.ParameterName == "EnablePenalty");
                var penaltyRate = parameterList.FirstOrDefault(x => x.ParameterName == "PenaltyRate");
                var minDepositRate = parameterList.FirstOrDefault(x => x.ParameterName == "MinDepositRate");
                var minAdvanceBookingRate = parameterList.FirstOrDefault(x => x.ParameterName == "MinReserveTableRate");

                if (enablePenalty == null || penaltyRate == null || minDepositRate == null || minAdvanceBookingRate == null)
                {
                    MessageBox.Show("Không tìm thấy tham số trong cơ sở dữ liệu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                enablePenalty.ParameterName = "EnablePenalty";
                decimal value = EnablePenaltyText == "Có" ? 1 : 0;
                enablePenalty.Value = value;

                penaltyRate.ParameterName = "PenaltyRate";
                penaltyRate.Value = decimal.Parse(PenaltyRate);

                minDepositRate.ParameterName = "MinDepositRate";
                minDepositRate.Value = decimal.Parse(MinDepositRate);

                minAdvanceBookingRate.ParameterName = "MinReserveTableRate";
                minAdvanceBookingRate.Value = decimal.Parse(MinAdvanceBookingRate);

                _parameterService.Update(penaltyRate);
                _parameterService.Update(minDepositRate);
                _parameterService.Update(minAdvanceBookingRate);
                _parameterService.Update(enablePenalty);

                MessageBox.Show("Cập nhật tham số thành công", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                EditMessage = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật tham số: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Helpers
        private void Reset(System.Collections.Generic.List<ParameterDTO> parameterList)
        {
            var enablePenalty = parameterList.FirstOrDefault(x => x.ParameterName == "EnablePenalty");
            var penaltyRate = parameterList.FirstOrDefault(x => x.ParameterName == "PenaltyRate");
            var minDepositRate = parameterList.FirstOrDefault(x => x.ParameterName == "MinDepositRate");
            var minAdvanceBookingRate = parameterList.FirstOrDefault(x => x.ParameterName == "MinReserveTableRate");

            if (enablePenalty != null && penaltyRate != null && minDepositRate != null && minAdvanceBookingRate != null)
            {
                EnablePenalty = (decimal)enablePenalty.Value;
                PenaltyRate = penaltyRate.Value.ToString();
                MinDepositRate = minDepositRate.Value.ToString();
                MinAdvanceBookingRate = minAdvanceBookingRate.Value.ToString();
                EnablePenaltyText = EnablePenalty == 1 ? "Có" : "Không";
            }
        }
        #endregion
    }
}