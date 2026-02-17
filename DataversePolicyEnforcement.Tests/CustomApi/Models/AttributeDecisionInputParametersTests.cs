using DataversePolicyEnforcement.CustomApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace DataversePolicyEnforcement.Tests.CustomApi.Models
{
    [TestClass]
    public class AttributeDecisionInputParametersTests
    {
        protected ParameterCollection _inputParameters;

        public AttributeDecisionInputParametersTests()
        {
            _inputParameters = new ParameterCollection
            {
                { "dpe_gad_entitylogicalname", "account" },
                { "dpe_gad_triggerattributelogicalname", "name" },
                { "dpe_gad_currentvalue", "Contoso" },
                {
                    "dpe_gad_targetattributelogicalnames",
                    new string[] { "telephone1", "emailaddress1" }
                }
            };
        }

        [TestMethod]
        [TestCategory("Collection")]
        public void Constructor_WithValidParameters_ShouldInitializeProperties()
        {
            var parameters = new AttributeDecisionInputParameters(_inputParameters);
            Assert.AreEqual("account", parameters.EntityLogicalName);
            Assert.AreEqual("name", parameters.TriggerAttributeLogicalName);
            Assert.AreEqual("Contoso", parameters.CurrentValue);
            CollectionAssert.AreEqual(
                new List<string> { "telephone1", "emailaddress1" },
                parameters.TargetAttributeLogicalNames
            );
        }

        [TestMethod]
        [TestCategory("Collection")]
        public void Constructor_NullCollection_ShouldThrowException()
        {
            var error = Assert.ThrowsException<ArgumentNullException>(
                () => new AttributeDecisionInputParameters(null)
            );
            Assert.IsTrue(error.Message.Contains("Collection is required and cannot be null"));
        }

        [TestMethod]
        [TestCategory("Collection")]
        [DataRow("dpe_gad_entitylogicalname")]
        [DataRow("dpe_gad_triggerattributelogicalname")]
        [DataRow("dpe_gad_currentvalue")]
        public void Constructor_MissingRequiredInput_ShouldThrowException(string input)
        {
            _inputParameters.Remove(input);

            var error = Assert.ThrowsException<ArgumentException>(
                () => new AttributeDecisionInputParameters(_inputParameters)
            );
            Assert.IsTrue(error.Message.Contains("Missing required parameter: " + input));
        }

        [TestMethod]
        [TestCategory("Collection")]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void Constructor_EmptyEntityLogicalName_ShouldThrowException(string value)
        {
            _inputParameters["dpe_gad_entitylogicalname"] = value;
            var error = Assert.ThrowsException<ArgumentException>(
                () => new AttributeDecisionInputParameters(_inputParameters)
            );
            Assert.AreEqual("Entity logical name cannot be null or empty.", error.Message);
        }

        [TestMethod]
        [TestCategory("Collection")]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("   ")]
        public void Constructor_TriggerAttributeLogicalName_NullEmptyWhitespace_ShouldThrowException(
            string value
        )
        {
            _inputParameters["dpe_gad_triggerattributelogicalnames"] = value;
            var error = Assert.ThrowsException<ArgumentException>(
                () => new AttributeDecisionInputParameters(_inputParameters)
            );
            Assert.AreEqual(
                "Trigger attribute logical name cannot be null or empty.",
                error.Message
            );
        }

        [TestMethod]
        [TestCategory("Collection")]
        public void Constructor_CurrentValueNull_ShouldInitialize()
        {
            _inputParameters["dpe_gad_currentvalue"] = null;
            var parameters = new AttributeDecisionInputParameters(_inputParameters);
            Assert.IsNull(parameters.CurrentValue);
        }

        [TestMethod]
        [TestCategory("Collection")]
        public void Constructor_TargetAttributeLogicalNames_Null_ShouldInitialize()
        {
            _inputParameters["dpe_gad_targetattributelogicalnames"] = null;
            var parameters = new AttributeDecisionInputParameters(_inputParameters);
            Assert.IsNotNull(parameters.TargetAttributeLogicalNames);
            Assert.AreEqual(0, parameters.TargetAttributeLogicalNames.Count);
        }

        [TestMethod]
        [TestCategory("Collection")]
        public void Constructor_TargetAttributeLogicalNames_EmptyStringArray_ShouldInitialize()
        {
            _inputParameters["dpe_gad_targetattributelogicalnames"] = new string[0];
            var parameters = new AttributeDecisionInputParameters(_inputParameters);
            Assert.IsNotNull(parameters.TargetAttributeLogicalNames);
            Assert.AreEqual(0, parameters.TargetAttributeLogicalNames.Count);
        }
    }
}
