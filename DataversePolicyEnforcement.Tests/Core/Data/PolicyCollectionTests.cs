using DataversePolicyEnforcement.Core.Data;
using DataversePolicyEnforcement.Models.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace DataversePolicyEnforcement.Tests.Core.Data
{
    public abstract class PolicyCollectionTestsBase : FakeXrmEasyTestBase
    {
        protected List<dpe_PolicyRule> _rules;
        protected List<dpe_PolicyCondition> _conditions;
        protected IOrganizationService _testService;
        protected PolicyCollection _collection;

        protected PolicyCollectionTestsBase()
        {
            _rules = new List<dpe_PolicyRule>
            {
                new dpe_PolicyRule
                {
                    Id = Guid.NewGuid(),
                    dpe_TargetEntityLogicalName = "account",
                    dpe_TargetAttributeLogicalName = "name",
                    dpe_Sequence = 1,
                    statecode = dpe_policyrule_statecode.Active
                },
                new dpe_PolicyRule
                {
                    Id = Guid.NewGuid(),
                    dpe_TargetEntityLogicalName = "account",
                    dpe_TargetAttributeLogicalName = "name",
                    dpe_Sequence = 2,
                    statecode = dpe_policyrule_statecode.Inactive
                },
                new dpe_PolicyRule
                {
                    Id = Guid.NewGuid(),
                    dpe_TargetEntityLogicalName = "account",
                    dpe_TargetAttributeLogicalName = "name",
                    dpe_Sequence = 3,
                    statecode = dpe_policyrule_statecode.Active
                },
                new dpe_PolicyRule
                {
                    Id = Guid.NewGuid(),
                    dpe_TargetEntityLogicalName = "account",
                    dpe_TargetAttributeLogicalName = "another",
                    dpe_Sequence = 1,
                    statecode = dpe_policyrule_statecode.Active
                },
                new dpe_PolicyRule
                {
                    Id = Guid.NewGuid(),
                    dpe_TargetEntityLogicalName = "activity",
                    dpe_TargetAttributeLogicalName = "name",
                    dpe_Sequence = 1,
                    statecode = dpe_policyrule_statecode.Active
                }
            };

            _conditions = new List<dpe_PolicyCondition>
            {
                new dpe_PolicyCondition
                {
                    Id = Guid.NewGuid(),
                    dpe_PolicyRuleId = _rules[0].ToEntityReference(),
                    dpe_Sequence = 2,
                    statecode = dpe_policycondition_statecode.Active
                },
                new dpe_PolicyCondition
                {
                    Id = Guid.NewGuid(),
                    dpe_PolicyRuleId = _rules[0].ToEntityReference(),
                    dpe_Sequence = 1,
                    statecode = dpe_policycondition_statecode.Active
                },
                new dpe_PolicyCondition
                {
                    Id = Guid.NewGuid(),
                    dpe_PolicyRuleId = _rules[1].ToEntityReference(),
                    dpe_Sequence = 1,
                    statecode = dpe_policycondition_statecode.Active
                }
            };

            foreach (var rule in _rules)
                _context.AddEntity(rule);
            foreach (var condition in _conditions)
                _context.AddEntity(condition);

            _testService = _context.GetOrganizationService();
            _collection = new PolicyCollection();
        }
    }

    [TestClass]
    public class PolicyCollection_GetRules_Tests : PolicyCollectionTestsBase
    {
        [DataTestMethod]
        [DataRow(0, "service")]
        [DataRow(1, "entityLogicalName")]
        [DataRow(2, "attributeLogicalName")]
        public void NullParameters_ThrowsArgumentNullException_ForEachParameter(
            int nullIndex,
            string expectedParamName
        )
        {
            // Arrange
            string entity = "account";
            string attribute = "name";
            IOrganizationService service = _testService;

            if (nullIndex == 0)
                service = null;
            else if (nullIndex == 1)
                entity = null;
            else if (nullIndex == 2)
                attribute = null;

            var error = Assert.ThrowsException<ArgumentNullException>(() =>
            {
                _collection.GetRules(service, entity, attribute);
            });

            Assert.AreEqual(expectedParamName, error.ParamName);
        }

        [TestMethod]
        public void FilterByEntityAndAttribute_ReturnsMatchingRules()
        {
            var results = _collection.GetRules(_testService, "account", "name");

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(_rules[0].Id, results[0].Id);
        }

        [TestMethod]
        public void FilterByEntityAndAttributes_ReturnOnlyActiveRules()
        {
            var results = _collection.GetRules(_testService, "account", "name");

            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.TrueForAll(r => r.statecode == dpe_policyrule_statecode.Active));
        }

        [TestMethod]
        public void FilterByEntityAndAttribute_ReturnsRulesInSequenceOrder()
        {
            var results = _collection.GetRules(_testService, "account", "name");

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(1, results[0].dpe_Sequence);
            Assert.AreEqual(3, results[1].dpe_Sequence);
        }
    }

    [TestClass]
    public class PolicyCollection_GetConditions_Tests : PolicyCollectionTestsBase
    {
        [TestMethod]
        public void NullService_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => _collection.GetConditions(null, _rules[0].Id)
            );
        }

        [TestMethod]
        public void NullRuleId_ThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(
                () => _collection.GetConditions(_testService, Guid.Empty)
            );
        }

        [TestMethod]
        public void ForRule_ReturnsConditionsInSequenceOrder()
        {
            var results = _collection.GetConditions(_testService, _rules[0].Id);

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(1, results[0].dpe_Sequence);
            Assert.AreEqual(2, results[1].dpe_Sequence);
        }

        [TestMethod]
        public void ForRule_ReturnsOnlyActiveConditions()
        {
            _conditions.Add(
                new dpe_PolicyCondition
                {
                    Id = Guid.NewGuid(),
                    dpe_PolicyRuleId = _rules[0].ToEntityReference(),
                    dpe_Sequence = 3,
                    statecode = dpe_policycondition_statecode.Inactive
                }
            );

            var results = _collection.GetConditions(_testService, _rules[0].Id);

            Assert.AreEqual(2, results.Count);
            Assert.AreEqual(1, results[0].dpe_Sequence);
            Assert.AreEqual(2, results[1].dpe_Sequence);
        }
    }
}
