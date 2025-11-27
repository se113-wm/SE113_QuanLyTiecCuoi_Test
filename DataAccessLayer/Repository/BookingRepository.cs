using QuanLyTiecCuoi.DataAccessLayer.IRepository;
using QuanLyTiecCuoi.Model;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace QuanLyTiecCuoi.DataAccessLayer.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly QuanLyTiecCuoiEntities _context;

        public BookingRepository()
        {
            _context = new QuanLyTiecCuoiEntities();
        }

        public IEnumerable<Booking> GetAll()
        {
            return _context.Bookings
                .Include(b => b.Hall)
                .Include(b => b.Hall.HallType)
                .Include(b => b.Shift)
                .ToList();
        }

        public Booking GetById(int bookingId)
        {
            return _context.Bookings
                .Include(b => b.Hall)
                .Include(b => b.Hall.HallType)
                .Include(b => b.Shift)
                .FirstOrDefault(b => b.BookingId == bookingId);
        }

        public void Create(Booking booking)
        {
            _context.Bookings.Add(booking);
            _context.SaveChanges();
            _context.Entry(booking).Reload();
        }

        public void Update(Booking booking)
        {
            var existing = _context.Bookings.Find(booking.BookingId);
            if (existing != null)
            {
                existing.GroomName = booking.GroomName;
                existing.BrideName = booking.BrideName;
                existing.Phone = booking.Phone;
                existing.BookingDate = booking.BookingDate;
                existing.WeddingDate = booking.WeddingDate;
                existing.ShiftId = booking.ShiftId;
                existing.HallId = booking.HallId;
                existing.Deposit = booking.Deposit;
                existing.TableCount = booking.TableCount;
                existing.ReserveTableCount = booking.ReserveTableCount;
                existing.PaymentDate = booking.PaymentDate;
                existing.TablePrice = booking.TablePrice;
                existing.TotalTableAmount = booking.TotalTableAmount;
                existing.TotalServiceAmount = booking.TotalServiceAmount;
                existing.TotalInvoiceAmount = booking.TotalInvoiceAmount;
                existing.RemainingAmount = booking.RemainingAmount;
                existing.AdditionalCost = booking.AdditionalCost;
                existing.PenaltyAmount = booking.PenaltyAmount;
                _context.SaveChanges();
                _context.Entry(existing).Reload();
            }
        }

        public void Delete(int bookingId)
        {
            var booking = _context.Bookings.Find(bookingId);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                _context.SaveChanges();
            }
        }
    }
}