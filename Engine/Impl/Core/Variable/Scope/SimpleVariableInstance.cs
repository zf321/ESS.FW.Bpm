//using System;
//using ESS.FW.Bpm.Engine.Variable.Type;
//using ESS.FW.Bpm.Engine.Variable.Value;

//namespace ESS.FW.Bpm.Engine.Impl.Core.Variable.Scope
//{
//    /// <summary>
//    ///     
//    /// </summary>
//    public class SimpleVariableInstance : ICoreVariableInstance
//    {
//        protected internal string name;
//        protected internal ITypedValue value;

//        public SimpleVariableInstance(string name, ITypedValue value)
//        {
//            this.name = name;
//            this.value = value;
//        }

//        public virtual string Name
//        {
//            get { return name; }
//            set { name = value; }
//        }

//        public virtual ITypedValue GetTypedValue(bool deserialize)
//        {
//            return value;
//        }

//        public bool GetIsTransient()
//        {
//            return false;
//        }

//        public void Delete()
//        {
//        }

//        public void IncrementSequenceCounter()
//        {
//        }

//        public bool IsConcurrentLocal { get; set; }

//        public virtual ITypedValue Value
//        {
//            set { this.value = value; }
//        }

//        public class SimpleVariableInstanceFactory : IVariableInstanceFactory<SimpleVariableInstance>
//        {
//            public static readonly SimpleVariableInstanceFactory Instance = new SimpleVariableInstanceFactory();

//            public virtual SimpleVariableInstance Build(string name, ITypedValue value, bool isTransient)
//            {
//                return new SimpleVariableInstance(name, value);
//            }
//        }
//    }
//}