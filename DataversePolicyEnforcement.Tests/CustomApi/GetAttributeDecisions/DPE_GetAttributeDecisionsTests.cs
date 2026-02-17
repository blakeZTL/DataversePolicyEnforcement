using DataversePolicyEnforcement.CustomApi.Models;
using DataversePolicyEnforcement.Models;
using DataversePolicyEnforcement.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.Json;

namespace DataversePolicyEnforcement.Tests.CustomApi.GetAttributeDecisions
{
    [TestClass]
    public class DPE_GetAttributeDecisionsTests : FakeXrmEasyTestBase
    {
        dpe_PolicyRule _rule;
        dpe_GetAttributeDecisionsRequest _request;
        ConditionHelpers _conditionHelpers;
        dpe_PolicyCondition _metCondition;
        dpe_PolicyCondition _notMetCondition;

        public DPE_GetAttributeDecisionsTests()
        {
            _rule = new dpe_PolicyRule
            {
                Id = Guid.NewGuid(),
                dpe_TargetEntityLogicalName = "account",
                dpe_TargetAttributeLogicalName = "name",
                dpe_TriggerAttributeLogicalName = "trigger",
                dpe_Sequence = 1,
                dpe_Scope = dpe_policyscope.Both,
                dpe_PolicyType = dpe_policytype.Required,
                dpe_Result = true,
                statecode = dpe_policyrule_statecode.Active
            };
            _context.AddEntity(_rule);

            _request = new dpe_GetAttributeDecisionsRequest
            {
                dpe_gad_entitylogicalname = "account",
                dpe_gad_triggerattributelogicalname = "trigger",
                dpe_gad_triggercurrentvalue_string = "test",
                dpe_gad_targetattributelogicalnames = new string[] { "name" }
            };

            _metCondition = new dpe_PolicyCondition
            {
                Id = Guid.NewGuid(),
                dpe_Operator = dpe_policyconditionoperator.Equals,
                dpe_ValueType = dpe_policyconditionvaluetype.String,
                dpe_ValueString = _request.dpe_gad_triggercurrentvalue_string,
                statecode = dpe_policycondition_statecode.Active
            };
            _notMetCondition = new dpe_PolicyCondition
            {
                Id = Guid.NewGuid(),
                dpe_Operator = dpe_policyconditionoperator.Equals,
                dpe_ValueType = dpe_policyconditionvaluetype.String,
                dpe_ValueString = "not_value",
                statecode = dpe_policycondition_statecode.Active
            };

            _conditionHelpers = new ConditionHelpers(_context, _metCondition, _notMetCondition);
        }

        [TestMethod]
        public void FoundMatchingRule_ReturnsDecisions()
        {
            _conditionHelpers.AddConditionToRule(_rule);

            var response = _service.Execute(_request) as dpe_GetAttributeDecisionsResponse;

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.dpe_gad_attributedecision_results);

            var results = JsonSerializer.Deserialize<AttributeDecisionResult[]>(
                response.dpe_gad_attributedecision_results
            );

            Assert.AreEqual(results.Count(), 1);

            var result = results.First();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.EntityLogicalName, _request.dpe_gad_entitylogicalname);
            Assert.AreEqual(
                result.AttributeLogicalName,
                _request.dpe_gad_targetattributelogicalnames.First()
            );
            Assert.AreEqual(
                result.TriggerAttributeLogicalName,
                _request.dpe_gad_triggerattributelogicalname
            );
            Assert.IsNotNull(result.ClientPolicyDetails);

            var clientDetail = result.ClientPolicyDetails;
            Assert.IsTrue(clientDetail.Visible);
            Assert.IsTrue(clientDetail.Required);
            Assert.IsFalse(clientDetail.NotAllowed);
        }

        [TestMethod]
        public void NoMatchingRule_ReturnsDecisions()
        {
            _conditionHelpers.AddConditionToRule(_rule, met: false);

            var response = _service.Execute(_request) as dpe_GetAttributeDecisionsResponse;

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.dpe_gad_attributedecision_results);

            var results = JsonSerializer.Deserialize<AttributeDecisionResult[]>(
                response.dpe_gad_attributedecision_results
            );

            Assert.AreEqual(results.Count(), 1);

            var result = results.First();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.EntityLogicalName, _request.dpe_gad_entitylogicalname);
            Assert.AreEqual(
                result.AttributeLogicalName,
                _request.dpe_gad_targetattributelogicalnames.First()
            );
            Assert.AreEqual(
                result.TriggerAttributeLogicalName,
                _request.dpe_gad_triggerattributelogicalname
            );
            Assert.IsNotNull(result.ClientPolicyDetails);

            var clientDetail = result.ClientPolicyDetails;
            Assert.IsTrue(clientDetail.Visible);
            Assert.IsFalse(clientDetail.Required);
            Assert.IsFalse(clientDetail.NotAllowed);
        }
    }
}
