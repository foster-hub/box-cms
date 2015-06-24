using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box.CMS.Services;

namespace Box.CMS.Extensions {
    public static class PeriodExtension {

        public static DateTime? StartDate(this Enum e) {
            return StartDate(e, DateTime.Now);
        }

        public static DateTime? StartDate(this Enum e, DateTime? to) {

            if (to == null)
                return null;

            Periods period = (Box.CMS.Periods)e;

            switch (period) {
            case Periods.LastHour:
            return to.Value.AddDays(-1);
            case Periods.LastDay:
            return to.Value.AddDays(-1);
            case Periods.Last5Days:
            return to.Value.AddDays(-5);
            case Periods.Last30Days:
            return to.Value.AddDays(-30);
            case Periods.Last150Days:
            return to.Value.AddDays(-150);
            case Periods.Last360Days:
            return to.Value.AddDays(-360);

            }
            return null;
        }

    }
}
