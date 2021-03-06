﻿using ESS.FW.Bpm.Engine.Persistence.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.EntityFramework.Maps
{
    public class IdentityLinkMap: IEntityTypeConfiguration<IdentityLinkEntity>
    {
        public void Configure(EntityTypeBuilder<IdentityLinkEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_RU_IDENTITYLINK");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Type).HasColumnName("TYPE");
            builder.Property(c => c.UserId).HasColumnName("USER_ID");
            builder.Property(c => c.GroupId).HasColumnName("GROUP_ID");
            builder.Property(c => c.TaskId).HasColumnName("TASK_ID");
            builder.Property(c => c.ProcessDefId).HasColumnName("PROC_DEF_ID");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");

            builder.Ignore(c => c.ProcessDef);
            //builder.HasRequired(c => c.ProcessDef).WithMany().HasForeignKey(c => c.ProcessDefId);
            //builder.HasRequired(c => c.Task).WithMany().HasForeignKey(c => c.TaskId);
        }
        
    }
}
