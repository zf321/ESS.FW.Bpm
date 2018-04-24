using System;

namespace ESS.FW.Bpm.Engine.Impl.Util
{
    /// <summary>
    ///     
    /// </summary>
    public class ClockUtil
    {
        private  static DateTime? _currentTime = null;
        public static DateTime CurrentTime
        {
            set {
                _currentTime = value; }
            get
            {
                if (_currentTime != null)
                    return _currentTime.Value;
                return DateTime.Now;
            }
        }

        public static void Reset()
        {
            _currentTime = null;
        }
    }
}