using DataversePolicyEnforcement.Models.Entities;
using DataversePolicyEnforcement.Plugins.PolicyRule;
using FakeXrmEasy.Extensions;
using FakeXrmEasy.Plugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using System.Collections.Generic;

namespace DataversePolicyEnforcement.Tests.Plugins.PolicyRule
{
    [TestClass]
    public class ValidatePolicyRulePluginTests : FakeXrmEasyTestBase
    {
        readonly XrmFakedPluginExecutionContext _pluginContext;
        readonly dpe_PolicyRule _target;
        readonly dpe_PolicyRule _preImage;

        public ValidatePolicyRulePluginTests()
        {
            _pluginContext = _context.GetDefaultPluginContext();
            _pluginContext.MessageName = "Update";
            _pluginContext.Stage = 10;

            _target = new dpe_PolicyRule
            {
                dpe_TargetEntityLogicalName = "account",
                dpe_TargetAttributeLogicalName = "name",
                dpe_TriggerAttributeLogicalName = "trigger",
                dpe_PolicyType = Models.OptionSets.dpe_policytype.Required,
                dpe_Scope = Models.OptionSets.dpe_policyscope.ServerOnly,
                dpe_Sequence = 1,
                dpe_Result = true
            };

            _pluginContext.InputParameters.Add("Target", _target);

            _preImage = _target.Clone().ToEntity<dpe_PolicyRule>();

            _pluginContext.PreEntityImages.Add("PreImage", _preImage);

            var accountMetadata = new EntityMetadata
            {
                LogicalName = _target.dpe_TargetEntityLogicalName
            };

            var nameAttributeMetadata = new AttributeMetadata
            {
                LogicalName = _target.dpe_TargetAttributeLogicalName
            };
            accountMetadata.SetAttribute(nameAttributeMetadata);

            var triggerAttributeMetadata = new AttributeMetadata
            {
                LogicalName = _target.dpe_TriggerAttributeLogicalName
            };
            accountMetadata.SetAttribute(triggerAttributeMetadata);

            _context.InitializeMetadata(new List<EntityMetadata>() { accountMetadata });
        }

        [TestMethod]
        [TestCategory("PluginExecutionContext")]
        public void InvalidMessage_Executes()
        {
            _pluginContext.MessageName = "Delete";
            _context.ExecutePluginWith<ValidatePolicyRulePlugin>();

            //pass
        }

        [TestMethod]
        [TestCategory("PluginExecutionContext")]
        public void InvalidStage_Executes()
        {
            _pluginContext.Stage = 40;
            _context.ExecutePluginWith<ValidatePolicyRulePlugin>();

            //pass
        }

        [TestMethod]
        [TestCategory("PluginResources")]
        public void TargetNotInInputs_Throws()
        {
            _pluginContext.InputParameters.Clear();

            var error = Assert.ThrowsException<InvalidPluginExecutionException>(
                () => _context.ExecutePluginWith<ValidatePolicyRulePlugin>(_pluginContext)
            );

            Assert.AreEqual(error.Message, "Target not found in input parameters");
        }

        [TestMethod]
        [TestCategory("PluginResources")]
        [DataRow("Create")]
        [DataRow("Update")]
        public void PreImageMissingOnUpdate_Throws(string messageName)
        {
            _pluginContext.PreEntityImages.Clear();
            _pluginContext.MessageName = messageName;
            if (messageName == "Update")
            {
                var error = Assert.ThrowsException<InvalidPluginExecutionException>(
                    () => _context.ExecutePluginWith<ValidatePolicyRulePlugin>(_pluginContext)
                );

                Assert.AreEqual(error.Message, "Pre-image named PreImage required on update");
            }
            else
            {
                _context.ExecutePluginWith<ValidatePolicyRulePlugin>(_pluginContext);

                //pass
            }
        }

        [TestMethod]
        [DataRow("Create")]
        [DataRow("Update")]
        public void ValidRecord_Executes(string messageName)
        {
            _pluginContext.MessageName = messageName;
            _context.ExecutePluginWith<ValidatePolicyRulePlugin>(_pluginContext);

            // pass
        }

