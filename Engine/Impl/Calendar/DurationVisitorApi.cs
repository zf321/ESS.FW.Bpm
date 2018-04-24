using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESS.FW.Bpm.Engine.Impl.Calendar
{
    public partial class DurationVisitor
    {
        public static DurationVisitor Parse(string duration)
        {
            var visitor = new DurationVisitor(duration);
            visitor.Visit();
            if (visitor.isValid)
            {
                return visitor;
            }
            throw new FormatException($"无法解析duration：{duration}");
        }
		public static DurationVisitor FromTimeSpan(TimeSpan timespan)
        {
            return new DurationVisitor(timespan);
        }
		public  DurationVisitor Negate()
        {
            var result = new DurationVisitor();
            result.days = -this.Days;
            result.hours = -this.Hours;
            result.inTimeSection = this.inTimeSection;
            result.minutes = -this.Minutes;
            result.seconds = -this.Seconds;
            return result;
        }
        public TimeSpan GetTimeSpan()
        {
            return TimeSpan.Zero
               .Add(TimeSpan.FromDays(this.Weeks * 7))
               .Add(TimeSpan.FromDays(this.Days))
               .Add(TimeSpan.FromHours(this.Hours))
               .Add(TimeSpan.FromMinutes(this.Minutes))
               .Add(TimeSpan.FromSeconds(this.Seconds));
        }

        public bool Valid => this.isValid;

        public double Weeks => this.weeks;

        public double Days => this.days;

        public double Hours => this.hours;

        public double Minutes => this.minutes;

        public double Seconds => this.seconds;
        private static TimeSpan ConvertResult(DurationVisitor visitor)
        {
            return TimeSpan.Zero
                .Add(TimeSpan.FromDays(visitor.Weeks * 7))
                .Add(TimeSpan.FromDays(visitor.Days))
                .Add(TimeSpan.FromHours(visitor.Hours))
                .Add(TimeSpan.FromMinutes(visitor.Minutes))
                .Add(TimeSpan.FromSeconds(visitor.Seconds));
        }
    }
}
