using DataversePolicyEnforcement.Core.Data;
using FakeXrmEasy.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk.Metadata;
using System.Collections.Generic;

namespace DataversePolicyEnforcement.Tests.Core.Data
{
    public abstract class MetadataValidatorTestsBase : FakeXrmEasyTestBase
    {
        protected readonly MetadataValidator _validator;
        protected EntityMetadata _accountMetadata;
        protected AttributeMetadata _nameAttributeMetadata;
        protected AttributeMetadata _accountNumberAttributeMetadata;

        public MetadataValidatorTestsBase()
        {
            _validator = new MetadataValidator(_context.GetOrganizationService());

            _nameAttributeMetadata = new AttributeMetadata { LogicalName = "name" };
            _accountNumberAttributeMetadata = new AttributeMetadata
            {
                LogicalName = "accountnumber"
            };

            _accountMetadata = new EntityMetadata { LogicalName = "account" };
            _accountMetadata.SetAttribute(_nameAttributeMetadata);
            _accountMetadata.SetAttribute(_accountNumberAttributeMetadata);

            _context.InitializeMetadata(new List<EntityMetadata> { _accountMetadata });
        }

        [TestClass]
        public class ValidateAttributeTests : MetadataValidatorTestsBase
        {
            [TestMethod]
            public void ValidateAttribute_ValidAttribute_ReturnsTrue()
            {
                var result = _validator.ValidateAttribute("account", "name");
                Assert.IsTrue(result);
            }

            [TestMethod]
            public void ValidateAttribute_InvalidAttribute_ReturnsFalse()
            {
                var result = _validator.ValidateAttribute("account", "invalidattribute");
                Assert.IsFalse(result);
            }

            [TestMethod]
            public void ValidateAttribute_InvalidEntity_ReturnsFalse()
            {
                var result = _validator.ValidateAttribute("invalidentity", "name");
                Assert.IsFalse(result);
            }

            [TestMethod]
            public void ValidateAttribute_NullOrWhitespaceParameters_ReturnsFalse()
            {
                Assert.IsFalse(_validator.ValidateAttribute(null, "name"));
                Assert.IsFalse(_validator.ValidateAttribute("account", null));
                Assert.IsFalse(_validator.ValidateAttribute("", "name"));
                Assert.IsFalse(_validator.ValidateAttribute("account", ""));
                Assert.IsFalse(_validator.ValidateAttribute("   ", "name"));
                Assert.IsFalse(_validator.ValidateAttribute("account", "   "));
            }
        }

        [TestClass]
        public class ValidateAllAttributesTests : MetadataValidatorTestsBase
        {
            [TestMethod]
            public void ValidateAllAttributes_ValidAttributes_ReturnsTrue()
            {
                var result = _validator.ValidateAllAttributes(
                    "account",
                    new List<string> { "name", "accountnumber" }
                );
                Assert.IsTrue(result);
            }

            [TestMethod]
            public void ValidateAllAttributes_SomeInvalidAttributes_ReturnsFalse()
            {
                var result = _validator.ValidateAllAttributes(
                    "account",
                    new List<string> { "name", "invalidattribute" }
                );
                Assert.IsFalse(result);
            }

            [TestMethod]
            public void ValidateAllAttributes_InvalidEntity_ReturnsFalse()
            {
                var result = _validator.ValidateAllAttributes(
                    "invalidentity",
                    new List<string> { "name", "accountnumber" }
                );
                Assert.IsFalse(result);
            }

            [TestMethod]
            public void ValidateAllAttributes_NullOrWhitespaceEntity_ReturnsFalse()
            {
                Assert.IsFalse(_validator.ValidateAllAttributes(null, new List<string> { "name" }));
                Assert.IsFalse(_validator.ValidateAllAttributes("", new List<string> { "name" }));
                Assert.IsFalse(
                    _validator.ValidateAllAttributes("   ", new List<string> { "name" })
                );
            }

            [TestMethod]
            public void ValidateAllAttributes_NullOrEmptyAttributeList_ReturnsTrue()
            {
                Assert.IsTrue(_validator.ValidateAllAttributes("account", null));
                Assert.IsTrue(_validator.ValidateAllAttributes("account", new List<string>()));
            }
        }

        [TestClass]
        public class ValidateEntityTests : MetadataValidatorTestsBase
        {
            [TestMethod]
            public void ValidateEntity_ValidEntity_ReturnsTrue()
            {
                var result = _validator.ValidateEntity("account");
                Assert.IsTrue(result);
            }

            [TestMethod]
            public void ValidateEntity_InvalidEntity_ReturnsFalse()
            {
                var result = _validator.ValidateEntity("invalidentity");
                Assert.IsFalse(result);
            }

            [TestMethod]
            public void ValidateEntity_NullOrWhitespaceEntity_ReturnsFalse()
            {
                Assert.IsFalse(_validator.ValidateEntity(null));
                Assert.IsFalse(_validator.ValidateEntity(""));
                Assert.IsFalse(_validator.ValidateEntity("   "));
            }
        }
    }
}
