using System;

namespace ESS.FW.Bpm.Engine.Authorization
{    
    /// <summary>
     ///     <para>
     ///         A permission represents an authorization to interact with a given
     ///         resource in a specific way. See <seealso cref="Permissions" /> for a set of built-in
     ///         permissions and <seealso cref="IAuthorization" /> for general overview on authorizations.
     ///     </para>
     ///     <para>
     ///         In camunda BPM, multiple permissions are grouped into an <seealso cref="IAuthorization" />.
     ///         For efficient storage and checking of authorizations, the permissons that make
     ///         up an autorization are coded into a single integer.
     ///         The implication of this design is that a permission must have a unique integer value
     ///         and it must be a power of two, ie 2^0, 2^1, 2^2, 2^3, 2^4 ...
     ///         The permission can then be added to an authorization using bitwise OR:
     ///         <pre>
     ///             Auth: 0000001001001
     ///             Perm to add: 0000000010000
     ///             bit OR (|) : 0000001011001
     ///         </pre>
     ///         and removed using bitwise AND of the inverted value:
     ///         <pre>
     ///             Auth: 0000001001001
     ///             Perm to rem: 0000000001000
     ///             invert (~) : 1111111110111
     ///             bit AND (&): 0000001000001
     ///         </pre>
     ///         <h2>Defining a custom Permission</h2>
     ///         The <seealso cref="Permissions" /> class contains the values of the  built-in
     ///         permissions. In order to define a custom permission, you must provide
     ///         an implementation of this interface such that the <seealso cref="#getValue()" />
     ///         method returns an integer which is a power of two and not yet used by the
     ///         built-in <seealso cref="Permissions" /> and not reserved (values <=2^14 are reserved).
     ///         Valid example: 2^15=32768.</para>
     /// </summary>
    [Flags]
    public enum Permissions
    {
        /// <summary>
        ///     The none permission means 'no action', 'doing nothing'.
        ///     It does not mean that no permissions are granted.
        /// </summary>
        None = 0, //("NONE", 0),

        /// <summary>
        ///     Indicates that  all interactions are permitted.
        ///     If ALL is revoked it means that the user is not permitted
        ///     to do everything, which means that at least one permission
        ///     is revoked. This does not implicate that all individual
        ///     permissions are revoked.
        ///     Example: If the UPDATE permission is revoke also the ALL
        ///     permission is revoked, because the user is not authorized
        ///     to execute all actions anymore.
        /// </summary>
        All = int.MaxValue, //("ALL", Integer.MAX_VALUE),

        /// <summary>
        ///     Indicates that READ interactions are permitted.
        /// </summary>
        Read = 2, //("READ", 2),

        /// <summary>
        ///     Indicates that UPDATE interactions are permitted.
        /// </summary>
        Update = 4, //("UPDATE", 4),

        /// <summary>
        ///     Indicates that CREATE interactions are permitted.
        /// </summary>
        Create = 8, //("CREATE", 8),

        /// <summary>
        ///     Indicates that DELETE interactions are permitted.
        /// </summary>
        Delete = 16, //("DELETE", 16),

        /// <summary>
        ///     Indicates that ACCESS interactions are permitted.
        /// </summary>
        Access = 32, //("ACCESS", 32),

        /// <summary>
        ///     Indicates that READ_TASK interactions are permitted.
        /// </summary>
        ReadTask = 64, //("READ_TASK", 64),

        /// <summary>
        ///     Indicates that UPDATE_TASK interactions are permitted.
        /// </summary>
        UpdateTask = 128, //("UPDATE_TASK", 128),

        /// <summary>
        ///     Indicates that CREATE_INSTANCE interactions are permitted.
        /// </summary>
        CreateInstance = 256, //("CREATE_INSTANCE", 256),

        /// <summary>
        ///     Indicates that READ_INSTANCE interactions are permitted.
        /// </summary>
        ReadInstance = 512, //("READ_INSTANCE", 512),

        /// <summary>
        ///     Indicates that UPDATE_INSTANCE interactions are permitted.
        /// </summary>
        UpdateInstance = 1024, //("UPDATE_INSTANCE", 1024),

        /// <summary>
        ///     Indicates that DELETE_INSTANCE interactions are permitted.
        /// </summary>
        DeleteInstance = 2048, //("DELETE_INSTANCE", 2048),

        /// <summary>
        ///     Indicates that UPDATE_INSTANCE interactions are permitted.
        /// </summary>
        ReadHistory = 4096, //("READ_HISTORY", 4096),

        /// <summary>
        ///     Indicates that DELETE_INSTANCE interactions are permitted.
        /// </summary>
        DeleteHistory = 8192, //("DELETE_HISTORY", 8192),

        /// <summary>
        ///     Indicates that TASK_WORK interactions are permitted
        /// </summary>
        TaskWork = 16384, //("TASK_WORK", 16384),

        /// <summary>
        ///     Indicates that TASK_ASSIGN interactions are permitted
        /// </summary>
        TaskAssign = 32768, //("TASK_ASSIGN", 32768),

        /// <summary>
        ///     Indicates that MIGRATE_INSTANCE interactions are permitted
        /// </summary>
        MigrateInstance = 65536 //("MIGRATE_INSTANCE", 65536);

        // implmentation //////////////////////////
    }
}