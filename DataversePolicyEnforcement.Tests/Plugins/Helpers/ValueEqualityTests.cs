using DataversePolicyEnforcement.Plugins.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DataversePolicyEnforcement.Tests.Plugins.Helpers
{
    [TestClass]
    public class ValueEqualityTests
    {
        [TestMethod]
        [TestCategory("Null Values")]
        public void AreEqual_SameReference_ReturnsTrue()
        {
            var obj = new object();
            Assert.IsTrue(ValueEquality.AreEqual(obj, obj));
        }

        [TestMethod]
        [TestCategory("Null Values")]
        public void AreEqual_BothNull_ReturnsTrue()
        {
            Assert.IsTrue(ValueEquality.AreEqual(null, null));
        }

        [TestMethod]
        [TestCategory("Null Values")]
        public void AreEqual_OneNull_ReturnsFalse()
        {
            Assert.IsFalse(ValueEquality.AreEqual(null, new object()));
            Assert.IsFalse(ValueEquality.AreEqual(new object(), null));
        }

        [TestMethod]
        [TestCategory("Primitive Types")]
        public void AreEqual_Primitives_ReturnsExpected()
        {
            Assert.IsTrue(ValueEquality.AreEqual(5, 5));
            Assert.IsFalse(ValueEquality.AreEqual(5, 10));
            Assert.IsTrue(ValueEquality.AreEqual("test", "test"));
            Assert.IsFalse(ValueEquality.AreEqual("test", "Test"));

            Assert.IsTrue(ValueEquality.AreEqual(true, true));
            Assert.IsFalse(ValueEquality.AreEqual(true, false));

            Assert.IsTrue(ValueEquality.AreEqual(3.14, 3.14));
            Assert.IsFalse(ValueEquality.AreEqual(3.14, 2.71));
        }

        [TestMethod]
        [TestCategory("Aliased Values")]
        public void AreEqual_AliasedValue_UnwrapsAndCompares()
        {
            var aliasedA = new Microsoft.Xrm.Sdk.AliasedValue("alias", "v", 5);
            var aliasedB = new Microsoft.Xrm.Sdk.AliasedValue("alias", "v", 5);
            var aliasedC = new Microsoft.Xrm.Sdk.AliasedValue("alias", "v", 10);
            Assert.IsTrue(ValueEquality.AreEqual(aliasedA, aliasedB));
            Assert.IsFalse(ValueEquality.AreEqual(aliasedA, aliasedC));
        }

        [TestMethod]
        [TestCategory("OptionSetValue")]
        public void AreEqual_OptionSetValue_ComparesValues()
        {
            var osA = new Microsoft.Xrm.Sdk.OptionSetValue(1);
            var osB = new Microsoft.Xrm.Sdk.OptionSetValue(1);
            var osC = new Microsoft.Xrm.Sdk.OptionSetValue(2);
            Assert.IsTrue(ValueEquality.AreEqual(osA, osB));
            Assert.IsFalse(ValueEquality.AreEqual(osA, osC));
        }

        [TestMethod]
        [TestCategory("EntityReference")]
        public void AreEqual_EntityReference_ComparesIdAndLogicalName()
        {
            var erA = new Microsoft.Xrm.Sdk.EntityReference("account", Guid.NewGuid());
            var erB = new Microsoft.Xrm.Sdk.EntityReference("account", erA.Id);
            var erC = new Microsoft.Xrm.Sdk.EntityReference("contact", erA.Id);
            var erD = new Microsoft.Xrm.Sdk.EntityReference("account", Guid.NewGuid());
            Assert.IsTrue(ValueEquality.AreEqual(erA, erB));
            Assert.IsFalse(ValueEquality.AreEqual(erA, erC));
            Assert.IsFalse(ValueEquality.AreEqual(erA, erD));
        }

        [TestMethod]
        [TestCategory("Money")]
        public void AreEqual_Money_ComparesValues()
        {
            var mA = new Microsoft.Xrm.Sdk.Money(100);
            var mB = new Microsoft.Xrm.Sdk.Money(100);
            var mC = new Microsoft.Xrm.Sdk.Money(200);
            Assert.IsTrue(ValueEquality.AreEqual(mA, mB));
            Assert.IsFalse(ValueEquality.AreEqual(mA, mC));
        }

        [TestMethod]
        [TestCategory("DateTime")]
        public void AreEqual_DateTime_ComparesValues()
        {
            var dtA = new DateTime(2024, 1, 1);
            var dtB = new DateTime(2024, 1, 1);
            var dtC = new DateTime(2025, 1, 1);
            Assert.IsTrue(ValueEquality.AreEqual(dtA, dtB));
            Assert.IsFalse(ValueEquality.AreEqual(dtA, dtC));
        }
    }
}
