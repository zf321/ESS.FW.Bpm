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
    public class TenantMembershipMap : IEntityTypeConfiguration<TenantMembershipEntity>
    {
        public void Configure(EntityTypeBuilder<TenantMembershipEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_ID_TENANT_MEMBER");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("ID");
            builder.Property(c => c.UserId).HasColumnName("USER_ID");
            builder.Property(c => c.GroupId).HasColumnName("GROUP_ID");
            builder.Property(c => c.TenantId).HasColumnName("TENANT_ID");
        }
        
    }
}
