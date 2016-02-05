﻿using NetBpm.Workflow.Definition;
using NetBpm.Workflow.Definition.Attr;
using NetBpm.Workflow.Execution;
using NetBpm.Workflow.Organisation;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace MyTest
{
    [TestFixture]
    public class HolidayTest : BaseTest
    {
        [Test]
        public void DeployTest()
        {
            FileInfo parFile = new FileInfo("ExamplePar/holiday.par");
            FileStream fstream = parFile.OpenRead();
            byte[] b = new byte[parFile.Length];
            fstream.Read(b, 0, (int)parFile.Length);
            processDefinitionService.DeployProcessArchive(b);

            /*
             select * from [dbo].[NBPM_PROCESSBLOCK];
             select * from [dbo].[NBPM_NODE];
             select * from [dbo].[NBPM_TRANSITION];
             select * from [dbo].[NBPM_ACTION]
             select * from [dbo].[NBPM_ATTRIBUTE];
             select * from [dbo].[NBPM_DELEGATION];
             */
        }

        [Test]
        public void StartProcessTest()
        {
            IProcessInstance processInstance = null;
            Thread.CurrentPrincipal = new PrincipalUserAdapter("af");

            try
            {
                IDictionary attributeValues = new Hashtable();
                attributeValues.Add("start date", new DateTime(2016,3,1));
                attributeValues.Add("end date", new DateTime(2016, 3, 2));

                IProcessDefinition booaction = processDefinitionService.GetProcessDefinition("Holiday request");

                processInstance = executionComponent.StartProcessInstance(booaction.Id, attributeValues);

                Assert.IsNotNull(processInstance);
                Assert.IsNotNull(processInstance.RootFlow);

                /*
                 select *from [dbo].[NBPM_PROCESSINSTANCE]
                 select *from [dbo].[NBPM_FLOW]
                 select *from [dbo].[NBPM_LOG]
                 select *from [dbo].[NBPM_LOGDETAIL]
                 select * from [dbo].[NBPM_ATTRIBUTE];
                 select * from [dbo].[NBPM_ATTRIBUTEINSTANCE]
                 */
            }
            catch (ExecutionException e)
            {
                Assert.Fail("ExcecutionException while starting a new holiday request: " + e.Message);
            }
            finally
            {
                //      loginUtil.logout();
            }
        }

        [Test]
        public void ProcessActivityTest()
        {
            Thread.CurrentPrincipal = new PrincipalUserAdapter("ae");

            try
            {
                //af申請的，ae是af的主管，登入者要換成ae
                var taskLists = executionComponent.GetTaskList("ae");

                IDictionary attributeValues = new Hashtable();
                attributeValues.Add("evaluation result", Evaluation.APPROVE);

                foreach (IFlow task in taskLists)
                {
                    //出現一個無法處理的錯誤
                    executionComponent.PerformActivity(task.Id, attributeValues);
                }
            }
            catch (ExecutionException e)
            {
                Assert.Fail("ExcecutionException while starting a new holiday request: " + e.Message);
            }
            finally
            {
                //      loginUtil.logout();
            }
        }
    }
}
