using System;
using System.Collections.Generic;
using System.Text;
using ESS.FW.Bpm.Engine.Common;
using ESS.FW.Bpm.Engine.Impl.DB;
using ESS.FW.Bpm.Engine.Task;
using System.ComponentModel.DataAnnotations.Schema;
using ESS.Shared.Entities.Bpm;

namespace ESS.FW.Bpm.Engine.Persistence.Entity
{




    /// <summary>
	///  
	/// </summary>
	[Serializable]
    public class CommentEntity : IComment, IEvent, IDbEntity
    {


        public const string TypeEvent = "event";
        public const string TypeComment = "comment";


        // If comments would be removeable, revision needs to be added!

        public virtual object GetPersistentState()
        {
            return typeof(CommentEntity);
        }

        public virtual byte[] FullMessageBytes { get; set; }

        public static string MessagePartsMarker = "_|_";

        public virtual void SetMessage(string[] value)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string part in value)
            {
                if (part != null)
                {
                    stringBuilder.Append(part.Replace(MessagePartsMarker, " | "));
                    stringBuilder.Append(MessagePartsMarker);
                }
                else
                {
                    stringBuilder.Append("null");
                    stringBuilder.Append(MessagePartsMarker);
                }
            }
            for (int i = 0; i < MessagePartsMarker.Length; i++)
            {
                stringBuilder.Remove(stringBuilder.Length - 1, 1);
            }
            Message = stringBuilder.ToString();

        }

        public virtual IList<string> MessageParts
        {
            get
            {
                throw new NotImplementedException();
                //if (message == null)
                //{
                //  return null;
                //}
                //IList<string> messageParts = new List<string>();
                //StringTokenizer tokenizer = new StringTokenizer(message, MessagePartsMarker);
                //while (tokenizer.hasMoreTokens())
                //{
                //  string nextToken = tokenizer.nextToken();
                //  if ("null".Equals(nextToken))
                //  {
                //	messageParts.Add(null);
                //  }
                //  else
                //  {
                //	messageParts.Add(nextToken);
                //  }
                //}
                //return messageParts;
            }
        }

        // getters and setters //////////////////////////////////////////////////////

        public virtual string Id { get; set; }


        public virtual string UserId { get; set; }


        public virtual string TaskId { get; set; }




        public virtual string Message { get; set; }

        public virtual DateTime Time { get; set; }


        public virtual string ProcessInstanceId { get; set; }


        public virtual string Type { get; set; }

        [NotMapped]
        public virtual string FullMessage { get; set; }


	  public virtual string Action { get; set; }


	  public virtual string TenantId { get; set; }


	 // public override string ToString()
	 // {
		//return this.GetType().Name + "[id=" + id + ", type=" + type + ", userId=" + userId + ", time=" + time + ", taskId=" + taskId + ", processInstanceId=" + processInstanceId + ", action=" + action + ", message=" + message + ", fullMessage=" + fullMessage + ", tenantId=" + tenantId + "]";
	 // }

    }

}