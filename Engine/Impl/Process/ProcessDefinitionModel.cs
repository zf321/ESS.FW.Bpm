//using System;
//using System.Collections.Generic;
//using ESS.FW.Bpm.Engine.Impl.Bpmn.Parser;
//using ESS.FW.Bpm.Engine.Impl.Pvm.Process;
//using ESS.FW.Bpm.Engine.Impl.task;
//using ESS.FW.Bpm.Engine.Repository.Impl;

//namespace ESS.FW.Bpm.Engine.Impl.Process
//{
//    public class ProcessDefinitionEntity : ProcessDefinitionImpl, IResourceDefinitionEntity
//    {
//        public ProcessDefinitionEntity() : base(null)
//        {
//        }

//        public ProcessDefinitionEntity(string id) : base(id)
//        {
//        }

//        public bool Suspended { get; set; }
//        public string VersionTag { get; set; }
//        public bool StartFormKey { get; set; }
//        public virtual IDictionary<string, TaskDefinition> TaskDefinitions { get; set; }

//        /// <summary>
//        ///     图形符号定义
//        /// </summary>
//        public bool GraphicalNotationDefined { get; internal set; }

//        public string Category { get; set; }


//        public string Key { get; set; }

//        public string ResourceName { get; set; }

//        public string TenantId { get; set; }

//        public int Version { get; set; }

//        public IResourceDefinitionEntity PreviousDefinition
//        {
//            get
//            {
//                //TODO PreviousDefinition Get
//                throw new NotImplementedException();
//            }
//        }

//        public bool HasStartFormKey()
//        {
//            return StartFormKey;
//            
//        }

//        public IList<ActivityImpl> GetActivities()
//        {
//            return FlowActivities;
//        }

//        public string GetDescription()
//        {
//            return GetProperty(BpmnParse.PropertynameDocumentation).ToString();
//        }
//    }
//}