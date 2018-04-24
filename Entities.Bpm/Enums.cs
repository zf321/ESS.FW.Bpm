using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.Shared.Entities.Bpm
{
    public enum EnvironmentVariableCategory {
        [Description("COMPONENT")]
        Component
    }

    public enum DynamicFormType
    {
        /// <summary>
        /// 基础控件
        /// </summary>
        [Display(Name ="基础控件")]
        BaseControl,
        /// <summary>
        /// 模板表单
        /// </summary>
        [Display(Name ="模板控件")]
        LayoutControl
    }
}
