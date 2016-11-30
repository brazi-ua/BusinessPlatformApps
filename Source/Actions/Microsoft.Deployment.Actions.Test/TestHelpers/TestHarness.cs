﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.AppLoad;
using Microsoft.Deployment.Common.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Deployment.Actions.Test.TestHelpers;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json;

namespace Microsoft.Deployment.Actions.Test
{
    [TestClass]
    public class TestHarness
    {
        private static CommonController Controller { get; set; }
        public static string TemplateName = "TestApp";
        private static DataStore CommonDataStore = null;

        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            AppFactory factory = new AppFactory();
            CommonControllerModel model = new CommonControllerModel()
            {
                AppFactory = factory,
                AppRootFilePath = factory.AppPath,
                SiteCommonFilePath = factory.SiteCommonPath,
                ServiceRootFilePath = factory.SiteCommonPath + "../",
                Source = "TEST",
            };

            Controller = new CommonController(model);
            Credential.Load();
        }

        public static ActionResponse ExecuteAction(string actionName, DataStore datastore)
        {
            UserInfo info = new UserInfo();
            info.ActionName = actionName;
            info.AppName = TemplateName;
            return Controller.ExecuteAction(info, new ActionRequest() { DataStore = datastore }).Result;
        }

        public static async  Task<ActionResponse> ExecuteActionAsync(string actionName, DataStore datastore)
        {
            UserInfo info = new UserInfo();
            info.ActionName = actionName;
            info.AppName = TemplateName;
            return await Controller.ExecuteAction(info, new ActionRequest() { DataStore = datastore });
        }

        public static async Task<DataStore> GetCommonDataStore()
        {
            if (CommonDataStore == null)
            {
                await SetUp();
            }

            DataStore store =
                JsonConvert.DeserializeObject<DataStore>(JsonUtility.GetJsonStringFromObject(CommonDataStore));
            return store;
        }

        public static async Task<bool> SetUp()
        {
            if(!System.Diagnostics.Debugger.IsAttached)
            {
                CommonDataStore = await AAD.GetTokenWithDataStore();
            }
            else
            {
                CommonDataStore = await AAD.GetUserTokenFromPopup();
            }

            var subscriptionResult = await TestHarness.ExecuteActionAsync("Microsoft-GetAzureSubscriptions", CommonDataStore);
            Assert.IsTrue(subscriptionResult.IsSuccess);
            var subscriptionId = subscriptionResult.Body.GetJObject()["value"][0];
            CommonDataStore.AddToDataStore("SelectedSubscription", subscriptionId, DataStoreType.Public);

            var locationResult = await TestHarness.ExecuteActionAsync("Microsoft-GetLocations", CommonDataStore);
            Assert.IsTrue(locationResult.IsSuccess);
            var location = locationResult.Body.GetJObject()["value"][5];
            CommonDataStore.AddToDataStore("SelectedLocation", location, DataStoreType.Public);

            CommonDataStore.AddToDataStore("SelectedResourceGroup", "Test");

            if (System.Diagnostics.Debugger.IsAttached)
            {
                var deleteResourceGroupResult = await TestHarness.ExecuteActionAsync("Microsoft-DeleteResourceGroup", CommonDataStore);
            }

            var resourceGroupResult = await TestHarness.ExecuteActionAsync("Microsoft-CreateResourceGroup", CommonDataStore);
            Assert.IsTrue(resourceGroupResult.IsSuccess);

            return true;
        }

    }
}
