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
    public class UserMap : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_ID_USER");
            builder.HasKey(m => m.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.Revision).HasColumnName("REV");
            builder.Property(c => c.FirstName).HasColumnName("FIRST");
            builder.Property(c => c.LastName).HasColumnName("LAST");
            builder.Property(c => c.Email).HasColumnName("EMAIL");
            builder.Property(c => c.DbPassword).HasColumnName("PWD");

            builder.Ignore(c => c.Password);
        }
        
    }
}
