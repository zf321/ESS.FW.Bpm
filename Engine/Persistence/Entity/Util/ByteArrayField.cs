using ESS.FW.Bpm.Engine.context.Impl;
using ESS.FW.Bpm.Engine.Persistence.Entity.Impl;
using ESS.FW.Bpm.Engine.Variable.Serializer;
using ESS.FW.Common.Components;

namespace ESS.FW.Bpm.Engine.Persistence.Entity.Util
{
    /// <summary>
    ///     A byte array value field what load and save <seealso cref="ResourceEntity" />. It can
    ///     be used in an entity which implements <seealso cref="IValueFields" />.
    /// </summary>
    public class ByteArrayField
    {
        protected internal readonly INameable NameProvider;
        protected internal string byteArrayId;

        protected ResourceEntity byteArrayValue;

        public ByteArrayField(INameable nameProvider)
        {
            NameProvider = nameProvider;
        }

        public virtual string ByteArrayId
        {
            get { return byteArrayId; }
            set
            {
                byteArrayId = value;
                if (value != null)
                {
                    byteArrayValue = GetByteArrayEntity();
                }
                else
                {
                    byteArrayValue = null;
                }

            }
        }

        protected internal virtual ResourceEntity ResourceEntity
        {
            get { return byteArrayValue; }
        }


        public virtual byte[] GetByteArrayValue()
        {
            GetByteArrayEntity();

            if (byteArrayValue != null)
                return byteArrayValue.Bytes;
            return null;
        }

        protected ResourceEntity GetByteArrayEntity()
        {
            if (byteArrayValue == null)
                if (byteArrayId != null)
                    if (Context.CommandContext != null)
                            byteArrayValue =
                                Context.CommandContext.ByteArrayManager.Get(byteArrayId);

            return byteArrayValue;
        }

        public virtual void SetByteArrayValue(byte[] bytes)
        {
            if (bytes != null)
                if (byteArrayId != null && ResourceEntity != null)
                {
                    byteArrayValue.Bytes = bytes;
                }
                else
                {
                    DeleteByteArrayValue();
                    //TODO 每次新建ResourceEntity？
                    if (Context.CommandContext != null)
                    {
                        byteArrayValue = new ResourceEntity(NameProvider.Name, bytes);
                        Context.CommandContext.ByteArrayManager.Add(byteArrayValue);
                        byteArrayId = byteArrayValue.Id;
                    }
                    else//TODO 不在cmd中
                    {
                        using (IScope scope = ObjectContainer.BeginLifetimeScope())
                        {
                            IByteArrayManager manager = scope.Resolve<IByteArrayManager>();
                            byteArrayValue = new ResourceEntity(NameProvider.Name, bytes);
                            var bytearr = manager.Add(byteArrayValue);
                            byteArrayId = byteArrayValue.Id;
                        }
                        //throw new System.NotImplementedException("CommandContext is null");
                    }
                }
            else
                DeleteByteArrayValue();
        }

        public virtual void DeleteByteArrayValue()
        {
            if (byteArrayId != null)
            {
                // the next apparently useless line is probably to ensure consistency in the DbSqlSession cache,
                // but should be checked and docked here (or removed if it turns out to be unnecessary)
                GetByteArrayEntity();

                if (byteArrayValue != null)
                    Context.CommandContext.ByteArrayManager.Delete(byteArrayValue);

                byteArrayId = null;
            }
        }

        public virtual void SetByteArrayValue(ResourceEntity byteArrayValue)
        {
            this.byteArrayValue = byteArrayValue;
        }
    }
}