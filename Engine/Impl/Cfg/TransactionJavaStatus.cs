using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Cfg
{
    public enum TransactionJavaStatus
    {
        /// <summary>
        /// 已经提交
        /// </summary>
        Committed,
        /// <summary>
        /// 已经回滚
        /// </summary>
        RolledBack,
        /// <summary>
        /// 正在提交
        /// </summary>
        Committing,
        /// <summary>
        /// 正在回滚
        /// </summary>
        RollingBack
    }
}
