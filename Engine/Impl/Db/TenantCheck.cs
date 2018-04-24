using System;
using System.Collections.Generic;

namespace ESS.FW.Bpm.Engine.Impl.DB
{
    [Serializable]
    public class TenantCheck
    {
        private const long SerialVersionUid = 1L;



        /// <summary>
        ///     If <code>true</code> then the process engine performs tenant checks to
        ///     ensure that the query only access data that belongs to one of the
        ///     authenticated tenant ids.
        /// </summary>

        public virtual bool IsTenantCheckEnabled { get; set; } = true;

        /// <summary>
        ///     is used by myBatis
        /// </summary>


        /// <summary>
        ///     the ids of the authenticated tenants
        /// </summary>
        public virtual IList<string> AuthTenantIds { get; set; } = new List<string>();
    }
}