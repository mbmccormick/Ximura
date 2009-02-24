#region Copyright
// *******************************************************************************
// Copyright (c) 2000-2009 Paul Stancer.
// All rights reserved. This program and the accompanying materials
// are made available under the terms of the Eclipse Public License v1.0
// which accompanies this distribution, and is available at
// http://www.eclipse.org/legal/epl-v10.html
//
// Contributors:
//     Paul Stancer - initial implementation
// *******************************************************************************
#endregion
#region using
using System;
using System.Collections;
using System.Xml;
using System.Configuration;

using CH=Ximura.Helper.Common;
#endregion // using
namespace Ximura.Command
{
    ///// <summary>
    ///// Summary description for AppCommandScheduleProcessConfigSH.
    ///// </summary>
    //public class AppCommandScheduleProcessConfigSH : AppCommandProcessConfigSH, IXimuraCommandScheduleProcessConfigSH
    //{
    //    #region Declarations
    //    ArrayList arrSchedule = new ArrayList();
    //    #endregion
    //    #region Constructors/Destructor
    //    /// <summary>
    //    /// Constructor of AppCommandScheduleProcessConfigSH
    //    /// </summary>
    //    public AppCommandScheduleProcessConfigSH()
    //    {
    //        //
    //        // TODO: Add constructor logic here
    //        //
    //    }
    //    #endregion
	
    //    #region IXimuraCommandScheduleProcessConfigSH Members
    //    #region IConfigurationSectionHandler Members

    //    /// <summary>
    //    /// Create Config Object
    //    /// </summary>
    //    /// <param name="parent"></param>
    //    /// <param name="configContext"></param>
    //    /// <param name="section"></param>
    //    /// <returns></returns>
    //    public override object Create(object parent, object configContext, System.Xml.XmlNode section)
    //    {
    //        // TODO:  Add SessionConfigSectionHandler.Create implementation

    //        // Create Setting Collection
    //        // get the settings node
    //        base.Create(parent, configContext, section);

    //        // Create Schedule Collection
    //        // get all the schedules
    //        XmlNodeList nodelistSchedules = section.SelectNodes("./schedules/add[@enable='true']");
    //        // loop thru all schedules to get the schedule settings
    //        foreach(XmlNode currNodeSchedule in nodelistSchedules)
    //        {
    //            // get schedule settings from the current schedule
    //            Hashtable htSchedule = base.AddSettings(currNodeSchedule);
    //            // add the settings to the schedule collection
    //            this.arrSchedule.Add(htSchedule);

    //        }

    //        return this;
    //    }

    //    #endregion

    //    /// <summary>
    //    /// Get Next Schedules within interval(in milliseconds)
    //    /// </summary>
    //    /// <param name="interval">interval in milliseconds</param>
    //    /// <returns>return all upcoming schedules in time/schedule pairs</returns>
    //    public ArrayList GetNextSchedules(double interval)
    //    {
    //        // TODO:  Add AppCommandScheduleProcessConfigSH.NextSchedule implementation
    //        ArrayList arrNextSchedules = new ArrayList();

    //        // set up the interval time
    //        DateTime dtIntervalStart = DateTime.Now;
    //        DateTime dtIntervalEnd = dtIntervalStart.AddMilliseconds(interval);

    //        // loop thru all schedules and calculate the schedule time
    //        foreach (Hashtable htSchedule in this.arrSchedule)
    //        {
    //            DateTime scheduleDateTime = CalculateNextSchedule(htSchedule, dtIntervalStart, dtIntervalEnd);
    //            if (DateTime.Compare(scheduleDateTime, DateTime.MinValue) > 0)
    //            {
    //                arrNextSchedules.Add(htSchedule);
    //            }
    //        }
    //        return arrNextSchedules;
    //    }
    //    #endregion

