using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using QuanLyTiecCuoi.BusinessLogicLayer.IService;
using QuanLyTiecCuoi.DataTransferObject;
using QuanLyTiecCuoi.ViewModel;

namespace QuanLyTiecCuoi.Tests.UnitTests.ViewModels
{
    /// <summary>
    /// Unit Tests for ServiceViewModel  
    /// Covers BR89-BR105 - Service Management
    /// </summary>
    [TestClass]
    public class ServiceViewModelTests
    {
        private Mock<IServiceService> _mockServiceService;
        private Mock<IServiceDetailService> _mockServiceDetailService;

        [TestInitialize]
        public void Setup()
        {
            _mockServiceService = new Mock<IServiceService>();
            _mockServiceDetailService = new Mock<IServiceDetailService>();

            // Setup default mock returns
            _mockServiceService.Setup(s => s.GetAll()).Returns(CreateSampleServices());
            _mockServiceDetailService.Setup(s => s.GetAll()).Returns(new List<ServiceDetailDTO>());
        }

        #region BR89 - Display Service List Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR89")]
        [Description("TC_BR89_001: Verify ServiceViewModel initializes and loads service list")]
        public void TC_BR89_001_Constructor_LoadsServiceList()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel);
            Assert.IsNotNull(viewModel.ServiceList);
            Assert.IsTrue(viewModel.ServiceList.Count > 0);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR89")]
        [Description("TC_BR89_002: Verify service list contains pricing information")]
        public void TC_BR89_002_ServiceList_ContainsPricingInfo()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            foreach (var service in viewModel.ServiceList)
            {
                Assert.IsNotNull(service.UnitPrice);
                Assert.IsTrue(service.UnitPrice > 0);
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR89")]
        [Description("TC_BR89_003: Verify original list is preserved")]
        public void TC_BR89_003_OriginalList_IsPreserved()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.OriginalList);
            Assert.AreEqual(viewModel.ServiceList.Count, viewModel.OriginalList.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR89")]
        [Description("TC_BR89_004: Verify service names are loaded")]
        public void TC_BR89_004_ServiceList_ContainsNames()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            foreach (var service in viewModel.ServiceList)
            {
                Assert.IsFalse(string.IsNullOrEmpty(service.ServiceName));
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR89")]
        [Description("TC_BR89_005: Verify services are distinct")]
        public void TC_BR89_005_ServiceList_ContainsDistinctServices()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            var uniqueNames = viewModel.ServiceList.Select(s => s.ServiceName).Distinct().Count();
            Assert.AreEqual(viewModel.ServiceList.Count, uniqueNames);
        }

        #endregion

        #region BR91 - Create Service Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR91")]
        [Description("TC_BR91_001: Verify AddCommand is initialized")]
        public void TC_BR91_001_AddCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.AddCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR91")]
        [Description("TC_BR91_002: Verify cannot add without service name")]
        public void TC_BR91_002_AddCommand_RequiresServiceName()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.ServiceName = "";
            viewModel.UnitPrice = "2000000";

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR91")]
        [Description("TC_BR91_003: Verify cannot add with invalid unit price")]
        public void TC_BR91_003_AddCommand_RequiresValidUnitPrice()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.ServiceName = "New Service";
            viewModel.UnitPrice = "abc"; // Invalid

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR91")]
        [Description("TC_BR91_004: Verify cannot add with negative price")]
        public void TC_BR91_004_AddCommand_RequiresPositivePrice()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.ServiceName = "New Service";
            viewModel.UnitPrice = "-100";

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR91")]
        [Description("TC_BR91_005: Verify cannot add duplicate service name")]
        public void TC_BR91_005_AddCommand_PreventsDuplicateServiceName()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var existingService = viewModel.ServiceList.First();
            viewModel.ServiceName = existingService.ServiceName;
            viewModel.UnitPrice = "2000000";

            // Act
            bool canExecute = viewModel.AddCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        #endregion

        #region BR92 - Update Service Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR92")]
        [Description("TC_BR92_001: Verify EditCommand is initialized")]
        public void TC_BR92_001_EditCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.EditCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR92")]
        [Description("TC_BR92_002: Verify cannot edit without selection")]
        public void TC_BR92_002_EditCommand_RequiresSelection()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.SelectedItem = null;

            // Act
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR92")]
        [Description("TC_BR92_003: Verify cannot edit with empty name")]
        public void TC_BR92_003_EditCommand_ValidatesName()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var service = viewModel.ServiceList.First();
            viewModel.IsEditing = true;
            viewModel.SelectedItem = service;
            viewModel.ServiceName = ""; // Invalid

            // Act
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR92")]
        [Description("TC_BR92_004: Verify cannot edit to duplicate name")]
        public void TC_BR92_004_EditCommand_PreventsDuplicateName()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var service = viewModel.ServiceList.First();
            var otherService = viewModel.ServiceList.Last();
            viewModel.IsEditing = true;
            viewModel.SelectedItem = service;
            viewModel.ServiceName = otherService.ServiceName;

            // Act
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR92")]
        [Description("TC_BR92_005: Verify cannot edit with invalid price")]
        public void TC_BR92_005_EditCommand_ValidatesPrice()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var service = viewModel.ServiceList.First();
            viewModel.IsEditing = true;
            viewModel.SelectedItem = service;
            viewModel.ServiceName = "Updated Name";
            viewModel.UnitPrice = "invalid";

            // Act
            bool canExecute = viewModel.EditCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        #endregion

        #region BR93 - Delete Service Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR93")]
        [Description("TC_BR93_001: Verify DeleteCommand is initialized")]
        public void TC_BR93_001_DeleteCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.DeleteCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR93")]
        [Description("TC_BR93_002: Verify cannot delete without selection")]
        public void TC_BR93_002_DeleteCommand_RequiresSelection()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.SelectedItem = null;

            // Act
            bool canExecute = viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR93")]
        [Description("TC_BR93_003: Verify cannot delete service in use")]
        public void TC_BR93_003_DeleteCommand_PreventsDeletionWhenInUse()
        {
            // Arrange
            var service = CreateSampleServices().First();
            _mockServiceDetailService.Setup(s => s.GetAll()).Returns(new List<ServiceDetailDTO>
            {
                new ServiceDetailDTO { ServiceId = service.ServiceId }
            });

            var viewModel = CreateViewModel();
            viewModel.IsDeleting = true;
            viewModel.SelectedItem = service;

            // Act
            bool canExecute = viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.IsFalse(canExecute);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR93")]
        [Description("TC_BR93_004: Verify can delete service not in use")]
        public void TC_BR93_004_DeleteCommand_AllowsDeletionWhenNotInUse()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.IsDeleting = true;
            viewModel.SelectedItem = viewModel.ServiceList.First();

            // Act
            bool canExecute = viewModel.DeleteCommand.CanExecute(null);

            // Assert
            Assert.IsTrue(canExecute);
        }

        #endregion

        #region BR94 - Search/Filter Service Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR94")]
        [Description("TC_BR94_001: Verify search by service name works")]
        public void TC_BR94_001_SearchByServiceName_FiltersCorrectly()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var searchTerm = "Photography";

            // Act
            viewModel.SelectedSearchProperty = viewModel.SearchProperties[0];
            viewModel.SearchText = searchTerm;

            // Assert
            Assert.IsTrue(viewModel.ServiceList.Count > 0);
            Assert.IsTrue(viewModel.ServiceList.All(s => 
                s.ServiceName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR94")]
        [Description("TC_BR94_002: Verify search by unit price works")]
        public void TC_BR94_002_SearchByUnitPrice_FiltersCorrectly()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var searchTerm = "4500000";

            // Act
            viewModel.SelectedSearchProperty = viewModel.SearchProperties[1];
            viewModel.SearchText = searchTerm;

            // Assert
            Assert.IsTrue(viewModel.ServiceList.Count > 0);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR94")]
        [Description("TC_BR94_003: Verify clearing search restores full list")]
        public void TC_BR94_003_ClearSearch_RestoresFullList()
        {
            // Arrange
            var viewModel = CreateViewModel();
            var originalCount = viewModel.ServiceList.Count;
            viewModel.SearchText = "Photography";

            // Act
            viewModel.SearchText = "";

            // Assert
            Assert.AreEqual(originalCount, viewModel.ServiceList.Count);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR94")]
        [Description("TC_BR94_004: Verify search is case insensitive")]
        public void TC_BR94_004_Search_IsCaseInsensitive()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            viewModel.SelectedSearchProperty = viewModel.SearchProperties[0];
            viewModel.SearchText = "photography"; // lowercase

            // Assert
            Assert.IsTrue(viewModel.ServiceList.Count > 0);
        }

        #endregion

        #region BR95 - Action Selection Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR95")]
        [Description("TC_BR95_001: Verify action list contains all actions")]
        public void TC_BR95_001_ActionList_ContainsAllActions()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.ActionList);
            Assert.AreEqual(5, viewModel.ActionList.Count);
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.ActionList[0]));
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.ActionList[1]));
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.ActionList[2]));
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.ActionList[3]));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR95")]
        [Description("TC_BR95_002: Verify selecting Add action sets IsAdding")]
        public void TC_BR95_002_SelectedAction_Add_SetsIsAdding()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            viewModel.SelectedAction = viewModel.ActionList[0];

            // Assert
            Assert.IsTrue(viewModel.IsAdding);
            Assert.IsFalse(viewModel.IsEditing);
            Assert.IsFalse(viewModel.IsDeleting);
            Assert.IsFalse(viewModel.IsExporting);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR95")]
        [Description("TC_BR95_003: Verify selecting Export action sets IsExporting")]
        public void TC_BR95_003_SelectedAction_Export_SetsIsExporting()
        {
            // Arrange
            var viewModel = CreateViewModel();

            // Act
            viewModel.SelectedAction = viewModel.ActionList[3];

            // Assert
            Assert.IsFalse(viewModel.IsAdding);
            Assert.IsFalse(viewModel.IsEditing);
            Assert.IsFalse(viewModel.IsDeleting);
            Assert.IsTrue(viewModel.IsExporting);
        }

        #endregion

        #region BR96-BR104 - Additional Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR96")]
        [Description("TC_BR96_001: Verify ResetCommand is initialized")]
        public void TC_BR96_001_ResetCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.ResetCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR97")]
        [Description("TC_BR97_001: Verify reset clears all fields")]
        public void TC_BR97_001_ResetCommand_ClearsAllFields()
        {
            // Arrange
            var viewModel = CreateViewModel();
            viewModel.ServiceName = "Test";
            viewModel.UnitPrice = "2000000";
            viewModel.Note = "Note";
            viewModel.SelectedItem = viewModel.ServiceList.First();

            // Act
            viewModel.ResetCommand.Execute(null);

            // Assert
            Assert.IsNull(viewModel.SelectedItem);
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.ServiceName));
            Assert.IsNull(viewModel.UnitPrice);
            Assert.IsTrue(string.IsNullOrEmpty(viewModel.Note));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR98")]
        [Description("TC_BR98_001: Verify ServiceName raises PropertyChanged")]
        public void TC_BR98_001_ServiceName_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.ServiceName))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.ServiceName = "New Service";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR99")]
        [Description("TC_BR99_001: Verify UnitPrice raises PropertyChanged")]
        public void TC_BR99_001_UnitPrice_RaisesPropertyChanged()
        {
            // Arrange
            var viewModel = CreateViewModel();
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.UnitPrice))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.UnitPrice = "3000000";

            // Assert
            Assert.IsTrue(propertyChangedRaised);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR103")]
        [Description("TC_BR103_001: Verify ExportToExcelCommand is initialized")]
        public void TC_BR103_001_ExportToExcelCommand_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.ExportToExcelCommand);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR104")]
        [Description("TC_BR104_001: Verify search properties list is initialized")]
        public void TC_BR104_001_SearchProperties_IsInitialized()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.SearchProperties);
            Assert.AreEqual(3, viewModel.SearchProperties.Count);
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.SearchProperties[0]));
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.SearchProperties[1]));
            Assert.IsFalse(string.IsNullOrEmpty(viewModel.SearchProperties[2]));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("ServiceViewModel")]
        [TestCategory("BR105")]
        [Description("TC_BR105_001: Verify default search property is selected")]
        public void TC_BR105_001_SelectedSearchProperty_HasDefault()
        {
            // Act
            var viewModel = CreateViewModel();

            // Assert
            Assert.IsNotNull(viewModel.SelectedSearchProperty);
            Assert.IsTrue(viewModel.SearchProperties.Contains(viewModel.SelectedSearchProperty));
        }

        #endregion

        #region Helper Methods

        private ServiceViewModel CreateViewModel()
        {
            return new ServiceViewModel(
                _mockServiceService.Object,
                _mockServiceDetailService.Object);
        }

        private List<ServiceDTO> CreateSampleServices()
        {
            return new List<ServiceDTO>
            {
                new ServiceDTO
                {
                    ServiceId = 1,
                    ServiceName = "Wedding Photography",
                    UnitPrice = 4500000,
                    Note = "Professional photographer"
                },
                new ServiceDTO
                {
                    ServiceId = 2,
                    ServiceName = "Flower Decoration",
                    UnitPrice = 3000000,
                    Note = "Fresh flowers"
                },
                new ServiceDTO
                {
                    ServiceId = 3,
                    ServiceName = "Wedding MC",
                    UnitPrice = 3500000,
                    Note = "Professional MC"
                },
                new ServiceDTO
                {
                    ServiceId = 4,
                    ServiceName = "Sound System",
                    UnitPrice = 2000000,
                    Note = "High quality audio"
                },
                new ServiceDTO
                {
                    ServiceId = 5,
                    ServiceName = "Lighting System",
                    UnitPrice = 2500000,
                    Note = "Professional lighting"
                }
            };
        }

        #endregion
    }
}