        [TestMethod]
        [TestCategory("RequiredColumnValidation")]
        [DataRow("Create")]
        [DataRow("Update")]
        public void MissingRequiredAttribute_Throws(string messageName)
        {
            _pluginContext.MessageName = messageName;

            foreach (var attr in ValidatePolicyRulePlugin.RequiredAttributes)
            {
                var target = _target.Clone().ToEntity<dpe_PolicyRule>();
                var preImage = _preImage.Clone().ToEntity<dpe_PolicyRule>();

                target[attr] = null;
                preImage[attr] = null;

                _pluginContext.InputParameters["Target"] = target;
                _pluginContext.PreEntityImages["PreImage"] = preImage;

                var error = Assert.ThrowsException<InvalidPluginExecutionException>(
                    () => _context.ExecutePluginWith<ValidatePolicyRulePlugin>(_pluginContext)
                );

                var expectedMessage = $"{attr} is required";

                Assert.AreEqual(error.Message, expectedMessage);
            }
        }

        [TestMethod]
        [TestCategory("RequiredColumnValidation")]
        public void OnUpdate_MissingRequiredAttributeFromTargetOnly_Executes()
        {
            foreach (var attr in ValidatePolicyRulePlugin.RequiredAttributes)
            {
                var target = _target.Clone().ToEntity<dpe_PolicyRule>();

                target[attr] = null;

                _pluginContext.InputParameters["Target"] = target;

                _context.ExecutePluginWith<ValidatePolicyRulePlugin>(_pluginContext);

                //pass
            }
        }

        [TestMethod]
        [TestCategory("MetadataValidation")]
        [DataRow("Create")]
        [DataRow("Update")]
        public void ValidMetadata_Executes(string messageName)
        {
            _pluginContext.MessageName = messageName;
            _context.ExecutePluginWith<ValidatePolicyRulePlugin>(_pluginContext);

            // pass
        }

        [TestMethod]
        [TestCategory("MetadataValidation")]
        [DataRow("Create")]
        [DataRow("Update")]
        public void InvalidEntityMetadata_Throws(string messageName)
        {
            _pluginContext.MessageName = messageName;
            _target.dpe_TargetEntityLogicalName = "another";

            var error = Assert.ThrowsException<InvalidPluginExecutionException>(
                () => _context.ExecutePluginWith<ValidatePolicyRulePlugin>(_pluginContext)
            );

            var expectedMessage = $"{_target.dpe_TargetEntityLogicalName} is not a valid entity";

            Assert.AreEqual(error.Message, expectedMessage);
        }

        [TestMethod]
        [TestCategory("MetadataValidation")]
        [DataRow("Create")]
        [DataRow("Update")]
        public void InvalidAttributeMetadata_Throws(string messageName)
        {
            _pluginContext.MessageName = messageName;
            _target.dpe_TargetAttributeLogicalName = "another";

            var expectedMessage =
                $"{_target.dpe_TargetAttributeLogicalName} is not a valid attribute of {_target.dpe_TargetEntityLogicalName}";

            var error = Assert.ThrowsException<InvalidPluginExecutionException>(
                () => _context.ExecutePluginWith<ValidatePolicyRulePlugin>(_pluginContext)
            );

            Assert.AreEqual(error.Message, expectedMessage);

            _target.dpe_TargetAttributeLogicalName = "name";
            _target.dpe_TriggerAttributeLogicalName = "another";

            expectedMessage =
                $"{_target.dpe_TriggerAttributeLogicalName} is not a valid attribute of {_target.dpe_TargetEntityLogicalName}";

            error = Assert.ThrowsException<InvalidPluginExecutionException>(
                () => _context.ExecutePluginWith<ValidatePolicyRulePlugin>(_pluginContext)
            );

            Assert.AreEqual(error.Message, expectedMessage);
        }

        [TestMethod]
        [TestCategory("ValidateScope")]
        [DataRow("Create")]
        [DataRow("Update")]
        public void InvalidScopeForPolicyType_Throws(string messageName)
        {
            _pluginContext.MessageName = messageName;
            _target.dpe_PolicyType = Models.OptionSets.dpe_policytype.Visible;
            _target.dpe_Scope = Models.OptionSets.dpe_policyscope.ServerOnly;

            var expectedMessage = "A visibility rule can only be applied to the form or both";

            var error = Assert.ThrowsException<InvalidPluginExecutionException>(
                () => _context.ExecutePluginWith<ValidatePolicyRulePlugin>(_pluginContext)
            );

            Assert.AreEqual(error.Message, expectedMessage);
        }
    }
}
