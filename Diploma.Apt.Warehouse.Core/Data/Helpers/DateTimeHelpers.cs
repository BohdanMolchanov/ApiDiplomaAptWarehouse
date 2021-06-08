using System;

namespace EShop.Data.Helpers
{
    public static class DateTimeHelpers
    {
        #region Truncate
        public static DateTime Truncate(this DateTime date, TruncateTo truncateTo)
            => new DateTime(date.Ticks - date.Ticks % (long)truncateTo, date.Kind == DateTimeKind.Unspecified 
                ? DateTimeKind.Local 
                : date.Kind);

        public static DateTime? Truncate(this DateTime? date, TruncateTo truncateTo)
            => date?.Truncate(truncateTo);
        
        #endregion

        #region EnsureUniversalTime

        public static DateTime EnsureUniversalTime(this DateTime value)
            => value.Kind == DateTimeKind.Local
                ? value.ToUniversalTime()
                : DateTime.SpecifyKind(value, DateTimeKind.Utc);
            
        public static DateTime? EnsureUniversalTime(this DateTime? value)
            => value?.EnsureUniversalTime();

        #endregion
    }
}