    //    #region private helper method
    //    /// <summary>
    //    /// Calculate the next schedule time
    //    /// </summary>
    //    /// <param name="htSchedule">the schedule settings</param>
    //    /// <param name="dtIntervalStart">the start of interval</param>
    //    /// <param name="dtIntervalEnd">the end of interval</param>
    //    /// <returns>the next scheduled datetime</returns>
    //    private DateTime CalculateNextSchedule(Hashtable htSchedule, DateTime dtIntervalStart, DateTime dtIntervalEnd)
    //    {
    //        //Schedules Attributes:
    //        //		name - the name of the schedule
    //        //		type="[daily|weekly|monthly|once]" - the type of the schedule
    //        //		priority="[number]" - the priority of the schedule, "1" means highest
    //        //	the following attributes are used by Daily schedule:
    //        //		everyDays="[number]" - run every [number] days
    //        //	the following attributes are used by Weekly schedule:
    //        //		everyWeeks="[number]" - run every [number] weeks
    //        //	the following attributes are shared by Weekly and Monthly schedule:
    //        //		monday="[[none|every]|[none|first|second|third|fourth|last]]" - run on X monday
    //        //		tuesday="[[none|every]|[none|first|second|third|fourth|last]]" - run on X tuesday
    //        //		wednesday="[[none|every]|[none|first|second|third|fourth|last]]" - run on X wednesday
    //        //		thursday="[[none|every]|[none|first|second|third|fourth|last]]" - run on X thursday
    //        //		friday="[[none|every]|[none|first|second|third|fourth|last]]" - run on X friday
    //        //		saturday="[[none|every]|[none|first|second|third|fourth|last]]" - run on X saturday
    //        //		sunday="[[none|every]|[none|first|second|third|fourth|last]]" - run on X sunday
    //        //	the following attributes are used by Monthly schedule:
    //        //		day="[1]|[2]|[3]...[31]" - Day of the Month(s), can be multiple
    //        //		month="[1]|[2]|[3]...[12]" - Month, can be multiple
    //        //	the following attributes are Advanced options:
    //        //		startDate="[yyyy-MM-dd]" - Start Date of the task
    //        //		startTime="[hh:mm:ss]" - Start Time of the task 
    //        //		endDate="[yyyy-MM-dd]" - End Date of the task
    //        //		endTime="[hh:mm:ss]" - End Time of the task 
    //        //		everyHours="[hours]" - repeat every X hours
    //        //		everyMinutes="[minutes]" - repeat every X minutes
    //        //		duration="[minutes]" - Duration of the task

    //        #region get initial settings
    //        // get start date and time
    //        string strStartDate = (string)htSchedule["startDate"];
    //        string strStartTime = (string)htSchedule["startTime"];
    //        // get end date and time
    //        string strEndDate = (string)htSchedule["endDate"];
    //        string strEndTime = (string)htSchedule["endTime"];

    //        // convert start and end date time string to date time object
    //        DateTime startDateTime = Convert.ToDateTime(strStartDate + " " + strStartTime);
    //        DateTime endDateTime;
    //        try
    //        {
    //            endDateTime = Convert.ToDateTime(strEndDate + " " + strEndTime);
    //        }
    //        catch (Exception)
    //        {
    //            // if nothing set, set it to maxvalue
    //            endDateTime = DateTime.MaxValue;
    //        }

    //        DateTime nextScheduledDateTime = startDateTime;
    //        DateTime prevScheduledDateTime = nextScheduledDateTime;
    //        #endregion

    //        bool scheduleFound = false;
			
    //        // check any next schedule that within the interval
    //        scheduleFound = CheckNextSchedule(htSchedule, 
    //            dtIntervalStart, dtIntervalEnd,
    //            ref prevScheduledDateTime, ref nextScheduledDateTime);

    //        // if schedule not found check previous schedule
    //        if (!scheduleFound)
    //        {
    //            // check any previous schedule that within the interval
    //            scheduleFound = CheckPreviousSchedule(htSchedule, 
    //                dtIntervalStart, dtIntervalEnd,
    //                ref prevScheduledDateTime, ref nextScheduledDateTime);
    //        }

    //        // if schedule was found, check whether the scheduled time already passed the end date time
    //        if (scheduleFound
    //            && DateTime.Compare(nextScheduledDateTime, endDateTime) < 0)
    //        {
    //            // return the schedule time if it is still not expire
    //            return nextScheduledDateTime;
    //        }
    //        // return minvalue means the schedule not found (cannot return null date time)
    //        return DateTime.MinValue;
    //    }

    //    private bool CheckNextSchedule(Hashtable htSchedule, 
    //        DateTime dtIntervalStart, DateTime dtIntervalEnd, 
    //        ref DateTime prevScheduledDateTime, ref DateTime nextScheduledDateTime)
    //    {
    //        bool scheduleFound = false;
    //        // get type
    //        string type = (string)htSchedule["type"];
    //        switch (type)
    //        {
    //            case "once":
    //                #region Run Once Schedule
    //                // this is the schedule
    //                scheduleFound = true;
    //                #endregion
    //                break;
    //            case "daily":
    //                #region Daily Schedule

