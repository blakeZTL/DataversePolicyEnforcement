using DataversePolicyEnforcement.Core.Comparison;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DataversePolicyEnforcement.Tests.Core.Comparison
{
    [TestClass]
    public class ConditionValueEquals_Tests
    {
        [TestMethod]
        public void ConditionValueEquals_NullCondition_ReturnsFalse()
        {
            Assert.IsFalse(Condition.ConditionValueEquals(null, "AnyValue"));
        }

        [TestMethod]
        public void ConditionValueEquals_NullValue_ReturnsFalse()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.String,
                dpe_ValueString = "TestValue"
            };
            Assert.IsFalse(Condition.ConditionValueEquals(condition, null));
        }

        [TestMethod]
        public void ConditionValueEquals_UnsupportedValueType_ReturnsFalse()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = (Models.OptionSets.dpe_policyconditionvaluetype)999 // Invalid enum value
            };
            Assert.IsFalse(Condition.ConditionValueEquals(condition, "AnyValue"));
        }

        [TestMethod]
        public void ConditionValueEquals_String_Match()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.String,
                dpe_ValueString = "TestValue"
            };
            Assert.IsTrue(Condition.ConditionValueEquals(condition, "TestValue"));
        }

        [TestMethod]
        public void ConditionValueEquals_String_NoMatch()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.String,
                dpe_ValueString = "TestValue"
            };
            Assert.IsFalse(Condition.ConditionValueEquals(condition, "AnotherValue"));
        }

        [TestMethod]
        public void ConditionValueEquals_WholeNumber_Match()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.WholeNumber,
                dpe_ValueWholeNumber = 42
            };
            Assert.IsTrue(Condition.ConditionValueEquals(condition, 42));
        }

        [TestMethod]
        public void ConditionValueEquals_WholeNumber_NoMatch()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.WholeNumber,
                dpe_ValueWholeNumber = 42
            };
            Assert.IsFalse(Condition.ConditionValueEquals(condition, 43));
        }

        [TestMethod]
        public void ConditionValueEquals_Boolean_Match()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.Boolean,
                dpe_ValueBoolean = true
            };
            Assert.IsTrue(Condition.ConditionValueEquals(condition, true));
        }

        [TestMethod]
        public void ConditionValueEquals_Boolean_NoMatch()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.Boolean,
                dpe_ValueBoolean = true
            };
            Assert.IsFalse(Condition.ConditionValueEquals(condition, false));
        }

        [TestMethod]
        public void ConditionValueEquals_DateTime_Match()
        {
            var date = new DateTime(2024, 1, 1);
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.DateTime,
                dpe_ValueDateTime = date
            };
            Assert.IsTrue(Condition.ConditionValueEquals(condition, date));
        }

        [TestMethod]
        public void ConditionValueEquals_DateTime_NoMatch()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.DateTime,
                dpe_ValueDateTime = new DateTime(2024, 1, 1)
            };
            Assert.IsFalse(Condition.ConditionValueEquals(condition, new DateTime(2024, 1, 2)));
        }

        [TestMethod]
        public void ConditionValueEquals_Guid_Match()
        {
            var guid = System.Guid.NewGuid();
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.Guid,
                dpe_ValueGuid = guid.ToString()
            };
            Assert.IsTrue(Condition.ConditionValueEquals(condition, guid));
        }

        [TestMethod]
        public void ConditionValueEquals_Guid_NoMatch()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.Guid,
                dpe_ValueGuid = System.Guid.NewGuid().ToString()
            };
            Assert.IsFalse(Condition.ConditionValueEquals(condition, System.Guid.NewGuid()));
        }

        [TestMethod]
        public void ConditionValueEquals_Money_Match()
        {
            var money = new Microsoft.Xrm.Sdk.Money(100);
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.Money,
                dpe_ValueMoney = money
            };
            Assert.IsTrue(Condition.ConditionValueEquals(condition, money));
        }

        [TestMethod]
        public void ConditionValueEquals_Money_NoMatch()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.Money,
                dpe_ValueMoney = new Microsoft.Xrm.Sdk.Money(100)
            };
            Assert.IsFalse(Condition.ConditionValueEquals(condition, 101));
        }

        [TestMethod]
        public void ConditionValueEquals_OptionSet_Match()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.OptionSet,
                dpe_ValueOptionSetValue = 1
            };
            Assert.IsTrue(Condition.ConditionValueEquals(condition, 1));
        }

        [TestMethod]
        public void ConditionValueEquals_OptionSet_NoMatch()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.OptionSet,
                dpe_ValueOptionSetValue = 1
            };
            Assert.IsFalse(Condition.ConditionValueEquals(condition, 2));
        }

        [TestMethod]
        public void ConditionValueEquals_Lookup_Match()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.Lookup,
                dpe_ValueLookupLogicalName = "account",
                dpe_ValueLookupId = Guid.NewGuid().ToString()
            };
            var entityRef = new Microsoft.Xrm.Sdk.EntityReference(
                condition.dpe_ValueLookupLogicalName,
                Guid.Parse(condition.dpe_ValueLookupId)
            );
            Assert.IsTrue(Condition.ConditionValueEquals(condition, entityRef));
        }

        [TestMethod]
        public void ConditionValueEquals_Lookup_NoMatch()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.Lookup,
                dpe_ValueLookupLogicalName = "account",
                dpe_ValueLookupId = Guid.NewGuid().ToString()
            };
            var entityRef = new Microsoft.Xrm.Sdk.EntityReference(
                condition.dpe_ValueLookupLogicalName,
                Guid.NewGuid()
            );
            Assert.IsFalse(Condition.ConditionValueEquals(condition, entityRef));
        }

        [TestMethod]
        public void ConditionValueEquals_Lookup_WrongLogicalName_ReturnsFalse()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.Lookup,
                dpe_ValueLookupLogicalName = "account",
                dpe_ValueLookupId = Guid.NewGuid().ToString()
            };
            var entityRef = new Microsoft.Xrm.Sdk.EntityReference("another_entity", Guid.NewGuid());
            Assert.IsFalse(Condition.ConditionValueEquals(condition, entityRef));
        }

        [TestMethod]
        public void ConditionValueEquals_Lookup_NullLogicalName_ReturnsFalse()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.Lookup,
                dpe_ValueLookupLogicalName = null,
                dpe_ValueLookupId = Guid.NewGuid().ToString()
            };
            var entityRef = new Microsoft.Xrm.Sdk.EntityReference(
                "entity_name",
                Guid.Parse(condition.dpe_ValueLookupId)
            );
            Assert.IsFalse(Condition.ConditionValueEquals(condition, entityRef));
        }

        [TestMethod]
        public void ConditionValueEquals_Lookup_NullId_ReturnsFalse()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.Lookup,
                dpe_ValueLookupLogicalName = "account",
                dpe_ValueLookupId = null
            };
            var entityRef = new Microsoft.Xrm.Sdk.EntityReference(
                condition.dpe_ValueLookupLogicalName,
                Guid.NewGuid()
            );
            Assert.IsFalse(Condition.ConditionValueEquals(condition, entityRef));
        }

        [TestMethod]
        public void ConditionValueEquals_Lookup_NullValue_ReturnsFalse()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.Lookup,
                dpe_ValueLookupLogicalName = "account",
                dpe_ValueLookupId = Guid.NewGuid().ToString()
            };
            Assert.IsFalse(Condition.ConditionValueEquals(condition, null));
        }

        [TestMethod]
        public void ConditionValueEquals_Lookup_WrongType_ReturnsFalse()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.Lookup,
                dpe_ValueLookupLogicalName = "account",
                dpe_ValueLookupId = Guid.NewGuid().ToString()
            };
            Assert.IsFalse(Condition.ConditionValueEquals(condition, 123));
        }

        [TestMethod]
        public void ConditionValueEquals_String_CaseInsensitiveMatch()
        {
            var condition = new Models.Entities.dpe_PolicyCondition
            {
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.String,
                dpe_ValueString = "TestValue"
            };
            Assert.IsTrue(Condition.ConditionValueEquals(condition, "testvalue"));
        }
    }
}
