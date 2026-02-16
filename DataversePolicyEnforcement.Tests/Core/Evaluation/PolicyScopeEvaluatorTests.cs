using DataversePolicyEnforcement.Core.Data;
using DataversePolicyEnforcement.Core.Evaluation;
using DataversePolicyEnforcement.Core.Model;
using DataversePolicyEnforcement.Models.Entities;
using DataversePolicyEnforcement.Tests.Helpers;
using FakeXrmEasy.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DataversePolicyEnforcement.Tests.Core.Evaluation
{
    public abstract class PolicyScopeEvaluatorTestsBase : FakeXrmEasyTestBase
    {
        protected IOrganizationService _testService;
        protected IPolicyCollection _policyCollection = new PolicyCollection();
        protected List<dpe_PolicyRule> _rules;
        protected Entity _target;
        protected Entity _preImage;
        protected dpe_PolicyRule _requiredRule;
        protected dpe_PolicyRule _notAllowedRule;
        protected dpe_PolicyCondition _metCondition;
        protected dpe_PolicyCondition _notMetCondition;
        protected ConditionHelpers _helpers;

        public PolicyScopeEvaluatorTestsBase()
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

            _rules = new List<dpe_PolicyRule> { _requiredRule, _notAllowedRule };

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

            _helpers = new ConditionHelpers(_context, _metCondition, _notMetCondition);
        }
    }

    [TestClass]
    public class EvaluateClientScopeTests : PolicyScopeEvaluatorTestsBase
    {
        protected ClientPolicyDetails _details = new ClientPolicyDetails();
        protected PolicyScopeEvaluation _evaluator;
        protected List<dpe_PolicyRule> _clientRules;
        protected dpe_PolicyRule _notVisibleRule;

        public EvaluateClientScopeTests()
        {
            _evaluator = new PolicyScopeEvaluation(_policyCollection);

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

            _clientRules = new List<dpe_PolicyRule> { _notVisibleRule };
            _clientRules.AddRange(_rules);
        }

        [TestMethod]
        public void NullParameters_ReturnsBaseClientDetails_ForEachParameter()
        {
            Debug.WriteLine("Testing null service...");
            var nullServiceResult = _evaluator.EvaluateClientScope(
                null,
                _clientRules,
                _target,
                _preImage,
                _details
            );
            Assert.AreSame(_details, nullServiceResult);

            Debug.WriteLine("Testing null rules...");
            var nullRulesResult = _evaluator.EvaluateClientScope(
                _testService,
                null,
                _target,
                _preImage,
                _details
            );
            Assert.AreSame(_details, nullRulesResult);

            Debug.WriteLine("Testing empty rules...");
            var emptyRulesResult = _evaluator.EvaluateClientScope(
                _testService,
                new List<dpe_PolicyRule>(),
                _target,
                _preImage,
                _details
            );
            Assert.AreSame(_details, emptyRulesResult);

            Debug.WriteLine("Testing null target...");
            var nullTargetResult = _evaluator.EvaluateClientScope(
                _testService,
                _clientRules,
                null,
                _preImage,
                _details
            );
            Assert.AreSame(_details, nullTargetResult);

            Debug.WriteLine("Testing null initial...");
            var nullInitialResult = _evaluator.EvaluateClientScope(
                _testService,
                _clientRules,
                _target,
                _preImage,
                null
            );
            Assert.IsNotNull(nullInitialResult);
            Assert.IsInstanceOfType(_details, typeof(ClientPolicyDetails));
        }

        [TestMethod]
        public void RuleConditionsDoNotMatch_SkipsRule()
        {
            var rule = _notVisibleRule.Clone().ToEntity<dpe_PolicyRule>();
            _context.AddEntity(rule);
            var rules = new List<dpe_PolicyRule> { rule };

            _helpers.AddConditionToRule(rule, false);

            var result = _evaluator.EvaluateClientScope(
                _service,
                rules,
                _target,
                _preImage,
                _details
            );

            Assert.IsTrue(result.Visible);
        }

        [TestMethod]
        public void FirstVisibleMatch_SetsVisibleToRuleResult()
        {
            var rule = _notVisibleRule.Clone().ToEntity<dpe_PolicyRule>();
            rule.dpe_Result = true;
            rule.dpe_Sequence = _notVisibleRule.dpe_Sequence + 1;
            _context.AddEntity(rule);

            _helpers.AddConditionToRule(rule);
            _helpers.AddConditionToRule(_notVisibleRule);

            var rules = new List<dpe_PolicyRule> { rule, _notVisibleRule };

            var result = _evaluator.EvaluateClientScope(
                _service,
                rules,
                _target,
                _preImage,
                _details
            );

            Assert.IsFalse(result.Visible);
        }

        // TODO: Include other two cases, Required and NotAllowed
        [TestMethod]
        public void VisibleRuleResultNull_PreservesInitialVisible()
        {
            _notVisibleRule.dpe_Result = null;

            _helpers.AddConditionToRule(_notVisibleRule);

            var rules = new List<dpe_PolicyRule> { _notVisibleRule };

            var result = _evaluator.EvaluateClientScope(
                _service,
                rules,
                _target,
                _preImage,
                _details
            );

            Assert.IsTrue(result.Visible);
        }

        [TestMethod]
        public void FirstRequiredMatch_SetsRequiredToRuleResult()
        {
            var rule = _requiredRule.Clone().ToEntity<dpe_PolicyRule>();
            rule.dpe_Result = false;
            rule.dpe_Sequence = _requiredRule.dpe_Sequence + 1;
            _context.AddEntity(rule);

            _helpers.AddConditionToRule(rule);
            _helpers.AddConditionToRule(_requiredRule);

            var rules = new List<dpe_PolicyRule> { rule, _requiredRule };

            var result = _evaluator.EvaluateClientScope(
                _service,
                rules,
                _target,
                _preImage,
                _details
            );

            Assert.IsTrue(result.Required);
        }

        [TestMethod]
        public void NotAllowedTrue_ImmediatelySetsNotAllowedAndStopsEvaluation()
        {
            _helpers.AddConditionToRule(_notAllowedRule);

            var additionalRule = _notAllowedRule.Clone().ToEntity<dpe_PolicyRule>();
            additionalRule.dpe_Result = false;
            additionalRule.dpe_Sequence = _notAllowedRule.dpe_Sequence + 1;
            _context.AddEntity(additionalRule);

            _helpers.AddConditionToRule(additionalRule);

            var rules = new List<dpe_PolicyRule> { additionalRule, _notAllowedRule };

            var result = _evaluator.EvaluateClientScope(
                _service,
                rules,
                _target,
                _preImage,
                _details
            );

            Assert.IsTrue(result.NotAllowed);
        }

        [TestMethod]
        public void NotAllowedFalse_DoesNotPreventALaterTrueFromOverriding()
        {
            var additionalRule = _notAllowedRule.Clone().ToEntity<dpe_PolicyRule>();
            additionalRule.dpe_Result = false;
            additionalRule.dpe_Sequence = 1;
            _context.AddEntity(additionalRule);

            _helpers.AddConditionToRule(additionalRule);

            _notAllowedRule.dpe_Sequence = additionalRule.dpe_Sequence + 1;
            _helpers.AddConditionToRule(_notAllowedRule);

            var rules = new List<dpe_PolicyRule> { additionalRule, _notAllowedRule };

            var result = _evaluator.EvaluateClientScope(
                _service,
                rules,
                _target,
                _preImage,
                _details
            );

            Assert.IsTrue(result.NotAllowed);
        }
    }

    [TestClass]
    public class EvaluateServerScopeTests : PolicyScopeEvaluatorTestsBase
    {
        protected ServerPolicyDetails _details = new ServerPolicyDetails();
        protected PolicyScopeEvaluation _evaluator;
        protected List<dpe_PolicyRule> _serverRules;

        public EvaluateServerScopeTests()
        {
            _evaluator = new PolicyScopeEvaluation(_policyCollection);

            _serverRules = new List<dpe_PolicyRule>();
            _serverRules.AddRange(_rules);
        }

        [TestMethod]
        public void NullParameters_ReturnsBaseServerDetails_ForEachParameter()
        {
            Debug.WriteLine("Testing null service...");
            var nullServiceResult = _evaluator.EvaluateServerScope(
                null,
                _serverRules,
                _target,
                _preImage,
                _details
            );
            Assert.AreSame(_details, nullServiceResult);

            Debug.WriteLine("Testing null rules...");
            var nullRulesResult = _evaluator.EvaluateServerScope(
                _testService,
                null,
                _target,
                _preImage,
                _details
            );
            Assert.AreSame(_details, nullRulesResult);

            Debug.WriteLine("Testing empty rules...");
            var emptyRulesResult = _evaluator.EvaluateServerScope(
                _testService,
                new List<dpe_PolicyRule>(),
                _target,
                _preImage,
                _details
            );
            Assert.AreSame(_details, emptyRulesResult);

            Debug.WriteLine("Testing null target...");
            var nullTargetResult = _evaluator.EvaluateServerScope(
                _testService,
                _serverRules,
                null,
                _preImage,
                _details
            );
            Assert.AreSame(_details, nullTargetResult);

            Debug.WriteLine("Testing null initial...");
            var nullInitialResult = _evaluator.EvaluateServerScope(
                _testService,
                _serverRules,
                _target,
                _preImage,
                null
            );
            Assert.IsNotNull(nullInitialResult);
            Assert.IsInstanceOfType(_details, typeof(ServerPolicyDetails));
        }

        [TestMethod]
        public void RuleConditionsDoNotMatch_SkipsRule()
        {
            var rule = _requiredRule.Clone().ToEntity<dpe_PolicyRule>();
            _context.AddEntity(rule);
            var rules = new List<dpe_PolicyRule> { rule };

            _helpers.AddConditionToRule(rule, false);

            var result = _evaluator.EvaluateServerScope(
                _service,
                rules,
                _target,
                _preImage,
                _details
            );

            Assert.IsFalse(result.Required);
        }

        // TODO: Include other two cases, Required and NotAllowed
        [TestMethod]
        public void VisibleRuleResultNull_PreservesInitialValue()
        {
            _requiredRule.dpe_Result = null;

            _helpers.AddConditionToRule(_requiredRule);

            var rules = new List<dpe_PolicyRule> { _requiredRule };

            var result = _evaluator.EvaluateServerScope(
                _service,
                rules,
                _target,
                _preImage,
                _details
            );

            Assert.IsFalse(result.Required);
        }

        [TestMethod]
        public void FirstRequiredMatch_SetsRequiredToRuleResult()
        {
            var rule = _requiredRule.Clone().ToEntity<dpe_PolicyRule>();
            rule.dpe_Result = false;
            rule.dpe_Sequence = _requiredRule.dpe_Sequence + 1;
            _context.AddEntity(rule);

            _helpers.AddConditionToRule(rule);
            _helpers.AddConditionToRule(_requiredRule);

            var rules = new List<dpe_PolicyRule> { rule, _requiredRule };

            var result = _evaluator.EvaluateServerScope(
                _service,
                rules,
                _target,
                _preImage,
                _details
            );

            Assert.IsTrue(result.Required);
        }

        [TestMethod]
        public void NotAllowedTrue_ImmediatelySetsNotAllowedAndStopsEvaluation()
        {
            _helpers.AddConditionToRule(_notAllowedRule);

            var additionalRule = _notAllowedRule.Clone().ToEntity<dpe_PolicyRule>();
            additionalRule.dpe_Result = false;
            additionalRule.dpe_Sequence = _notAllowedRule.dpe_Sequence + 1;
            _context.AddEntity(additionalRule);

            _helpers.AddConditionToRule(additionalRule);

            var rules = new List<dpe_PolicyRule> { additionalRule, _notAllowedRule };

            var result = _evaluator.EvaluateServerScope(
                _service,
                rules,
                _target,
                _preImage,
                _details
            );

            Assert.IsTrue(result.NotAllowed);
        }

        [TestMethod]
        public void NotAllowedFalse_DoesNotPreventALaterTrueFromOverriding()
        {
            var additionalRule = _notAllowedRule.Clone().ToEntity<dpe_PolicyRule>();
            additionalRule.dpe_Result = false;
            additionalRule.dpe_Sequence = 1;
            _context.AddEntity(additionalRule);

            _helpers.AddConditionToRule(additionalRule);

            _notAllowedRule.dpe_Sequence = additionalRule.dpe_Sequence + 1;
            _helpers.AddConditionToRule(_notAllowedRule);

            var rules = new List<dpe_PolicyRule> { additionalRule, _notAllowedRule };

            var result = _evaluator.EvaluateServerScope(
                _service,
                rules,
                _target,
                _preImage,
                _details
            );

            Assert.IsTrue(result.NotAllowed);
        }
    }
}
