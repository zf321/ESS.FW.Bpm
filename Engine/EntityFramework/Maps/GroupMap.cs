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
    public class GroupMap : IEntityTypeConfiguration<GroupEntity>
    {
        public void Configure(EntityTypeBuilder<GroupEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_ID_GROUP");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Revision).HasColumnName("REV");
            builder.Property(c => c.Name).HasColumnName("NAME");
            builder.Property(c => c.Type).HasColumnName("TYPE");
        }
        
    }
}
