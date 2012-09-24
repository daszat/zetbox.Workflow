
namespace zetbox.Workflow.Common.Workflow.CommonInvocations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using wf = Zetbox.Basic.Workflow;
    using Zetbox.App.Base;
    using Zetbox.API;

    public class Action
    {
        public static bool Schedule(wf.Action action, wf.ParameterizedActionDefinition parameter, wf.State current, Identity identity)
        {
            var ctx = current.Context;
            var sAction = (wf.SchedulerAction)action;
            var sParameter = (wf.ScheduledActionDefinition)parameter;

            DateTime dt = DateTime.Now;

            if (sParameter.EveryYear.HasValue)
            {
                dt = dt.FirstYearDay().AddYears(1);
            }
            else if (sParameter.EveryQuater.HasValue)
            {
                dt = dt.FirstQuaterDay().AddMonths(3);
            }
            else if (sParameter.EveryMonth.HasValue)
            {
                dt = dt.FirstMonthDay().AddMonths(1);
            }
            else if (sParameter.EveryDay.HasValue)
            {
                dt = dt.Date;
            }

            dt = dt
                .AddMonths(sParameter.Months ?? sAction.Months ?? 0)
                .AddDays(sParameter.Days ?? sAction.Days ?? 0)
                .AddHours(sParameter.Hours ?? sAction.Hours ?? 0)
                .AddMinutes(sParameter.Minutes ?? sAction.Minutes ?? 0);

            current.ScheduleAction(dt, sParameter.InvokeAction);

            return true;
        }
    }
}
