using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace QuanLyTiecCuoi.Tests.UnitTests.Helpers
{
    [TestClass]
    public class PasswordHelperTests
    {
        #region Base64Encode Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("PasswordHelper")]
        [Description("Base64Encode mã hóa chu?i ??n gi?n ?úng")]
        public void Base64Encode_SimpleString_ReturnsCorrectEncoding()
        {
            // Arrange
            string input = "password";
            string expected = "cGFzc3dvcmQ=";

            // Act
            string result = PasswordHelper.Base64Encode(input);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("PasswordHelper")]
        [Description("Base64Encode v?i chu?i r?ng")]
        public void Base64Encode_EmptyString_ReturnsEmptyBase64()
        {
            // Arrange
            string input = "";

            // Act
            string result = PasswordHelper.Base64Encode(input);

            // Assert
            Assert.AreEqual("", result);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("PasswordHelper")]
        [Description("Base64Encode v?i chu?i có ký t? ??c bi?t")]
        public void Base64Encode_SpecialCharacters_ReturnsValidEncoding()
        {
            // Arrange
            string input = "p@ssw0rd!#$%";

            // Act
            string result = PasswordHelper.Base64Encode(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
            // Verify it's valid base64
            Assert.IsTrue(IsValidBase64(result));
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("PasswordHelper")]
        [Description("Base64Encode v?i ký t? Unicode (Ti?ng Vi?t)")]
        public void Base64Encode_UnicodeCharacters_ReturnsValidEncoding()
        {
            // Arrange
            string input = "m?tkh?u123";

            // Act
            string result = PasswordHelper.Base64Encode(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length > 0);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("PasswordHelper")]
        [Description("Base64Encode k?t qu? khác nhau cho input khác nhau")]
        public void Base64Encode_DifferentInputs_ReturnsDifferentResults()
        {
            // Arrange
            string input1 = "password1";
            string input2 = "password2";

            // Act
            string result1 = PasswordHelper.Base64Encode(input1);
            string result2 = PasswordHelper.Base64Encode(input2);

            // Assert
            Assert.AreNotEqual(result1, result2);
        }

        #endregion

        #region MD5Hash Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("PasswordHelper")]
        [Description("MD5Hash t?o hash ?úng cho chu?i ??n gi?n")]
        public void MD5Hash_SimpleString_ReturnsCorrectHash()
        {
            // Arrange
            string input = "test";
            // MD5 of "test" is "098f6bcd4621d373cade4e832627b4f6"
            string expected = "098f6bcd4621d373cade4e832627b4f6";

            // Act
            string result = PasswordHelper.MD5Hash(input);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("PasswordHelper")]
        [Description("MD5Hash luôn tr? v? chu?i 32 ký t?")]
        public void MD5Hash_AnyInput_Returns32Characters()
        {
            // Arrange
            string[] inputs = { "a", "password", "very long password string here", "123", "!@#$%" };

            foreach (var input in inputs)
            {
                // Act
                string result = PasswordHelper.MD5Hash(input);

                // Assert
                Assert.AreEqual(32, result.Length, $"Input '{input}' should produce 32 character hash");
            }
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("PasswordHelper")]
        [Description("MD5Hash tr? v? hash lowercase")]
        public void MD5Hash_AnyInput_ReturnsLowercaseHash()
        {
            // Arrange
            string input = "Password123";

            // Act
            string result = PasswordHelper.MD5Hash(input);

            // Assert
            Assert.AreEqual(result.ToLower(), result, "Hash should be lowercase");
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("PasswordHelper")]
        [Description("MD5Hash cùng input cho cùng k?t qu?")]
        public void MD5Hash_SameInput_ReturnsSameHash()
        {
            // Arrange
            string input = "consistentPassword";

            // Act
            string result1 = PasswordHelper.MD5Hash(input);
            string result2 = PasswordHelper.MD5Hash(input);
            string result3 = PasswordHelper.MD5Hash(input);

            // Assert
            Assert.AreEqual(result1, result2);
            Assert.AreEqual(result2, result3);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("PasswordHelper")]
        [Description("MD5Hash input khác nhau cho k?t qu? khác nhau")]
        public void MD5Hash_DifferentInputs_ReturnsDifferentHashes()
        {
            // Arrange
            string input1 = "password1";
            string input2 = "password2";

            // Act
            string result1 = PasswordHelper.MD5Hash(input1);
            string result2 = PasswordHelper.MD5Hash(input2);

            // Assert
            Assert.AreNotEqual(result1, result2);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("PasswordHelper")]
        [Description("MD5Hash v?i chu?i r?ng")]
        public void MD5Hash_EmptyString_ReturnsValidHash()
        {
            // Arrange
            string input = "";
            // MD5 of "" is "d41d8cd98f00b204e9800998ecf8427e"
            string expected = "d41d8cd98f00b204e9800998ecf8427e";

            // Act
            string result = PasswordHelper.MD5Hash(input);

            // Assert
            Assert.AreEqual(expected, result);
        }

        #endregion

        #region Combined Tests (Base64 + MD5)

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("PasswordHelper")]
        [Description("K?t h?p Base64Encode và MD5Hash nh? trong LoginViewModel")]
        public void CombinedHash_SimulateLoginFlow_ReturnsConsistentResult()
        {
            // Arrange - Simulate actual login flow
            string password = "admin123";

            // Act
            string base64 = PasswordHelper.Base64Encode(password);
            string finalHash = PasswordHelper.MD5Hash(base64);

            // Assert
            Assert.IsNotNull(finalHash);
            Assert.AreEqual(32, finalHash.Length);
            
            // Verify consistency
            string base64Again = PasswordHelper.Base64Encode(password);
            string finalHashAgain = PasswordHelper.MD5Hash(base64Again);
            Assert.AreEqual(finalHash, finalHashAgain);
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("PasswordHelper")]
        [Description("Hash khác nhau cho m?t kh?u g?n gi?ng nhau")]
        public void CombinedHash_SimilarPasswords_ReturnsDifferentHashes()
        {
            // Arrange
            string password1 = "Password";
            string password2 = "password";  // Only case difference
            string password3 = "Password1"; // With number

            // Act
            string hash1 = PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(password1));
            string hash2 = PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(password2));
            string hash3 = PasswordHelper.MD5Hash(PasswordHelper.Base64Encode(password3));

            // Assert
            Assert.AreNotEqual(hash1, hash2, "Case-different passwords should have different hashes");
            Assert.AreNotEqual(hash1, hash3, "Different passwords should have different hashes");
            Assert.AreNotEqual(hash2, hash3, "Different passwords should have different hashes");
        }

        #endregion

        #region Security Tests

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("PasswordHelper")]
        [TestCategory("Security")]
        [Description("Hash không ch?a m?t kh?u g?c")]
        public void MD5Hash_ResultDoesNotContainOriginalPassword()
        {
            // Arrange
            string password = "secretpassword";

            // Act
            string hash = PasswordHelper.MD5Hash(password);

            // Assert
            Assert.IsFalse(hash.Contains(password), "Hash should not contain original password");
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        [TestCategory("PasswordHelper")]
        [TestCategory("Security")]
        [Description("Hash ch? ch?a ký t? hex")]
        public void MD5Hash_ResultContainsOnlyHexCharacters()
        {
            // Arrange
            string input = "TestPassword123!@#";

            // Act
            string result = PasswordHelper.MD5Hash(input);

            // Assert
            foreach (char c in result)
            {
                Assert.IsTrue(
                    (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f'),
                    $"Character '{c}' is not a valid hex character");
            }
        }

        #endregion

        #region Helper Methods

        private bool IsValidBase64(string base64String)
        {
            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
