﻿using Microsoft.Deployment.Actions.Test.TestHelpers;
using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.Helpers;
namespace Microsoft.Deployment.Actions.Test.ActionsTest
{
    [TestClass]
    public class AzureFunctionsTests
    {
        public static string randomString = RandomGenerator.GetRandomLowerCaseCharacters(5);

        [TestMethod]
        public async Task DeployAzureFunction()
        {
            var dataStore = await TestHarness.GetCommonDataStore();

            dataStore.AddToDataStore("DeploymentName", "FunctionDeploymentTest");
            dataStore.AddToDataStore("FunctionAppHostingPlan", "FunctionPlanName");
            dataStore.AddToDataStore("SiteName", "UnitTestTrialbpst" + randomString);

            var response = TestHarness.ExecuteAction("Microsoft-DeployAzureFunction", dataStore);
            Assert.IsTrue(response.IsSuccess);

            response = TestHarness.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
           Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task DeployAzureFunctionAssets()
        {
            await this.DeployAzureFunction();
            var dataStore = await TestHarness.GetCommonDataStore();

            dataStore.AddToDataStore("DeploymentName", "FunctionDeploymentTest");
            dataStore.AddToDataStore("FunctionAppHostingPlan", "FunctionPlanName");
            dataStore.AddToDataStore("SiteName", "UnitTestTrialbpst" + randomString);

            dataStore.AddToDataStore("FunctionName", "TestA");
            dataStore.AddToDataStore("FunctionFileName", "TweetFunctionCSharp.cs" );

            var response = TestHarness.ExecuteAction("Microsoft-DeployAzureFunctionAssets", dataStore);
            Assert.IsTrue(response.Status == ActionStatus.Success);

        }

        [TestMethod]
        public async Task DeployTwitterCSharpFunctionAssetsTest()
        {
            await this.DeployAzureFunction();
            var dataStore = await TestHarness.GetCommonDataStore();

            dataStore.AddToDataStore("DeploymentName", "FunctionDeploymentTest");
            dataStore.AddToDataStore("FunctionAppHostingPlan", "FunctionPlanName");
            dataStore.AddToDataStore("SiteName", "UnitTestTrialbpst" + randomString);

            dataStore.AddToDataStore("FunctionName", "TestA");
            dataStore.AddToDataStore("FunctionFileName", "TweetFunctionCSharp.cs");

            var response = TestHarness.ExecuteAction("Microsoft-DeployTwitterCSharpFunctionAssets", dataStore);
            Assert.IsTrue(response.Status == ActionStatus.Success);

        }

    }
}

