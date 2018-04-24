using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using ESS.FW.Bpm.Engine.Common.Utils;
using ESS.FW.Common.Components;
using NUnit.Framework;

namespace Engine.Tests.EF
{
    [TestFixture]
    public class DbModelsTest
    {
        private EnsureUtilLogger Log = UtilsLogger.EnsureUtilLogger;
        [Test]
        public void CreateDb()
        {
            IList<string> sqlToCreate = new List<string>() {
                GetDbFile("create\\oracle.create.engine-tb_bpm_.sql"),
                //GetDbFile("create\\oracle.create.identity-tb_bpm_.sql"),
                //GetDbFile("create\\oracle.create.history-tb_bpm_.sql"),
                //GetDbFile("create\\oracle.create.decision.engine-tb_bpm_.sql"),
                //GetDbFile("create\\oracle.create.decision.history-tb_bpm_.sql"),
                //GetDbFile("create\\oracle.create.case.engine-tb_bpm_.sql"),
                //GetDbFile("create\\oracle.create.case.history-tb_bpm_.sql")
            };
            using (IScope scope = ObjectContainer.BeginLifetimeScope())
            {
                DbContext uof = scope.Resolve<DbContext>();
                foreach (var item in sqlToCreate)
                {
                    try
                    {
                        uof.Database.ExecuteSqlCommand(item);
                    }
                    catch (System.Exception e)
                    {
                        Log.LogDebug("数据表创建失败",e.Message);
                    }
                }
                uof.SaveChanges();
            }
        }
        [Test]
        public void DropDb()
        {
            IList<string> sqlToDrop = new List<string>() {
                //GetDbFile("drop\\oracle.drop.case.engine-tb_bpm_.sql"),
                //GetDbFile("drop\\oracle.drop.case.history-tb_bpm_.sql"),
                //GetDbFile("drop\\oracle.drop.decision.engine-tb_bpm_.sql"),
                //GetDbFile("drop\\oracle.drop.decision.history-tb_bpm_.sql"),
                //GetDbFile("drop\\oracle.drop.identity-tb_bpm_.sql"),
                GetDbFile("drop\\oracle.drop.engine-tb_bpm_.sql"),
            };
            using (IScope scope = ObjectContainer.BeginLifetimeScope())
            {
                DbContext uof = scope.Resolve<DbContext>();
                foreach (var item in sqlToDrop)
                {
                    try
                    {
                        uof.Database.ExecuteSqlCommand(item);
                    }
                    catch (System.Exception e)
                    {
                        Log.LogDebug("数据表删除失败", e.Message);
                    }
                    
                }
                uof.SaveChanges();
            }
        }
        private string GetDbFile(string fileName)
        {
            int sourceCodeIndex = AppDomain.CurrentDomain.BaseDirectory.IndexOf("SourceCode");
            string basePath = AppDomain.CurrentDomain.BaseDirectory.Substring(0, sourceCodeIndex);
            string path=string.Format("{0}Documents\\Deployment\\Bpm\\{1}",basePath , fileName);
            return File.ReadAllText(path,System.Text.Encoding.UTF8);
        }
    }
}
