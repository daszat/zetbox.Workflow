﻿
namespace zetbox.Workflow.Common.Workflow.CommonInvocations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using wf = Zetbox.Basic.Workflow;
    using Zetbox.App.Base;
    using Zetbox.API;
    using System.Net.Mail;

    public class Action
    {
        public readonly IMailSender _mail;
        public Action(IMailSender mail)
        {
            _mail = mail;
        }

        public bool SendReminder(wf.Action action, wf.ParameterizedActionDefinition parameter, wf.State current, Identity identity)
        {
            var body = string.Format(@"Hello Arthur!

This is an reminder of an open workflow item in your inbox.

Best regards, your Workflow System");
            var msg = new MailMessage("office@dasz.at", "arthur@dasz.at", "Reminder", body);
            _mail.Send(msg);
            return true;
        }

        public static bool Schedule(wf.Action action, wf.ParameterizedActionDefinition parameter, wf.State current, Identity identity)
        {
            var ctx = current.Context;
            var sAction = (wf.SchedulerAction)action;
            var sParameter = (wf.ScheduledActionDefinition)parameter;

            DateTime dt = sAction.Schedule.GetNext();
            dt = sParameter.Schedule.GetNext(dt);

            current.ScheduleAction(dt, sParameter.InvokeAction);

            return true;
        }

        public static bool AddResponsibles(wf.Action action, wf.ParameterizedActionDefinition parameter, wf.State current, Identity identity)
        {
            var ctx = current.Context;
            var sParameter = (wf.AddResponsiblesActionDefinition)parameter;

            if (sParameter.ClearResponsibles)
            {
                current.Groups.Clear();
                current.Persons.Clear();
            }

            foreach (var grp in sParameter.Groups)
            {
                current.Groups.Add(grp);
            }

            foreach (var person in sParameter.Persons)
            {
                current.Persons.Add(person);
            }

            return true;
        }
    }
}
