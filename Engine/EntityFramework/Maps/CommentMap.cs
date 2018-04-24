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
    public class CommentMap: IEntityTypeConfiguration<CommentEntity>
    {
        public void Configure(EntityTypeBuilder<CommentEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_HI_COMMENT");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Type).HasColumnName("TYPE");
            builder.Property(c => c.UserId).HasColumnName("USER_ID");
            builder.Property(c => c.Time).HasColumnName("TIME");
            builder.Property(c => c.TaskId).HasColumnName("TASK_ID");
            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROC_INST_ID");
            builder.Property(c => c.Action).HasColumnName("ACTION");
            builder.Property(c => c.Message).HasColumnName("MESSAGE");
            builder.Property(c => c.FullMessageBytes).HasColumnName("FULL_MSG");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
        }
        
    }
}
