using DataversePolicyEnforcement.CustomApi.Models;
using DataversePolicyEnforcement.Models;
using DataversePolicyEnforcement.Tests.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Linq;
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
                dpe_gad_targetattributelogicalnames = new string[] { "name" }
            };

            _metCondition = new dpe_PolicyCondition
            {
                Id = Guid.NewGuid(),
                dpe_Operator = dpe_policyconditionoperator.Equals,
                statecode = dpe_policycondition_statecode.Active
            };
            _notMetCondition = new dpe_PolicyCondition
            {
                Id = Guid.NewGuid(),
                dpe_Operator = dpe_policyconditionoperator.Equals,
                statecode = dpe_policycondition_statecode.Active
            };

            _conditionHelpers = new ConditionHelpers(_context, _metCondition, _notMetCondition);
        }

        [TestMethod]
        [TestCategory("RequestValidation")]
        public void ValidRequest_ReturnsDecisions()
        {
            _request.dpe_gad_triggercurrentvalue_string = "test";

            var response = _service.Execute(_request) as dpe_GetAttributeDecisionsResponse;
            Assert.IsNotNull(response);

            _request.dpe_gad_triggercurrentvalue_string = null;
            _request.dpe_gad_triggercurrentvalue_lookupid = Guid.NewGuid().ToString();
            _request.dpe_gad_triggercurrentvalue_lookup_logicalname = "account";

            response = _service.Execute(_request) as dpe_GetAttributeDecisionsResponse;
            Assert.IsNotNull(response);
        }

        [TestMethod]
        [TestCategory("RequestValidation")]
        public void InvalidRequest_ThrowsError()
        {
            _request.dpe_gad_triggercurrentvalue_string = "test";
            _request.dpe_gad_triggercurrentvalue_int = 1; // multiple trigger values set - invalid

            var error = Assert.ThrowsException<InvalidPluginExecutionException>(
                () => _service.Execute(_request)
            );

            Assert.AreEqual(
                "Invalid request: exactly one trigger current value must be provided.",
                error.Message
            );
        }

        [TestMethod]
        [TestCategory("String")]
        public void String_FoundMatchingRule_ReturnsDecisions()
        {
            _request.dpe_gad_triggercurrentvalue_string = "test";
            _metCondition.dpe_ValueString = _request.dpe_gad_triggercurrentvalue_string;
            _metCondition.dpe_ValueType = dpe_policyconditionvaluetype.String;
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
        [TestCategory("String")]
        public void String_NoMatchingRule_ReturnsDecisions()
        {
            _request.dpe_gad_triggercurrentvalue_string = "test";
            _notMetCondition.dpe_ValueType = dpe_policyconditionvaluetype.String;
            _notMetCondition.dpe_ValueString = "not_value";
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

        [TestMethod]
        [TestCategory("Int")]
        public void Int_FoundMatchingRule_ReturnsDecisions()
        {
            _request.dpe_gad_triggercurrentvalue_int = 1;
            _metCondition.dpe_ValueWholeNumber = _request.dpe_gad_triggercurrentvalue_int;
            _metCondition.dpe_ValueType = dpe_policyconditionvaluetype.WholeNumber;
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
        [TestCategory("Int")]
        public void Int_NoMatchingRule_ReturnsDecisions()
        {
            _request.dpe_gad_triggercurrentvalue_int = 1;
            _notMetCondition.dpe_ValueWholeNumber = 2;
            _notMetCondition.dpe_ValueType = dpe_policyconditionvaluetype.WholeNumber;
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

        [TestMethod]
        [TestCategory("OptionSet")]
        public void OptionSet_FoundMatchingRule_ReturnsDecisions()
        {
            _request.dpe_gad_triggercurrentvalue_optionsetvalue = 1;
            _metCondition.dpe_ValueOptionSetValue =
                _request.dpe_gad_triggercurrentvalue_optionsetvalue;
            _metCondition.dpe_ValueType = dpe_policyconditionvaluetype.OptionSet;
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
        [TestCategory("OptionSet")]
        public void OptionSet_NoMatchingRule_ReturnsDecisions()
        {
            _request.dpe_gad_triggercurrentvalue_optionsetvalue = 1;
            _notMetCondition.dpe_ValueOptionSetValue = 2;
            _notMetCondition.dpe_ValueType = dpe_policyconditionvaluetype.OptionSet;
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

        [TestMethod]
        [TestCategory("Lookup")]
        public void Lookup_FoundMatchingRule_ReturnsDecisions()
        {
            _request.dpe_gad_triggercurrentvalue_lookupid = Guid.NewGuid().ToString();
            _request.dpe_gad_triggercurrentvalue_lookup_logicalname = "account";
            _metCondition.dpe_ValueLookupId = _request.dpe_gad_triggercurrentvalue_lookupid;
            _metCondition.dpe_ValueLookupLogicalName =
                _request.dpe_gad_triggercurrentvalue_lookup_logicalname;
            _metCondition.dpe_ValueType = dpe_policyconditionvaluetype.Lookup;
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
        [TestCategory("Lookup")]
        public void Lookup_NoMatchingRule_ReturnsDecisions()
        {
            _request.dpe_gad_triggercurrentvalue_lookupid = Guid.NewGuid().ToString();
            _request.dpe_gad_triggercurrentvalue_lookup_logicalname = "account";
            _notMetCondition.dpe_ValueLookupId = Guid.NewGuid().ToString();
            _notMetCondition.dpe_ValueLookupLogicalName =
                _request.dpe_gad_triggercurrentvalue_lookup_logicalname;
            _notMetCondition.dpe_ValueType = dpe_policyconditionvaluetype.Lookup;
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
