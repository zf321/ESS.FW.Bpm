using System;



namespace ESS.FW.Bpm.Model.Bpmn.builder
{

    /// <summary>
    /// 
    /// </summary>
    public class AbstractEmbeddedSubProcessBuilder<TE> where TE : AbstractSubProcessBuilder
    {

        protected internal readonly TE SubProcessBuilder;
        //protected internal readonly TB Myself;
        
        protected internal AbstractEmbeddedSubProcessBuilder(TE subProcessBuilder)
        {
            //this.myself = (B) selfType.cast(this);
            this.SubProcessBuilder = subProcessBuilder;
        }

    }

}