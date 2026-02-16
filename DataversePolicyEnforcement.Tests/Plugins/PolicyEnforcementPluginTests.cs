using DataversePolicyEnforcement.Core.Data;
using DataversePolicyEnforcement.Core.Evaluation;
using DataversePolicyEnforcement.Core.Model;
using DataversePolicyEnforcement.Models.Entities;
using DataversePolicyEnforcement.Plugins;
using DataversePolicyEnforcement.Tests.Helpers;
using FakeXrmEasy.Extensions;
using FakeXrmEasy.Plugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace DataversePolicyEnforcement.Tests.Plugins
{
    [TestClass]
    public class PolicyEnforcementPluginTests : FakeXrmEasyTestBase
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
        protected PolicyEvaluator _evaluator;
        protected PolicyDecision _decision = new PolicyDecision();
        protected ConditionHelpers _conditionHelpers;
        protected XrmFakedPluginExecutionContext _pluginContext;

        public PolicyEnforcementPluginTests()
        {
            _pluginContext = _context.GetDefaultPluginContext();
            _pluginContext.MessageName = "Update";
            _pluginContext.Stage = 20; // PreOperation

            _target = new Entity("account")
            {
                Id = Guid.NewGuid(),
                ["name"] = "Test Account",
                ["trigger"] = "value"
            };
            _pluginContext.InputParameters["Target"] = _target;

            _preImage = _target.Clone();
            _pluginContext.PreEntityImages.Add("PreImage", _preImage);

            _requiredRule = new dpe_PolicyRule
            {
                Id = Guid.NewGuid(),
                dpe_TargetEntityLogicalName = "account",
                dpe_TargetAttributeLogicalName = "name",
                dpe_TriggerAttributeLogicalName = "trigger",
                dpe_PolicyType = Models.OptionSets.dpe_policytype.Required,
                dpe_Scope = Models.OptionSets.dpe_policyscope.Both,
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
                dpe_Scope = Models.OptionSets.dpe_policyscope.Both,
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

            _evaluator = new PolicyEvaluator(_policyCollection);

            _conditionHelpers = new ConditionHelpers(_context, _metCondition, _notMetCondition);
        }

        [TestMethod]
        [TestCategory("PluginExecutionContext")]
        public void ExecuteCdsPlugin_UnsupportedMessage_ExitsWithoutAction()
        {
            _pluginContext.MessageName = "Delete"; // Unsupported message

            _context.ExecutePluginWith<PolicyEnforcementPlugin>(_pluginContext);

            // If we reach this point without exceptions, the test passes
        }

        [TestMethod]
        [TestCategory("PluginExecutionContext")]
        public void ExecuteCdsPlugin_UnsupportedStage_ExitsWithoutAction()
        {
            _pluginContext.Stage = 40; // Unsupported stage

            _context.ExecutePluginWith<PolicyEnforcementPlugin>(_pluginContext);

            // If we reach this point without exceptions, the test passes
        }

        [TestMethod]
        [TestCategory("PluginResources")]
        public void ExecuteCdsPlugin_MissingTarget_ThrowsException()
        {
            _pluginContext.InputParameters.Remove("Target"); // Remove required Target parameter

            var error = Assert.ThrowsException<InvalidPluginExecutionException>(
                () => _context.ExecutePluginWith<PolicyEnforcementPlugin>(_pluginContext)
            );

            Assert.AreEqual("Target entity is missing in the input parameters.", error.Message);
        }

        [TestMethod]
        [TestCategory("PluginResources")]
        public void ExecuteCdsPlugin_UpdateMissingPreImage_ThrowsException()
        {
            _pluginContext.PreEntityImages.Clear(); // Remove required Pre-Image
            var error = Assert.ThrowsException<InvalidPluginExecutionException>(
                () => _context.ExecutePluginWith<PolicyEnforcementPlugin>(_pluginContext)
            );

            Assert.AreEqual("Pre-Image named PreImage required on update", error.Message);
        }

        [TestMethod]
        [TestCategory("PolicyEvaluation")]
        [DataRow("Update")]
        [DataRow("Create")]
        public void ExecuteCdsPlugin_PolicyEvaluation_DecisionMade(string messageName)
        {
            _pluginContext.MessageName = messageName;
            _conditionHelpers.AddConditionToRule(_requiredRule, met: true);
            // Needs to be allowed on create
            var notAllowedMet = messageName == "Update";
            _conditionHelpers.AddConditionToRule(_notAllowedRule, met: notAllowedMet, sequence: 2);

            _context.ExecutePluginWith<PolicyEnforcementPlugin>(_pluginContext);
            // If we reach this point without exceptions, the test passes
        }

        [TestMethod]
        [TestCategory("PolicyEvaluation")]
        [DataRow("Update")]
        [DataRow("Create")]
        public void ExecuteCdsPlugin_PolicyEvaluation_NotAllowedRuleEnforced(string messageName)
        {
            _pluginContext.MessageName = messageName;
            _conditionHelpers.AddConditionToRule(_notAllowedRule, met: true);

            _preImage["name"] = "Changed Name"; // Change the name to trigger the NotAllowed rule

            var error = Assert.ThrowsException<InvalidPluginExecutionException>(
                () => _context.ExecutePluginWith<PolicyEnforcementPlugin>(_pluginContext)
            );

            var expectedError =
                messageName == "Update"
                    ? $"Change blocked by policy: {_target.LogicalName}.name is not allowed to change."
                    : $"Creation blocked by policy: {_target.LogicalName}.name is not allowed to be set.";

            Assert.AreEqual(expectedError, error.Message);
        }

        [TestMethod]
        [TestCategory("PolicyEvaluation")]
        [DataRow("Update")]
        [DataRow("Create")]
        public void ExecuteCdsPlugin_PolicyEvaluation_RequiredRuleEnforced(string messageName)
        {
            _pluginContext.MessageName = messageName;
            _conditionHelpers.AddConditionToRule(_requiredRule, met: true);
            _conditionHelpers.AddConditionToRule(_notAllowedRule, met: false, sequence: 2); // Ensure NotAllowed rule does not interfere
            _target["name"] = null; // Change the name to meet the required rule condition

            var error = Assert.ThrowsException<InvalidPluginExecutionException>(
                () => _context.ExecutePluginWith<PolicyEnforcementPlugin>(_pluginContext)
            );

            var expectedError = $"Policy violation: {_target.LogicalName}.name is required.";

            Assert.AreEqual(expectedError, error.Message);
        }

        [TestMethod]
        [TestCategory("PolicyEvaluation")]
        [DataRow("Update")]
        [DataRow("Create")]
        public void ExecuteCdsPlugin_PolicyEvaluation_RequiredRuleNotMet(string messageName)
        {
            _pluginContext.MessageName = messageName;
            _conditionHelpers.AddConditionToRule(_requiredRule, met: false);
            _conditionHelpers.AddConditionToRule(_notAllowedRule, met: false, sequence: 2); // Ensure NotAllowed rule does not interfere
            _context.ExecutePluginWith<PolicyEnforcementPlugin>(_pluginContext);
            // If we reach this point without exceptions, the test passes
        }

        [TestMethod]
        [TestCategory("PolicyEvaluation")]
        [DataRow("Update")]
        [DataRow("Create")]
        public void ExecuteCdsPlugin_PolicyEvaluation_NotAllowedRuleNotMet(string messageName)
        {
            _pluginContext.MessageName = messageName;
            _conditionHelpers.AddConditionToRule(_notAllowedRule, met: false);

            _context.ExecutePluginWith<PolicyEnforcementPlugin>(_pluginContext);
            // If we reach this point without exceptions, the test passes
        }
    }
}
