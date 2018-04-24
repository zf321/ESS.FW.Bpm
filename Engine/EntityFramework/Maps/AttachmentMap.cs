using ESS.FW.Bpm.Engine.Persistence.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESS.FW.Bpm.Engine.EntityFramework.Maps
{
    public class AttachmentMap: IEntityTypeConfiguration<AttachmentEntity>
    {
        //public AttachmentMap()
        //{
        //    builder.ToTable("TB_GOS_BPM_HI_ATTACHMENT");
        //    builder.HasKey(c => c.Id);
        //    builder.Property(c => c.Id).HasColumnName("ID");
        //    builder.Property(c => c.Revision).HasColumnName("REV");
        //    builder.Property(c => c.Name).HasColumnName("NAME");
        //    builder.Property(c => c.Description).HasColumnName("DESCRIPTION");
        //    builder.Property(c => c.Type).HasColumnName("TYPE");
        //    builder.Property(c => c.TaskId).HasColumnName("TASK_ID");
        //    builder.Property(c => c.ProcessInstanceId).HasColumnName("PROC_INST_ID");
        //    builder.Property(c => c.Url).HasColumnName("URL");
        //    builder.Property(c => c.ContentId).HasColumnName("CONTENT_ID");
        //    builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
        //}

        public void Configure(EntityTypeBuilder<AttachmentEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_HI_ATTACHMENT");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Revision).HasColumnName("REV");
            builder.Property(c => c.Name).HasColumnName("NAME");
            builder.Property(c => c.Description).HasColumnName("DESCRIPTION");
            builder.Property(c => c.Type).HasColumnName("TYPE");
            builder.Property(c => c.TaskId).HasColumnName("TASK_ID");
            builder.Property(c => c.ProcessInstanceId).HasColumnName("PROC_INST_ID");
            builder.Property(c => c.Url).HasColumnName("URL");
            builder.Property(c => c.ContentId).HasColumnName("CONTENT_ID");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
        }
    }
}
