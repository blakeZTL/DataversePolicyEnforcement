using DataversePolicyEnforcement.Core.Data;
using DataversePolicyEnforcement.Core.Evaluation;
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
    public class PolicyEvaluatorTests : PolicyEvaluatorTestsBase
    {
        //
    }
}
