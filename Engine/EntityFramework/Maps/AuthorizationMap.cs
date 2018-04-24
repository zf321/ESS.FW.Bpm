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
    public class AuthorizationMap: IEntityTypeConfiguration<AuthorizationEntity>
    {
        public void Configure(EntityTypeBuilder<AuthorizationEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_RU_AUTHORIZATION");
            builder.HasKey(c => c.Id);
            builder.Property(r => r.Id).HasColumnName("ID");
            builder.Property(r => r.Revision).HasColumnName("REV");
            builder.Property(r => r.AuthorizationType).HasColumnName("TYPE");
            builder.Property(r => r.GroupId).HasColumnName("GROUP_ID");
            builder.Property(r => r.UserId).HasColumnName("USER_ID");
            builder.Property(r => r.ResourceType).HasColumnName("RESOURCE_TYPE");
            builder.Property(r => r.ResourceId).HasColumnName("RESOURCE_ID");
            builder.Property(r => r.Permissions).HasColumnName("PERMS");
        }
        
    }
}