    //                // ---------------------------------------------------------------
    //                // get the schedule that start right after the start of interval
    //                // ---------------------------------------------------------------

    //                // get every X days setting
    //                double everyDays = 0;
    //                try
    //                {
    //                    everyDays = double.Parse((string)htSchedule["everyDays"]);
    //                }
    //                catch (Exception)
    //                {
    //                    //do nothing, just lazy to check the validity
    //                }

    //                if (everyDays == 0)
    //                {
    //                    // this should not happen
    //                    break;
    //                }

    //                // find the next scheduled date time after the start of interval
    //                while (DateTime.Compare(nextScheduledDateTime, dtIntervalStart) < 0)
    //                {
    //                    prevScheduledDateTime = nextScheduledDateTime;
    //                    // add everyDays to nextScheduledDateTime
    //                    nextScheduledDateTime = nextScheduledDateTime.AddDays(everyDays);
    //                }

    //                // this is the schedule
    //                scheduleFound = true;

    //                #endregion
    //                break;
    //            case "weekly":
    //                #region Weekly Schedule
				
    //                // ---------------------------------------------------------------
    //                // get the schedule that start right after the start of interval
    //                // ---------------------------------------------------------------

    //                // get every X weeks setting
    //                double everyWeeks = 0;
    //                try
    //                {
    //                    everyWeeks = double.Parse((string)htSchedule["everyWeeks"]);
    //                }
    //                catch (Exception)
    //                {
    //                    //do nothing, just lazy to check the validity
    //                }

    //                if (everyWeeks == 0)
    //                {
    //                    // this should not happen
    //                    break;
    //                }

    //                // find the next scheduled date time after the start of interval
    //                while (DateTime.Compare(nextScheduledDateTime, dtIntervalStart) < 0)
    //                {
    //                    prevScheduledDateTime = nextScheduledDateTime;
    //                    // add everyWeeks to nextScheduledDateTime
    //                    nextScheduledDateTime = nextScheduledDateTime.AddDays(everyWeeks * 7);
    //                }

    //                // find the next schedule date time from Day Of Week setting
    //                for (int i=0; i<7; i++)
    //                {
    //                    string dayOfWeek = nextScheduledDateTime.DayOfWeek.ToString().ToLower();
    //                    if ((string)htSchedule[dayOfWeek] == "every")
    //                    {
    //                        // this is the day
    //                        scheduleFound = true;
    //                        break;
    //                    }
    //                    else
    //                    {
    //                        // increment by 1 to next iteration
    //                        nextScheduledDateTime = nextScheduledDateTime.AddDays(1);
    //                    }
    //                }

    //                #endregion
    //                break;
    //            case "monthly":
    //                #region Monthly Schedule
    //                // find the next schedule date time
    //                for (int i=0; i<366; i++)
    //                {
    //                    // check whether the schedule match the monthly settings
    //                    if (MatchMonthlySchedule(htSchedule, nextScheduledDateTime))
    //                    {
    //                        if (DateTime.Compare(nextScheduledDateTime, dtIntervalStart) >= 0)
    //                        {
    //                            // this is the schedule
    //                            scheduleFound = true;
    //                            break;

    //                        }
    //                        else
    //                        {
    //                            // mark this as previous schedule date
    //                            prevScheduledDateTime = nextScheduledDateTime;
    //                        }
    //                    }

    //                    // increment by 1 to next iteration
    //                    nextScheduledDateTime = nextScheduledDateTime.AddDays(1);
    //                }
    //                #endregion
    //                break;
    //            default:
    //                break;
    //        }

    //        // check whether the schedule time fall into the interval range
    //        return (scheduleFound
    //            && DateTime.Compare(nextScheduledDateTime, dtIntervalStart) >= 0 
    //            && DateTime.Compare(nextScheduledDateTime, dtIntervalEnd) <= 0);
    //    }

