using DataversePolicyEnforcement.Core.Data;
using DataversePolicyEnforcement.Core.Evaluation;
using DataversePolicyEnforcement.Core.Model;
using DataversePolicyEnforcement.Models.Entities;
using FakeXrmEasy.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace DataversePolicyEnforcement.Tests.Core.Evaluation
{
    public abstract class PolicyEvaluatorTestsBase : FakeXrmEasyTestBase
    {
        protected IOrganizationService _testService;
        protected IPolicyCollection _policyCollection = new PolicyCollection();
        protected List<dpe_PolicyRule> _rules;
        protected Entity _target;
        protected Entity _preImage;
        protected dpe_PolicyRule _requiredRule;
        protected dpe_PolicyRule _notAllowedRule;
        protected dpe_PolicyRule _notVisibleRule;
        protected dpe_PolicyCondition _metCondition;
        protected dpe_PolicyCondition _notMetCondition;
        protected PolicyEvaluator _evaluator;
        protected Helpers _helpers;
        protected PolicyDecision _decision = new PolicyDecision();

        public PolicyEvaluatorTestsBase()
        {
            _target = new Entity("account")
            {
                Id = Guid.NewGuid(),
                ["name"] = "Test Account",
                ["trigger"] = "value"
            };

            _preImage = _target.Clone();

            _requiredRule = new dpe_PolicyRule
            {
                Id = Guid.NewGuid(),
                dpe_TargetEntityLogicalName = "account",
                dpe_TargetAttributeLogicalName = "name",
                dpe_TriggerAttributeLogicalName = "trigger",
                dpe_PolicyType = Models.OptionSets.dpe_policytype.Required,
                dpe_Sequence = 1,
                dpe_Result = true,
                statecode = dpe_policyrule_statecode.Active
            };
            _context.AddEntity(_requiredRule);

            _notAllowedRule = new dpe_PolicyRule
            {
                Id = Guid.NewGuid(),
                dpe_TargetEntityLogicalName = "account",
                dpe_TargetAttributeLogicalName = "name",
                dpe_TriggerAttributeLogicalName = "trigger",
                dpe_PolicyType = Models.OptionSets.dpe_policytype.NotAllowed,
                dpe_Sequence = 3,
                dpe_Result = true,
                statecode = dpe_policyrule_statecode.Active
            };
            _context.AddEntity(_notAllowedRule);

            _notVisibleRule = new dpe_PolicyRule
            {
                Id = Guid.NewGuid(),
                dpe_TargetEntityLogicalName = "account",
                dpe_TargetAttributeLogicalName = "name",
                dpe_TriggerAttributeLogicalName = "trigger",
                dpe_PolicyType = Models.OptionSets.dpe_policytype.Visible,
                dpe_Scope = Models.OptionSets.dpe_policyscope.FormOnly,
                dpe_Sequence = 1,
                dpe_Result = false,
                statecode = dpe_policyrule_statecode.Active
            };
            _context.AddEntity(_notVisibleRule);

            _rules = new List<dpe_PolicyRule> { _requiredRule, _notAllowedRule, _notVisibleRule };

            _metCondition = new dpe_PolicyCondition
            {
                Id = Guid.NewGuid(),
                dpe_Operator = Models.OptionSets.dpe_policyconditionoperator.Equals,
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.String,
                dpe_ValueString = _target["trigger"].ToString(),
                statecode = dpe_policycondition_statecode.Active
            };
            _notMetCondition = new dpe_PolicyCondition
            {
                Id = Guid.NewGuid(),
                dpe_Operator = Models.OptionSets.dpe_policyconditionoperator.Equals,
                dpe_ValueType = Models.OptionSets.dpe_policyconditionvaluetype.String,
                dpe_ValueString = "not_value",
                statecode = dpe_policycondition_statecode.Active
            };

            _testService = _context.GetOrganizationService();

            _evaluator = new PolicyEvaluator(_policyCollection);

            _helpers = new Helpers(_context, _metCondition, _notMetCondition);
        }
    }

    [TestClass]
    public class EvaluateAttributeTests : PolicyEvaluatorTestsBase
    {
        [TestMethod]
        [TestCategory("General")]
        public void NullArguments_ReturnsDefaultDecision()
        {
            var nullService = _evaluator.EvaluateAttribute(null, "", "", _target, _preImage);
            Assert.IsFalse(nullService.ServerDetails.Required);
            Assert.IsFalse(nullService.ServerDetails.NotAllowed);
            Assert.IsFalse(nullService.ClientDetails.Required);
            Assert.IsFalse(nullService.ClientDetails.NotAllowed);
            Assert.IsTrue(nullService.ClientDetails.Visible);

            var nullTarget = _evaluator.EvaluateAttribute(_testService, "", "", null, _preImage);
            Assert.IsFalse(nullTarget.ServerDetails.Required);
            Assert.IsFalse(nullTarget.ServerDetails.NotAllowed);
            Assert.IsFalse(nullTarget.ClientDetails.Required);
            Assert.IsFalse(nullTarget.ClientDetails.NotAllowed);
            Assert.IsTrue(nullTarget.ClientDetails.Visible);
        }

        [TestMethod]
        [TestCategory("General")]
        public void NoRules_ReturnsDefaultDecision()
        {
            var decision = _evaluator.EvaluateAttribute(
                _testService,
                "not_account",
                "name",
                _target,
                _preImage
            );
            Assert.IsFalse(decision.ServerDetails.Required);
            Assert.IsFalse(decision.ServerDetails.NotAllowed);
            Assert.IsFalse(decision.ClientDetails.Required);
            Assert.IsFalse(decision.ClientDetails.NotAllowed);
            Assert.IsTrue(decision.ClientDetails.Visible);
        }

        [TestMethod]
        [TestCategory("Server Scope")]
        public void AllServerScopedRules_ReturnDefaultClientDecision()
        {
            _requiredRule.dpe_Scope = Models.OptionSets.dpe_policyscope.ServerOnly;
            _notAllowedRule.dpe_Scope = Models.OptionSets.dpe_policyscope.ServerOnly;
            _notVisibleRule.dpe_Scope = Models.OptionSets.dpe_policyscope.ServerOnly;
            _helpers.AddConditionToRule(_requiredRule);
            _helpers.AddConditionToRule(_notAllowedRule);
            _helpers.AddConditionToRule(_notVisibleRule);
            var decision = _evaluator.EvaluateAttribute(
                _testService,
                "account",
                "name",
                _target,
                _preImage
            );
            Assert.IsTrue(decision.ServerDetails.Required);
            Assert.IsTrue(decision.ServerDetails.NotAllowed);
            Assert.IsFalse(decision.ClientDetails.Required);
            Assert.IsFalse(decision.ClientDetails.NotAllowed);
            Assert.IsTrue(decision.ClientDetails.Visible);
        }

        [TestMethod]
        [TestCategory("Client Scope")]
        public void AllClientScopedRules_ReturnDefaultServerDecision()
        {
            _requiredRule.dpe_Scope = Models.OptionSets.dpe_policyscope.FormOnly;
            _notAllowedRule.dpe_Scope = Models.OptionSets.dpe_policyscope.FormOnly;
            _notVisibleRule.dpe_Scope = Models.OptionSets.dpe_policyscope.FormOnly;
            _helpers.AddConditionToRule(_requiredRule);
            _helpers.AddConditionToRule(_notAllowedRule);
            _helpers.AddConditionToRule(_notVisibleRule);
            var decision = _evaluator.EvaluateAttribute(
                _testService,
                "account",
                "name",
                _target,
                _preImage
            );
            Assert.IsFalse(decision.ServerDetails.Required);
            Assert.IsFalse(decision.ServerDetails.NotAllowed);
            Assert.IsTrue(decision.ClientDetails.Required);
            Assert.IsTrue(decision.ClientDetails.NotAllowed);
            Assert.IsFalse(decision.ClientDetails.Visible);
        }

        [TestMethod]
        [TestCategory("Both Scope")]
        public void BothScopedRules_ReturnBothDecisions()
        {
            _requiredRule.dpe_Scope = Models.OptionSets.dpe_policyscope.Both;
            _notAllowedRule.dpe_Scope = Models.OptionSets.dpe_policyscope.Both;
            _notVisibleRule.dpe_Scope = Models.OptionSets.dpe_policyscope.Both;
            _helpers.AddConditionToRule(_requiredRule);
            _helpers.AddConditionToRule(_notAllowedRule);
            _helpers.AddConditionToRule(_notVisibleRule);
            var decision = _evaluator.EvaluateAttribute(
                _testService,
                "account",
                "name",
                _target,
                _preImage
            );
            Assert.IsTrue(decision.ServerDetails.Required);
            Assert.IsTrue(decision.ServerDetails.NotAllowed);
            Assert.IsTrue(decision.ClientDetails.Required);
            Assert.IsTrue(decision.ClientDetails.NotAllowed);
            Assert.IsFalse(decision.ClientDetails.Visible);
        }

        [TestMethod]
        [TestCategory("General")]
        public void InactiveRules_ReturnDefaultDecision()
        {
            _requiredRule.statecode = dpe_policyrule_statecode.Inactive;
            _notAllowedRule.statecode = dpe_policyrule_statecode.Inactive;
            _notVisibleRule.statecode = dpe_policyrule_statecode.Inactive;
            _helpers.AddConditionToRule(_requiredRule);
            _helpers.AddConditionToRule(_notAllowedRule);
            _helpers.AddConditionToRule(_notVisibleRule);
            var decision = _evaluator.EvaluateAttribute(
                _testService,
                "account",
                "name",
                _target,
                _preImage
            );
            Assert.IsFalse(decision.ServerDetails.Required);
            Assert.IsFalse(decision.ServerDetails.NotAllowed);
            Assert.IsFalse(decision.ClientDetails.Required);
            Assert.IsFalse(decision.ClientDetails.NotAllowed);
            Assert.IsTrue(decision.ClientDetails.Visible);
        }

        [TestMethod]
        [TestCategory("General")]
        public void RulesWithNullScopes_ReturnDefaultDecision()
        {
            _requiredRule.dpe_Scope = null;
            _notAllowedRule.dpe_Scope = null;
            _notVisibleRule.dpe_Scope = null;
            _helpers.AddConditionToRule(_requiredRule);
            _helpers.AddConditionToRule(_notAllowedRule);
            _helpers.AddConditionToRule(_notVisibleRule);
            var decision = _evaluator.EvaluateAttribute(
                _testService,
                "account",
                "name",
                _target,
                _preImage
            );
            Assert.IsFalse(decision.ServerDetails.Required);
            Assert.IsFalse(decision.ServerDetails.NotAllowed);
            Assert.IsFalse(decision.ClientDetails.Required);
            Assert.IsFalse(decision.ClientDetails.NotAllowed);
            Assert.IsTrue(decision.ClientDetails.Visible);
        }
    }
}
