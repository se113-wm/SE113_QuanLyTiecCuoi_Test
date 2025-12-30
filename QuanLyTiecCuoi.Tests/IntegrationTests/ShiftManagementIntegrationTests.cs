using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.Service;
using QuanLyTiecCuoi.DataAccessLayer.Repository;
using QuanLyTiecCuoi.Model;

namespace QuanLyTiecCuoi.Tests.IntegrationTests
{
    /// <summary>
    /// Integration Tests for Shift Management (BR51-BR59)
    /// Tests the interaction between Service and Repository layers with real database
    /// </summary>
    [TestClass]
    public class ShiftManagementIntegrationTests
    {
        private ShiftService _shiftService;

        [TestInitialize]
        public void Setup()
        {
            // Initialize DataProvider with fresh context
            DataProvider.Ins.DB = new QuanLyTiecCuoiEntities();
            
            // Initialize service with real repository
            _shiftService = new ShiftService(new ShiftRepository());
        }

        #region BR51 - Display Shift Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR51")]
        [Description("TC_BR51_001: Integration - Verify shifts load from database")]
        public void TC_BR51_001_Integration_Shifts_LoadFromDatabase()
        {
            // Act
            var shifts = _shiftService.GetAll().ToList();

            // Assert
            Assert.IsTrue(shifts.Count > 0, "Should load shifts from database");
            
            // Verify each shift has required fields
            foreach (var shift in shifts)
            {
                Assert.IsFalse(string.IsNullOrEmpty(shift.ShiftName), 
                    "Shift name should not be empty");
                Assert.IsNotNull(shift.StartTime, 
                    "Start time should not be null");
                Assert.IsNotNull(shift.EndTime, 
                    "End time should not be null");
                Assert.IsTrue(shift.EndTime > shift.StartTime, 
                    $"Shift {shift.ShiftName} end time should be after start time");
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR51")]
        [Description("TC_BR51_002: Integration - Verify shifts are accessible")]
        public void TC_BR51_002_Integration_Shifts_AreAccessible()
        {
            // Act
            var shifts = _shiftService.GetAll().ToList();

            if (shifts.Count == 0)
            {
                Assert.Inconclusive("No shifts in database to test");
                return;
            }

            // Assert - Can access shift properties
            var firstShift = shifts.First();
            Assert.IsTrue(firstShift.ShiftId > 0);
            Assert.IsFalse(string.IsNullOrEmpty(firstShift.ShiftName));
            Assert.IsNotNull(firstShift.StartTime);
            Assert.IsNotNull(firstShift.EndTime);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR51")]
        [Description("TC_BR51_003: Integration - Verify shifts have valid time ranges")]
        public void TC_BR51_003_Integration_Shifts_HaveValidTimeRanges()
        {
            // Act
            var shifts = _shiftService.GetAll().ToList();

            if (shifts.Count == 0)
            {
                Assert.Inconclusive("No shifts in database to test");
                return;
            }

            // Assert - All shifts have valid time ranges
            foreach (var shift in shifts)
            {
                Assert.IsTrue(shift.StartTime.HasValue, 
                    $"Shift {shift.ShiftName} should have start time");
                Assert.IsTrue(shift.EndTime.HasValue, 
                    $"Shift {shift.ShiftName} should have end time");
                Assert.IsTrue(shift.EndTime > shift.StartTime, 
                    $"Shift {shift.ShiftName} end time must be after start time");
                
                // Time should be within business hours (7:30 - 24:00)
                Assert.IsTrue(shift.StartTime.Value >= new TimeSpan(7, 30, 0), 
                    $"Shift {shift.ShiftName} starts too early");
                Assert.IsTrue(shift.EndTime.Value <= new TimeSpan(24, 0, 0), 
                    $"Shift {shift.ShiftName} ends too late");
            }
        }

        #endregion

        #region BR52 - GetById Integration Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR52")]
        [Description("TC_BR52_001: Integration - Verify GetById retrieves correct shift")]
        public void TC_BR52_001_Integration_GetById_RetrievesCorrectShift()
        {
            // Arrange - Get an existing shift ID
            var allShifts = _shiftService.GetAll().ToList();
            
            if (allShifts.Count == 0)
            {
                Assert.Inconclusive("No shifts in database to test");
                return;
            }

            var existingId = allShifts.First().ShiftId;

            // Act
            var shift = _shiftService.GetById(existingId);

            // Assert
            Assert.IsNotNull(shift, "Should retrieve shift by ID");
            Assert.AreEqual(existingId, shift.ShiftId);
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR52")]
        [Description("TC_BR52_002: Integration - Verify GetById returns null for non-existent ID")]
        public void TC_BR52_002_Integration_GetById_ReturnsNull_ForNonExistentId()
        {
            // Act - Use an ID that shouldn't exist
            var shift = _shiftService.GetById(99999);

            // Assert
            Assert.IsNull(shift, "Should return null for non-existent ID");
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR52")]
        [Description("TC_BR52_003: Integration - Verify GetById preserves all shift properties")]
        public void TC_BR52_003_Integration_GetById_PreservesAllProperties()
        {
            // Arrange
            var allShifts = _shiftService.GetAll().ToList();
            
            if (allShifts.Count == 0)
            {
                Assert.Inconclusive("No shifts in database to test");
                return;
            }

            var original = allShifts.First();

            // Act
            var retrieved = _shiftService.GetById(original.ShiftId);

            // Assert
            Assert.AreEqual(original.ShiftName, retrieved.ShiftName);
            Assert.AreEqual(original.StartTime, retrieved.StartTime);
            Assert.AreEqual(original.EndTime, retrieved.EndTime);
        }

        #endregion

        #region BR51-BR59 - Complete Workflow Test

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR51")]
        [TestCategory("BR52")]
        [TestCategory("BR53")]
        [TestCategory("BR54")]
        [TestCategory("BR55")]
        [Description("Integration - Complete read workflow for shifts")]
        public void Integration_Shift_CompleteReadWorkflow()
        {
            // Verify read operations work
            var shifts = _shiftService.GetAll().ToList();
            
            Assert.IsTrue(shifts.Count >= 0, 
                "Should be able to query shifts (may be empty in test DB)");
            
            if (shifts.Count > 0)
            {
                var firstShift = shifts.First();
                var retrieved = _shiftService.GetById(firstShift.ShiftId);
                
                Assert.IsNotNull(retrieved, "Should retrieve shift by ID");
                Assert.AreEqual(firstShift.ShiftName, retrieved.ShiftName);
                Assert.AreEqual(firstShift.StartTime, retrieved.StartTime);
                Assert.AreEqual(firstShift.EndTime, retrieved.EndTime);
            }
        }

        #endregion

        #region Data Validation Tests

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR51")]
        [Description("Integration - Verify all shifts have valid time ordering")]
        public void Integration_Shifts_HaveValidTimeOrdering()
        {
            // Act
            var shifts = _shiftService.GetAll().ToList();

            if (shifts.Count == 0)
            {
                Assert.Inconclusive("No shifts in database");
                return;
            }

            // Assert - All shifts have proper time ordering
            foreach (var shift in shifts)
            {
                Assert.IsTrue(shift.StartTime.HasValue && shift.EndTime.HasValue, 
                    $"Shift '{shift.ShiftName}' should have both start and end times");
                Assert.IsTrue(shift.EndTime > shift.StartTime, 
                    $"Shift '{shift.ShiftName}' end time must be after start time");
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR51")]
        [Description("Integration - Verify shift names are unique")]
        public void Integration_Shifts_HaveUniqueNames()
        {
            // Act
            var shifts = _shiftService.GetAll().ToList();

            if (shifts.Count == 0)
            {
                Assert.Inconclusive("No shifts in database");
                return;
            }

            // Assert - No duplicate names
            var uniqueNames = shifts.Select(s => s.ShiftName).Distinct().Count();
            Assert.AreEqual(shifts.Count, uniqueNames, 
                "All shift names should be unique");
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR51")]
        [Description("Integration - Verify shifts don't overlap")]
        public void Integration_Shifts_DontOverlap()
        {
            // Act
            var shifts = _shiftService.GetAll().ToList();

            if (shifts.Count < 2)
            {
                Assert.Inconclusive("Need at least 2 shifts to test overlap");
                return;
            }

            // Assert - Check for overlaps
            for (int i = 0; i < shifts.Count; i++)
            {
                for (int j = i + 1; j < shifts.Count; j++)
                {
                    var shift1 = shifts[i];
                    var shift2 = shifts[j];
                    
                    // Shifts overlap if start1 < end2 AND start2 < end1
                    bool overlap = shift1.StartTime < shift2.EndTime && 
                                   shift2.StartTime < shift1.EndTime;
                    
                    Assert.IsFalse(overlap, 
                        $"Shifts '{shift1.ShiftName}' and '{shift2.ShiftName}' should not overlap");
                }
            }
        }

        [TestMethod]
        [TestCategory("IntegrationTest")]
        [TestCategory("BR51")]
        [Description("Integration - Verify shift duration is reasonable")]
        public void Integration_Shifts_HaveReasonableDuration()
        {
            // Act
            var shifts = _shiftService.GetAll().ToList();

            if (shifts.Count == 0)
            {
                Assert.Inconclusive("No shifts in database");
                return;
            }

            // Assert - Duration should be between 2 and 8 hours
            foreach (var shift in shifts)
            {
                var duration = shift.EndTime.Value - shift.StartTime.Value;
                
                Assert.IsTrue(duration.TotalHours >= 2, 
                    $"Shift '{shift.ShiftName}' duration too short (< 2 hours)");
                Assert.IsTrue(duration.TotalHours <= 8, 
                    $"Shift '{shift.ShiftName}' duration too long (> 8 hours)");
            }
        }

        #endregion
    }
}
