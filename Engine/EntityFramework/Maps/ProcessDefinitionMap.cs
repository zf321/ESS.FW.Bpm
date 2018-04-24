using ESS.FW.Bpm.Engine.Persistence.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.EntityFramework.Maps
{
    public class ProcessDefinitionMap : IEntityTypeConfiguration<ProcessDefinitionEntity>
    {
        public void Configure(EntityTypeBuilder<ProcessDefinitionEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_RE_PROCDEF");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Revision).HasColumnName("REV");
            builder.Property(c => c.Category).HasColumnName("CATEGORY");
            builder.Property(c => c.Name).HasColumnName("NAME");
            builder.Property(c => c.Key).HasColumnName("KEY");
            builder.Property(c => c.Version).HasColumnName("VERSION");
            builder.Property(c => c.DeploymentId).HasColumnName("DEPLOYMENT_ID");
            builder.Property(c => c.ResourceName).HasColumnName("RESOURCE_NAME");
            builder.Property(c => c.DiagramResourceName).HasColumnName("DGRM_RESOURCE_NAME");
            builder.Property(c => c.HasStartFormKey).HasColumnName("HAS_START_FORM_KEY");
            builder.Property(c => c.SuspensionState).HasColumnName("SUSPENSION_STATE");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
            builder.Property(c => c.VersionTag).HasColumnName("VERSION_TAG");
            //builder.Ignore(c => c.HistoryLevel)
                //.Ignore(c=>c.IoMapping)
                //.Ignore(c=>c.StartFormHandler)
                //.Ignore(c => c.GraphicalNotationDefined)
                //.Ignore(c => c.ParticipantProcess)
                //.Ignore(c => c.LaneSets)
                //.Ignore(c => c.Activities)
                //.Ignore(c => c.IdentityLinks)
                //.Ignore(c => c.EventActivities)
                //.Ignore(c => c.InitialActivityStack)
                //.Ignore(c => c.ActivityBehavior)
                //.Ignore(c => c.Initial)
            //Ignore(c => c.IsScope);
            //.Ignore(c=>c..Instances)
            //.Ignore(c=>c.FailedJobs)
            //.Ignore(c => c.Discriminator)
            //.Ignore(c=>c.Properties)
            //.Ignore(c=>c.ProcessDefinition)
            //.Ignore(c=>c.PreviousDefinition)
            //.Ignore(c=>c.PreviousProcessDefinitionId)
            //;
        }
        
    }
}
