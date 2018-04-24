using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Engine.Tests.Bpmn.SubProcess.Transaction
{
    public class TestDbContext : DbContext
    {
        const string  connString = "User ID=syerp;Password=syerp;Data Source=127.0.0.1/orcl;";
        public TestDbContext() 
        {

        }
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    //base.OnModelCreating(modelBuilder);
        //    modelBuilder.HasDefaultSchema("SYERP");
        //}
        public virtual DbSet<ProcessDefTestEntity> ProcessDefs { get; set; }
    }
    [Table("TB_GOS_BPM_RE_PROCDEF")]
    public class ProcessDefTestEntity
    {
        [Key]
        [Column("ID")]
        public string Id { get; set; }
        [Column("KEY")]
        public string Key { get; set; } = "TransationTest";
        [Column("VERSION")]
        public int Version { get; set; } = 1;
    }
}
