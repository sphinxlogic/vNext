// ==++==
// 
//   
//    Copyright (c) 2006 Microsoft Corporation.  All rights reserved.
//   
//    The use and distribution terms for this software are contained in the file
//    named license.txt, which can be found in the root of this distribution.
//    By using this software in any fashion, you are agreeing to be bound by the
//    terms of this license.
//   
//    You must not remove this notice, or any other, from this software.
//   
// 
// ==--==
/*============================================================
**
** Class: TimeZone
**
**
** Purpose: 
** This class is used to represent a TimeZone.  It
** has methods for converting a DateTime to UTC from local time
** and to local time from UTC and methods for getting the 
** standard name and daylight name of the time zone.  
**
** The only TimeZone that we support in version 1 is the 
** CurrentTimeZone as determined by the system timezone.
**
**
============================================================*/
namespace System {
    using System;
    using System.Text;
    using System.Threading;
    using System.Collections;
    using System.Globalization;

    [Serializable]
[System.Runtime.InteropServices.ComVisible(true)]
    public abstract class TimeZone {
        private static TimeZone currentTimeZone = null;

        // Private object for locking instead of locking on a public type for SQL reliability work.
        private static Object s_InternalSyncObject;
        private static Object InternalSyncObject {
            get {
                if (s_InternalSyncObject == null) {
                    Object o = new Object();
                    Interlocked.CompareExchange(ref s_InternalSyncObject, o, null);
                }
                return s_InternalSyncObject;
            }
        }


        protected TimeZone() {
        }
    
        public static TimeZone CurrentTimeZone {
            get {
                //Grabbing the cached value is required at the top of this function so that
                //we don't incur a race condition with the ResetTimeZone method below.
                TimeZone tz = currentTimeZone;
                if (tz == null) {
                    lock(InternalSyncObject) {
                        if (currentTimeZone == null) {
                            currentTimeZone = new CurrentSystemTimeZone();
                        }
                        tz = currentTimeZone;
                    }
                }
                return (tz);
            }
        }

        //This method is called by CultureInfo.ClearCachedData in response to control panel
        //change events.  It must be synchronized because otherwise there is a race condition 
        //with the CurrentTimeZone property above.
        internal static void ResetTimeZone() {
            if (currentTimeZone!=null) {
                lock(InternalSyncObject) {
                    currentTimeZone = null;
                }
            }
        }
    
        public abstract String StandardName {
            get;
        }
    
        public abstract String DaylightName {
            get;
        }

        public abstract TimeSpan GetUtcOffset(DateTime time);

        //
        // Converts the specified datatime to the Universal time base on the current timezone 
        //
        public virtual DateTime ToUniversalTime(DateTime time) {
            if (time.Kind == DateTimeKind.Utc) {
                return time;
            }
            long tickCount = time.Ticks - GetUtcOffset(time).Ticks;
            if (tickCount>DateTime.MaxTicks) {
                return new DateTime(DateTime.MaxTicks, DateTimeKind.Utc);
            }
            if (tickCount<DateTime.MinTicks) {
                return new DateTime(DateTime.MinTicks, DateTimeKind.Utc);
            }
            return new DateTime(tickCount, DateTimeKind.Utc);
        }

        //
        // Convert the specified datetime value from UTC to the local time based on the time zone.
        //
        public virtual DateTime ToLocalTime(DateTime time) {
            if (time.Kind == DateTimeKind.Local) {
                return time;
            }
            Boolean isAmbiguousLocalDst = false;
            Int64 offset = ((CurrentSystemTimeZone)(TimeZone.CurrentTimeZone)).GetUtcOffsetFromUniversalTime(time, ref isAmbiguousLocalDst);
            return new DateTime(time.Ticks + offset, DateTimeKind.Local, isAmbiguousLocalDst);
        }
       
        // Return an array of DaylightTime which reflects the daylight saving periods in a particular year.
        // We currently only support having one DaylightSavingTime per year.
        // If daylight saving time is not used in this timezone, null will be returned.
        public abstract DaylightTime GetDaylightChanges(int year);

        public virtual bool IsDaylightSavingTime(DateTime time) {
            return (IsDaylightSavingTime(time, GetDaylightChanges(time.Year)));
        }
  
        // Check if the specified time is in a daylight saving time.  Allows the user to
        // specify the array of Daylight Saving Times.
        public static bool IsDaylightSavingTime(DateTime time, DaylightTime daylightTimes) {
            return CalculateUtcOffset(time, daylightTimes)!=TimeSpan.Zero;
        }
      
        //      
        //
        //      
        
        //
        //      
        //
        //
        //      
        internal static TimeSpan CalculateUtcOffset(DateTime time, DaylightTime daylightTimes) {
            if (daylightTimes==null) {
                return TimeSpan.Zero;
            }
            DateTimeKind kind = time.Kind;
            if (kind == DateTimeKind.Utc) {
                return TimeSpan.Zero;
            }

            DateTime startTime;
            DateTime endTime;

            // startTime and endTime represent the period from either the start of DST to the end and includes the 
            // potentially overlapped times
            startTime = daylightTimes.Start + daylightTimes.Delta;
            endTime = daylightTimes.End;
            
            // For normal time zones, the ambiguous hour is the last hour of daylight saving when you wind the 
            // clock back. It is theoretically possible to have a positive delta, (which would really be daylight
            // reduction time), where you would have to wind the clock back in the begnning.
            DateTime ambiguousStart;
            DateTime ambiguousEnd;            
            if (daylightTimes.Delta.Ticks > 0) {
                ambiguousStart = endTime - daylightTimes.Delta;
                ambiguousEnd = endTime;
            } else {
                ambiguousStart = startTime;
                ambiguousEnd = startTime - daylightTimes.Delta;
            }

            Boolean isDst = false;
            if (startTime > endTime) {
                // In southern hemisphere, the daylight saving time starts later in the year, and ends in the beginning of next year.
                // Note, the summer in the southern hemisphere begins late in the year.
                if (time >= startTime || time < endTime) {
                    isDst = true;
                }
            }
            else if (time>=startTime && time < endTime) {
                // In northern hemisphere, the daylight saving time starts in the middle of the year.
                isDst = true;
            }
            
            // If this date was previously converted from a UTC date and we were able to detect that the local
            // DateTime would be ambiguous, this data is stored in the DateTime to resolve this ambiguity. 
            if (isDst && time >= ambiguousStart && time < ambiguousEnd) {
                isDst = time.IsAmbiguousDaylightSavingTime();
            }
           
            if (isDst) {
                return daylightTimes.Delta;
            }
            return TimeSpan.Zero;
        }
    }
}

