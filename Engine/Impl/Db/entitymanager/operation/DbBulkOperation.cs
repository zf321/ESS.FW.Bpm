using System;

namespace ESS.FW.Bpm.Engine.Impl.DB.EntityManager.Operation
{
    /// <summary>
    ///     A bulk operation
    ///     
    /// </summary>
    public class DbBulkOperation : DbOperation
    {
        protected internal object parameter;

        protected internal string statement;

        public DbBulkOperation()
        {
        }

        public DbBulkOperation(DbOperationType operationType, Type entityType, string statement, object parameter)
        {
            this.operationType = operationType;
            this.entityType = entityType;
            this.statement = statement;
            this.parameter = parameter;
        }

        public override bool Failed
        {
            get { return false; }
        }

        public virtual object Parameter
        {
            get { return parameter; }
            set { parameter = value; }
        }


        public virtual string Statement
        {
            get { return statement; }
            set { statement = value; }
        }

        public override void Recycle()
        {
            statement = null;
            parameter = null;
            base.Recycle();
        }


        public override string ToString()
        {
            return operationType + " " + statement + " " + parameter;
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            var result = 1;
            result = prime*result + (parameter == null ? 0 : parameter.GetHashCode());
            result = prime*result + (ReferenceEquals(statement, null) ? 0 : statement.GetHashCode());
            return result;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            var other = (DbBulkOperation) obj;
            if (parameter == null)
            {
                if (other.parameter != null)
                    return false;
            }
            else if (!parameter.Equals(other.parameter))
            {
                return false;
            }
            if (ReferenceEquals(statement, null))
            {
                if (!ReferenceEquals(other.statement, null))
                    return false;
            }
            else if (!statement.Equals(other.statement))
            {
                return false;
            }
            return true;
        }
    }
}