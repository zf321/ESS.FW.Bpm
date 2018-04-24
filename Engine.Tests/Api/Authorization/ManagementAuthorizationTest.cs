using System.Collections.Generic;
using ESS.FW.Bpm.Engine.Authorization;
using ESS.FW.Bpm.Engine.Management;
using ESS.FW.Bpm.Engine.Persistence.Entity;
using NUnit.Framework;

namespace Engine.Tests.Api.Authorization
{

    /// <summary>
    /// 
    /// 
    /// </summary>
    public class ManagementAuthorizationTest : AuthorizationTest
    {

        // get table Count //////////////////////////////////////////////

        public virtual void testGetTableCountWithoutAuthorization()
        {
            // given

            try
            {
                // when
                var obj = managementService.TableCount;
                Assert.Fail("Exception expected: It should not be possible to get the table Count");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent("ENGINE-03029 Required authenticated group 'camunda-admin'", message);
            }
        }

        public virtual void testGetTableCountAsCamundaAdmin()
        {
            // given
            //identityService.SetAuthentication(userId, Collections.singletonList(Groups.GroupsFields.CamundaAdmin));
            identityService.SetAuthentication(userId, new List<string>() { GroupsFields.CamundaAdmin });

            // when
            IDictionary<string, long> tableCount = managementService.TableCount;

            // then
            Assert.IsFalse(tableCount.Count == 0);
        }

        // get table name //////////////////////////////////////////////

        public virtual void testGetTableNameWithoutAuthorization()
        {
            // given

            try
            {
                // when
                managementService.GetTableName(typeof(ProcessDefinitionEntity));
                Assert.Fail("Exception expected: It should not be possible to get the table name");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent("ENGINE-03029 Required authenticated group 'camunda-admin'", message);
            }
        }

        public virtual void testGetTableNameAsCamundaAdmin()
        {
            // given
            //identityService.SetAuthentication(userId, Collections.singletonList(Groups.GroupsFields.CamundaAdmin));
            identityService.SetAuthentication(userId, new List<string>() { GroupsFields.CamundaAdmin });
            string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;

            // when
            string tableName = managementService.GetTableName(typeof(ProcessDefinitionEntity));

            // then
            Assert.AreEqual(tablePrefix + "ACT_RE_PROCDEF", tableName);
        }

        // get table meta data //////////////////////////////////////////////

        public virtual void testGetTableMetaDataWithoutAuthorization()
        {
            // given

            try
            {
                // when
                managementService.GetTableMetaData("ACT_RE_PROCDEF");
                Assert.Fail("Exception expected: It should not be possible to get the table meta data");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent("ENGINE-03029 Required authenticated group 'camunda-admin'", message);
            }
        }

        public virtual void testGetTableMetaDataAsCamundaAdmin()
        {
            // given
            //identityService.SetAuthentication(userId, Collections.singletonList(Groups.GroupsFields.CamundaAdmin));
            identityService.SetAuthentication(userId, new List<string>() { GroupsFields.CamundaAdmin });
            // when
            TableMetaData tableMetaData = managementService.GetTableMetaData("ACT_RE_PROCDEF");

            // then
            Assert.NotNull(tableMetaData);
        }

        // table page query //////////////////////////////////

        public virtual void testTablePageQueryWithoutAuthorization()
        {
            // given

            try
            {
                // when
                //managementService.CreateTablePageQuery().TableName("ACT_RE_PROCDEF").ListPage(0, int.MaxValue);
                Assert.Fail("Exception expected: It should not be possible to get a table page");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent("ENGINE-03029 Required authenticated group 'camunda-admin'", message);
            }

        }

        public virtual void testTablePageQueryAsCamundaAdmin()
        {
            // given
            //identityService.SetAuthentication(userId, Collections.singletonList(Groups.GroupsFields.CamundaAdmin));
            identityService.SetAuthentication(userId, new List<string>() { GroupsFields.CamundaAdmin });
            string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;

            // when
            //TablePage page = managementService.CreateTablePageQuery().TableName(tablePrefix + "ACT_RE_PROCDEF").ListPage(0, int.MaxValue);

            // then
            //Assert.NotNull(page);
        }

        // get history level /////////////////////////////////

        public virtual void testGetHistoryLevelWithoutAuthorization()
        {
            //given

            try
            {
                // when
                var obj = managementService.HistoryLevel;
                Assert.Fail("Exception expected: It should not be possible to get the history level");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent("ENGINE-03029 Required authenticated group 'camunda-admin'", message);
            }
        }

        public virtual void testGetHistoryLevelAsCamundaAdmin()
        {
            //given
            //identityService.SetAuthentication(userId, Collections.singletonList(Groups.GroupsFields.CamundaAdmin));
            identityService.SetAuthentication(userId, new List<string>() { GroupsFields.CamundaAdmin });
            // when
            int historyLevel = managementService.HistoryLevel;

            // then
            Assert.AreEqual(processEngineConfiguration.HistoryLevel.Id, historyLevel);
        }

        // database schema upgrade ///////////////////////////

        
        public virtual void testDataSchemaUpgradeWithoutAuthorization()
        {
            // given

            try
            {
                // when
                // Todo:IManagementService.DatabaseSchemaUpgrade
                //managementService.DatabaseSchemaUpgrade(null, null, null);
                Assert.Fail("Exception expected: It should not be possible to upgrade the database schema");
            }
            catch (AuthorizationException e)
            {
                // then
                string message = e.Message;
                AssertTextPresent("ENGINE-03029 Required authenticated group 'camunda-admin'", message);
            }
        }

    }

}