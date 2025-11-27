using System.Collections.Generic;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.BusinessLogicLayer.Service
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public IEnumerable<BookingDTO> GetAll()
        {
            return _bookingRepository.GetAll()
                .Select(x => new BookingDTO
                {
                    BookingId = x.BookingId,
                    GroomName = x.GroomName,
                    BrideName = x.BrideName,
                    Phone = x.Phone,
                    BookingDate = x.BookingDate,
                    WeddingDate = x.WeddingDate,
                    ShiftId = x.ShiftId,
                    HallId = x.HallId,
                    Deposit = x.Deposit,
                    TableCount = x.TableCount,
                    ReserveTableCount = x.ReserveTableCount,
                    PaymentDate = x.PaymentDate,
                    TablePrice = x.TablePrice,
                    TotalTableAmount = x.TotalTableAmount,
                    TotalServiceAmount = x.TotalServiceAmount,
                    TotalInvoiceAmount = x.TotalInvoiceAmount,
                    RemainingAmount = x.RemainingAmount,
                    AdditionalCost = x.AdditionalCost,
                    PenaltyAmount = x.PenaltyAmount,
                    Shift = x.Shift != null
                        ? new ShiftDTO
                        {
                            ShiftId = x.Shift.ShiftId,
                            ShiftName = x.Shift.ShiftName,
                            StartTime = x.Shift.StartTime,
                            EndTime = x.Shift.EndTime
                        }
                        : null,
                    Hall = x.Hall != null
                        ? new HallDTO
                        {
                            HallId = x.Hall.HallId,
                            HallTypeId = x.Hall.HallTypeId,
                            HallName = x.Hall.HallName,
                            MaxTableCount = x.Hall.MaxTableCount,
                            Note = x.Hall.Note,
                            HallType = x.Hall.HallType != null
                                ? new HallTypeDTO
                                {
                                    HallTypeId = x.Hall.HallType.HallTypeId,
                                    HallTypeName = x.Hall.HallType.HallTypeName,
                                    MinTablePrice = x.Hall.HallType.MinTablePrice
                                }
                                : null
                        }
                        : null
                });
        }

        public BookingDTO GetById(int bookingId)
        {
            var x = _bookingRepository.GetById(bookingId);
            if (x == null) return null;
            return new BookingDTO
            {
                BookingId = x.BookingId,
                GroomName = x.GroomName,
                BrideName = x.BrideName,
                Phone = x.Phone,
                BookingDate = x.BookingDate,
                WeddingDate = x.WeddingDate,
                ShiftId = x.ShiftId,
                HallId = x.HallId,
                Deposit = x.Deposit,
                TableCount = x.TableCount,
                ReserveTableCount = x.ReserveTableCount,
                PaymentDate = x.PaymentDate,
                TablePrice = x.TablePrice,
                TotalTableAmount = x.TotalTableAmount,
                TotalServiceAmount = x.TotalServiceAmount,
                TotalInvoiceAmount = x.TotalInvoiceAmount,
                RemainingAmount = x.RemainingAmount,
                AdditionalCost = x.AdditionalCost,
                PenaltyAmount = x.PenaltyAmount,
                Shift = x.Shift != null
                    ? new ShiftDTO
                    {
                        ShiftId = x.Shift.ShiftId,
                        ShiftName = x.Shift.ShiftName,
                        StartTime = x.Shift.StartTime,
                        EndTime = x.Shift.EndTime
                    }
                    : null,
                Hall = x.Hall != null
                    ? new HallDTO
                    {
                        HallId = x.Hall.HallId,
                        HallTypeId = x.Hall.HallTypeId,
                        HallName = x.Hall.HallName,
                        MaxTableCount = x.Hall.MaxTableCount,
                        Note = x.Hall.Note,
                        HallType = x.Hall.HallType != null
                            ? new HallTypeDTO
                            {
                                HallTypeId = x.Hall.HallType.HallTypeId,
                                HallTypeName = x.Hall.HallType.HallTypeName,
                                MinTablePrice = x.Hall.HallType.MinTablePrice
                            }
                            : null
                    }
                    : null
            };
        }

        public void Create(BookingDTO bookingDto)
        {
            var entity = new Booking
            {
                BookingId = bookingDto.BookingId,
                GroomName = bookingDto.GroomName,
                BrideName = bookingDto.BrideName,
                Phone = bookingDto.Phone,
                BookingDate = bookingDto.BookingDate,
                WeddingDate = bookingDto.WeddingDate,
                ShiftId = bookingDto.ShiftId,
                HallId = bookingDto.HallId,
                Deposit = bookingDto.Deposit,
                TableCount = bookingDto.TableCount,
                ReserveTableCount = bookingDto.ReserveTableCount,
                PaymentDate = bookingDto.PaymentDate,
                TablePrice = bookingDto.TablePrice,
                TotalTableAmount = bookingDto.TotalTableAmount,
                TotalServiceAmount = bookingDto.TotalServiceAmount,
                TotalInvoiceAmount = bookingDto.TotalInvoiceAmount,
                RemainingAmount = bookingDto.RemainingAmount,
                AdditionalCost = bookingDto.AdditionalCost,
                PenaltyAmount = bookingDto.PenaltyAmount
            };
            _bookingRepository.Create(entity);
            bookingDto.BookingId = entity.BookingId;
        }

        public void Update(BookingDTO bookingDto)
        {
            var entity = new Booking
            {
                BookingId = bookingDto.BookingId,
                GroomName = bookingDto.GroomName,
                BrideName = bookingDto.BrideName,
                Phone = bookingDto.Phone,
                BookingDate = bookingDto.BookingDate,
                WeddingDate = bookingDto.WeddingDate,
                ShiftId = bookingDto.ShiftId,
                HallId = bookingDto.HallId,
                Deposit = bookingDto.Deposit,
                TableCount = bookingDto.TableCount,
                ReserveTableCount = bookingDto.ReserveTableCount,
                PaymentDate = bookingDto.PaymentDate,
                TablePrice = bookingDto.TablePrice,
                TotalTableAmount = bookingDto.TotalTableAmount,
                TotalServiceAmount = bookingDto.TotalServiceAmount,
                TotalInvoiceAmount = bookingDto.TotalInvoiceAmount,
                RemainingAmount = bookingDto.RemainingAmount,
                AdditionalCost = bookingDto.AdditionalCost,
                PenaltyAmount = bookingDto.PenaltyAmount
            };
            _bookingRepository.Update(entity);
        }

        public void Delete(int bookingId)
        {
            _bookingRepository.Delete(bookingId);
        }
    }
}