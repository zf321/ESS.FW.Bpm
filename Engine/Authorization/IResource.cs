//namespace ESS.FW.Bpm.Engine.Authorization
//{
//    /// <summary>
//    ///     <para>
//    ///         Resources are entities for which a user or a group is authorized. Examples of
//    ///         resources are applications, process-definitions, process-instances, tasks ...
//    ///     </para>
//    ///     <para>
//    ///         A resource has a type and an id. The type (<seealso cref="#setResource(String)" />)
//    ///         allows to group all resources of the same kind. A resource id is the identifier of
//    ///         an individual resource instance (<seealso cref="#setResourceId(String)" />). For example:
//    ///         the resource type could be "processDefinition" and the resource-id could be the
//    ///         id of an individual process definition.
//    ///     </para>
//    ///     <para>See <seealso cref="Resources" /> for a set of built-in resource constants.</para>
//    ///     
//    /// </summary>
//    /// <seealso cref= "Resources"></seealso>
//    public interface IResource
//    {
//        /// <summary>
//        ///     returns the name of the resource
//        /// </summary>
//        string GetResourceName();

//        /// <summary>
//        ///     an integer representing the type of the resource.
//        /// </summary>
//        /// <returns> the type identitfyer of the resource  </returns>
//        int GetResourceType();
//    }
//}