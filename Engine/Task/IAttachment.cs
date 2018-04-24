namespace ESS.FW.Bpm.Engine.Task
{
    /// <summary>
    ///     Any type of content that is be associated with
    ///     a task or with a process instance.
    ///      
    /// </summary>
    public interface IAttachment
    {
        /// <summary>
        ///     unique id for this attachment
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     free user defined short (max 255 chars) name for this attachment
        /// </summary>
        string Name { get; set; }


        /// <summary>
        ///     long (max 255 chars) explanation what this attachment is about in context of the task and/or process instance it's
        ///     linked to.
        /// </summary>
        string Description { get; set; }


        /// <summary>
        ///     indication of the type of content that this attachment refers to. Can be mime type or any other indication.
        /// </summary>
        string Type { get; }

        /// <summary>
        ///     reference to the task to which this attachment is associated.
        /// </summary>
        string TaskId { get; }

        /// <summary>
        ///     reference to the process instance to which this attachment is associated.
        /// </summary>
        string ProcessInstanceId { get; }

        /// <summary>
        ///     the remote Uri in case this is remote content.  If the attachment content was
        ///     <seealso
        ///         cref="ITaskService#createAttachment(String, String, String, String, String, java.IO.InputStream) uploaded with an input stream" />
        ///     ,
        ///     then this method returns null and the content can be fetched with
        ///     <seealso cref="ITaskService#getAttachmentContent(String)" />.
        /// </summary>
        string Url { get; }
    }
}