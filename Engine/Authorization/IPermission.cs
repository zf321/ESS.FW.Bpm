//namespace ESS.FW.Bpm.Engine.Authorization
//{
//    /// <summary>
//    ///     <para>
//    ///         A permission represents an authorization to interact with a given
//    ///         resource in a specific way. See <seealso cref="Permissions" /> for a set of built-in
//    ///         permissions and <seealso cref="IAuthorization" /> for general overview on authorizations.
//    ///     </para>
//    ///     <para>
//    ///         In camunda BPM, multiple permissions are grouped into an <seealso cref="IAuthorization" />.
//    ///         For efficient storage and checking of authorizations, the permissons that make
//    ///         up an autorization are coded into a single integer.
//    ///         The implication of this design is that a permission must have a unique integer value
//    ///         and it must be a power of two, ie 2^0, 2^1, 2^2, 2^3, 2^4 ...
//    ///         The permission can then be added to an authorization using bitwise OR:
//    ///         <pre>
//    ///             Auth: 0000001001001
//    ///             Perm to add: 0000000010000
//    ///             bit OR (|) : 0000001011001
//    ///         </pre>
//    ///         and removed using bitwise AND of the inverted value:
//    ///         <pre>
//    ///             Auth: 0000001001001
//    ///             Perm to rem: 0000000001000
//    ///             invert (~) : 1111111110111
//    ///             bit AND (&): 0000001000001
//    ///         </pre>
//    ///         <h2>Defining a custom Permission</h2>
//    ///         The <seealso cref="Permissions" /> class contains the values of the  built-in
//    ///         permissions. In order to define a custom permission, you must provide
//    ///         an implementation of this interface such that the <seealso cref="#getValue()" />
//    ///         method returns an integer which is a power of two and not yet used by the
//    ///         built-in <seealso cref="Permissions" /> and not reserved (values <=2^14 are reserved).
//    ///         Valid example: 2^15=32768.</para>
//    ///     
//    ///     
//    /// </summary>
//    public interface IPermission
//    {
//        /// <summary>
//        ///     returns the name of the perwission, ie. 'WRITE'
//        /// </summary>
//        string Name { get; }

//        /// <summary>
//        ///     returns the unique numeric value of the permission.
//        ///     Must be a power of 2. ie 2^0, 2^1, 2^2, 2^3, 2^4 ...
//        /// </summary>
//        int Value { get; }
//    }
//}