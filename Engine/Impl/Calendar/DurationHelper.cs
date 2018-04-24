



using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ESS.FW.Bpm.Engine.Impl.Util;

namespace ESS.FW.Bpm.Engine.Impl.Calendar
{
    /// <summary>
    ///     helper class for parsing ISO8601 duration format(also recurring) and computing next timer date
    /// </summary>
    public class DurationHelper
    {
        private static readonly EngineUtilLogger Log = ProcessEngineLogger.UtilLogger;

        //internal DatatypeFactory datatypeFactory;
        /// <summary>
        /// 结束时间
        /// </summary>
        internal DateTime? end;
        /// <summary>
        /// 是否循环 R
        /// </summary>
        internal bool isRepeat;
        /// <summary>
        /// 循环间隔?
        /// </summary>
        internal DurationVisitor period;
        /// <summary>
        /// 开始时间
        /// </summary>
        internal DateTime? start;
        /// <summary>
        /// 循环次数
        /// </summary>
        internal int times = 0;

        public DurationHelper(string expressions)
        {
            IList<string> expression = new List<string>();
            if (expressions != null)
            {
                expression = (expressions.Split('/'));
            }
            //datatypeFactory = DatatypeFactory.newInstance();

            if (expression.Count > 3 || expression.Count == 0)
            {
                throw Log.CannotParseDuration(expressions);
            }
            if (expression[0].StartsWith("R"))
            {
                isRepeat = true;
                times = expression[0].Length == 1 ? int.MaxValue : int.Parse(expression[0].Substring(1));
                expression = expression.Skip(1).ToList();
            }

            if (IsDuration(expression[0]))
            {
                period = DurationVisitor.Parse(expression[0]);
                end = expression.Count == 1 ? (DateTime?)null : DateTime.Parse(expression[1]);
            }
            else
            {
                start = DateTime.Parse(expression[0]);
                if (IsDuration(expression[1]))
                {
                    period = DurationVisitor.Parse(expression[0]);
                }
                else
                {
                    end = DateTime.Parse(expression[1]);
                    period =DurationVisitor.FromTimeSpan(((DateTime)end) - ((DateTime)start));// datatypeFactory.newDuration(end.GetValueOrDefault().Ticks - start.Ticks);
                }
            }
            if (start == null && end == null)
            {
                start = ClockUtil.CurrentTime;
            }
        }

        public virtual DateTime? DateAfter
        {
            get
            {
                if (isRepeat)
                {
                    return GetDateAfterRepeat(ClockUtil.CurrentTime);
                }
                //TODO: is this correct?
                if (end != null)
                {
                    return end.Value;
                }
                return Add(start, period.GetTimeSpan());
            }
        }

        public virtual int Times
        {
            get { return times; }
        }

        private DateTime? GetDateAfterRepeat(DateTime date)
        {
            //过滤最大循环数
            if (times == int.MaxValue)
            {
                return DateTime.MaxValue;
            }

            if (start != null)
            {
                var cr = start;
                //R/PT1H 循环次数maxint 超出时间最大值
                for (var i = 0; i < times && !(cr > date); i++)
                {
                    cr = Add(cr, period.GetTimeSpan());
                }
                return cr < date ? (DateTime?)null : cr;
            }
            var cur =Add(end.Value, period.Negate().GetTimeSpan());
            var next = end;

            for (var i = 0; i < times && cur > date; i++)
            {
                next = cur;
                cur = Add(cur, period.Negate().GetTimeSpan());
            }
            return next < date ? (DateTime?)null : next;
        }

        private DateTime Add(DateTime? date, TimeSpan duration)
        {
            if (date == null)
            {
                throw new NullReferenceException("date can not be null");
            }
            return ((DateTime)date).Add(duration);
        }
        /// <summary>
        /// 实现JAVA com.sun.org.apache.xerces.internal.util.DatatypeMessageFormatter
        /// 
        /// </summary>
        /// <param name="period"></param>
        /// <returns></returns>
        private TimeSpan ParsePeriod(string period)
        {
            string d = "0";
            string h = "0";
            string m = "0";
            string s = "0";
            bool isFu = false;
            string preDateTime = string.Empty;
            if (period.StartsWith("P"))
            {
                preDateTime = period.Substring(1, period.Length - 1);
            }
            else if (period.StartsWith("-P"))
            {
                isFu = true;
                preDateTime = period.Substring(2, period.Length - 2);
            }
            if (preDateTime.IndexOf("D") > 0)//1D
            {
                d = preDateTime.Split('D')[0];
            }
            if (preDateTime.IndexOf("T") > -1)//代表后面的是时间。5H10M...
            {
                string preTime = preDateTime.Split('T')[1];
                if (preTime.IndexOf("H") > 0) //12H
                {
                    h = preTime.Split('H')[0];
                    preTime = preTime.Split('H')[1];
                }
                if (preTime.IndexOf("M") > 0)//10M
                {
                    m = preTime.Split('M')[0];
                    preTime = preTime.Split('M')[1];
                }
                if (preTime.IndexOf("S") > 0)
                {
                    s = preTime.Split('S')[0];
                }
            }
            string result = string.Format("{0}{1}.{2}:{3}:{4}", isFu ? "-" : "", d, h, m, s);
            return TimeSpan.Parse(result);
            //return datatypeFactory.newDuration(period);
        }

        private bool IsDuration(string time)
        {
            return time.StartsWith("P");
        }
    }
}