    //    /// <summary>
    //    /// Check the previous schedule whether it is within the interval
    //    /// </summary>
    //    /// <param name="htSchedule">the schedule settings</param>
    //    /// <param name="dtIntervalStart">the start of interval</param>
    //    /// <param name="dtIntervalEnd">the end of interval</param>
    //    /// <param name="prevScheduledDateTime">previous schedule</param>
    //    /// <param name="nextScheduledDateTime">schedule that within the interval</param>
    //    /// <returns>boolean</returns>
    //    private bool CheckPreviousSchedule(Hashtable htSchedule, 
    //        DateTime dtIntervalStart, DateTime dtIntervalEnd, 
    //        ref DateTime prevScheduledDateTime, ref DateTime nextScheduledDateTime)
    //    {
    //        #region get initial settings
    //        double everyHours = 0;
    //        try
    //        {
    //            everyHours = double.Parse((string)htSchedule["everyHours"]);
    //        }
    //        catch (Exception)
    //        {
    //            //do nothing, just lazy to check the validity
    //        }
    //        double everyMinutes = 0;
    //        try
    //        {
    //            everyMinutes = double.Parse((string)htSchedule["everyMinutes"]);
    //        }
    //        catch (Exception)
    //        {
    //            //do nothing, just lazy to check the validity
    //        }
    //        everyMinutes += (everyHours * 60);
    //        double duration = 0;
    //        try
    //        {
    //            duration = double.Parse((string)htSchedule["duration"]);
    //        }
    //        catch (Exception)
    //        {
    //            //do nothing, just lazy to check the validity
    //        }
    //        // get start and end time
    //        string strStartTime = (string)htSchedule["startTime"];
    //        string strEndTime = (string)htSchedule["endTime"];
    //        // if no duration setting, calculate it based on Start Time and End Time
    //        if (duration == 0 && strEndTime != "")
    //        {
    //            DateTime dtStartTime = Convert.ToDateTime(strStartTime);
    //            DateTime dtEndTime = Convert.ToDateTime(strEndTime);
				
    //            if (DateTime.Compare(dtStartTime, dtEndTime) > 0)
    //            {
    //                // make end time a future time than start time
    //                dtEndTime = dtEndTime.AddDays(1);
    //            }
    //            TimeSpan tsDiff =  dtEndTime - dtStartTime;
    //            duration = tsDiff.TotalMinutes;
    //        }
    //        #endregion

    //        // ---------------------------------------------------------------
    //        // since the schedule passed the end of interval
    //        // check previous schedule using the every X hours, Y minutes and duration settings
    //        // to get the schedule that is within the interval
    //        // ---------------------------------------------------------------

    //        // apply the everyHours, everyMinutes and duration settings on prevScheduleDateTime
    //        if (everyMinutes == 0 || duration == 0)
    //        {
    //            // no everyHours, everyMinutes or duration settings
    //            return false;
    //        }
					
    //        // set nextScheduledDateTime to last iteration
    //        nextScheduledDateTime = prevScheduledDateTime;

    //        bool scheduleFound = false;
    //        // get type
    //        string type = (string)htSchedule["type"];
    //        switch (type)
    //        {
    //            case "once":
    //            case "daily":
    //                #region Run Once and Daily Schedule
    //                // call the check daily frequency to see whether there is a schedule match
    //                scheduleFound = CheckScheduleDailyFrequency(dtIntervalStart, 
    //                    prevScheduledDateTime, ref nextScheduledDateTime,
    //                    everyMinutes, duration);
    //                #endregion
    //                break;
    //            case "weekly":
    //                #region Weekly Schedule
    //                // find the next schedule date time from Day Of Week setting
    //                for (int i=0; i<7; i++)
    //                {
    //                    string dayOfWeek = nextScheduledDateTime.DayOfWeek.ToString().ToLower();
    //                    if ((string)htSchedule[dayOfWeek] == "every")
    //                    {
    //                        // call the check daily frequency to see whether there is a schedule match
    //                        scheduleFound = CheckScheduleDailyFrequency(dtIntervalStart, 
    //                            prevScheduledDateTime, ref nextScheduledDateTime,
    //                            everyMinutes, duration);
    //                    }

    //                    // if schedule was found, break
    //                    if (scheduleFound) break;

    //                    prevScheduledDateTime = nextScheduledDateTime;
    //                    // increment by 1 to next iteration
    //                    nextScheduledDateTime = nextScheduledDateTime.AddDays(1);
    //                }
    //                #endregion
    //                break;
    //            case "monthly":
    //                #region Monthly Schedule

    //                // find the next schedule date time
    //                for (int i=0; i<366; i++)
    //                {
    //                    // check whether the schedule match the monthly settings
    //                    if (MatchMonthlySchedule(htSchedule, nextScheduledDateTime))
    //                    {
    //                        // call the check daily frequency to see whether there is a schedule match
    //                        if (CheckScheduleDailyFrequency(dtIntervalStart, 
    //                            prevScheduledDateTime, ref nextScheduledDateTime,
    //                            everyMinutes, duration))
    //                        {
    //                            // this is the schedule
    //                            scheduleFound = true;
    //                            break;
    //                        }
    //                        else
    //                        {
    //                            // mark this as previous schedule date
    //                            prevScheduledDateTime = nextScheduledDateTime;
    //                        }
    //                    }

