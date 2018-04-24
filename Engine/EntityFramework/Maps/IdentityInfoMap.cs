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
    public class IdentityInfoMap : IEntityTypeConfiguration<IdentityInfoEntity>
    {
        public void Configure(EntityTypeBuilder<IdentityInfoEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_ID_INFO");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Revision).HasColumnName("REV");
            builder.Property(c => c.UserId).HasColumnName("USER_ID");
            builder.Property(c => c.Type).HasColumnName("TYPE");
            builder.Property(c => c.Key).HasColumnName("KEY");
            builder.Property(c => c.Value).HasColumnName("VALUE");
            builder.Property(c => c.PasswordBytes).HasColumnName("PASSWORD");
            builder.Property(c => c.ParentId).HasColumnName("PARENT_ID");
        }
        
    }
}
