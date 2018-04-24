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
    public class MembershipMap : IEntityTypeConfiguration<MembershipEntity>
    {
        public void Configure(EntityTypeBuilder<MembershipEntity> builder)
        {
            builder.ToTable("TB_GOS_BPM_ID_MEMBERSHIP");
            builder.HasKey(c => new { c.UserId, c.GroupId });
            builder.Property(c => c.UserId).HasColumnName("USER_ID");
            builder.Property(c => c.GroupId).HasColumnName("GROUP_ID");
            builder.Ignore(c => c.Id);
        }
        
    }
}