    //                    // increment by 1 to next iteration
    //                    nextScheduledDateTime = nextScheduledDateTime.AddDays(1);
    //                }
    //                #endregion
    //                break;
    //            default:
    //                break;
    //        }

    //        // check whether the schedule time fall into the interval range
    //        return (scheduleFound
    //            && DateTime.Compare(nextScheduledDateTime, dtIntervalStart) >= 0 
    //            && DateTime.Compare(nextScheduledDateTime, dtIntervalEnd) <= 0);
    //    }

    //    private bool MatchMonthlySchedule(Hashtable htSchedule, DateTime scheduledDateTime)
    //    {
    //        // split the month 
    //        ArrayList arrMonth = SplitToArrayList((string)htSchedule["month"], "|");
    //        // split the day
    //        ArrayList arrDay = SplitToArrayList((string)htSchedule["day"], "|");

    //        bool matchDay = false;
    //        // check if the month contains in the setting
    //        string month = scheduledDateTime.Month.ToString();
    //        if (arrMonth.Contains(month))
    //        {
    //            // check if the day contains in the setting
    //            string day = scheduledDateTime.Day.ToString();
    //            if (arrDay.Contains(day))
    //            {
    //                matchDay = true;
    //            }
    //            else
    //            {
    //                // otherwise, check day of week settings
    //                string dayOfWeek = scheduledDateTime.DayOfWeek.ToString().ToLower();
    //                switch ((string)htSchedule[dayOfWeek])
    //                {
    //                    case "first":
    //                        // if scheduledDateTime <= 7, 
    //                        // then this is the day
    //                        matchDay = (scheduledDateTime.Day <= 7);
    //                        break;
    //                    case "second":
    //                        // if scheduledDateTime <= 14,  
    //                        // then this is the day
    //                        matchDay = (scheduledDateTime.Day <= 14);
    //                        break;
    //                    case "third":
    //                        // if scheduledDateTime <= 21,  
    //                        // then this is the day
    //                        matchDay = (scheduledDateTime.Day <= 21);
    //                        break;
    //                    case "fourth":
    //                        // if scheduledDateTime <= 28,  
    //                        // then this is the day
    //                        matchDay = (scheduledDateTime.Day <= 28);
    //                        break;
    //                    case "last":
    //                        // get next week date
    //                        DateTime nextWeekDateTime = scheduledDateTime.AddDays(7);
    //                        // if month of scheduledDateTime != month of nextWeekDateTime, 
    //                        // then this is the day
    //                        matchDay = (scheduledDateTime.Month != nextWeekDateTime.Month);
    //                        break;
    //                    default:
    //                        break;
    //                }
    //            }
    //        }
    //        return matchDay;
    //    }

    //    private bool CheckScheduleDailyFrequency(DateTime dtIntervalStart, 
    //        DateTime prevScheduledDateTime,	ref DateTime nextScheduledDateTime,
    //        double everyMinutes, double duration)
    //    {
    //        DateTime dtCurr = nextScheduledDateTime;
    //        // find the next scheduled date time after the start of interval
    //        while (DateTime.Compare(dtCurr, dtIntervalStart) < 0)
    //        {
    //            // add everyMinutes to dtCurr
    //            dtCurr = dtCurr.AddMinutes(everyMinutes);
    //            // check this schedule whether it is within the duration
    //            if (((TimeSpan)(dtCurr - prevScheduledDateTime)).TotalMinutes > duration)
    //            {
    //                // this schedule passes the duration
    //                return false;
    //            }
    //        }

    //        // this is the day
    //        nextScheduledDateTime = dtCurr;
    //        return true;
    //    }

    //    /// <summary>
    //    /// Spilt the string and put into an Array List
    //    /// </summary>
    //    /// <param name="theString">the string to be split</param>
    //    /// <param name="separator">the separator</param>
    //    /// <returns>Array List</returns>
    //    private ArrayList SplitToArrayList(string theString, string separator)
    //    {
    //        string[] arrString = theString.Split(separator.ToCharArray());
    //        ArrayList arrList = new ArrayList();
    //        foreach (string item in arrString)
    //        {
    //            arrList.Add(item);
    //        }
    //        return arrList; 

    //    }
    //    #endregion
    //}
}
