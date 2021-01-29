using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TemperatureWPF.Models;

namespace TemperatureWPF
{
    class Dates
    {




        /// <summary>
        /// Returns 
        /// </summary>
        /// <param name="dt">DateTime to format</param>
        /// <returns>
        /// a new DateTime with the same year,month,day 
        /// and the rest is default
        /// </returns>
        public static DateTime FormatDate(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day); //det är endast år, månad och dag som behövs

        }


        /// <summary>
        /// Gets all DateTime objects from the table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>List of DateTimes for the specified type</returns>
        public static List<DateTime> ExtractDates<T>()
        {
            Type t = typeof(T);


            using (var context = new TemperatureDBContext())
            {
                //kollar vilken typ det är
                if (t == typeof(Indoor))
                {
                    return context.Indoors.AsEnumerable()
                                         .GroupBy(indoorData => FormatDate(indoorData.Date))//grupperar efter datum  
                                         .Select(group => group.Key)//väljer datumet i format yyyy-DD-mm
                                         .OrderBy(date => date) //sorterar på datumet
                                         .ToList();
                }
                else
                {
                    return context.Outdoors.AsEnumerable()
                                         .GroupBy(outdoorData => FormatDate(outdoorData.Date))
                                         .Select(group => group.Key)
                                         .OrderBy(date => date)
                                         .ToList();
                }
            }
        }

        /// <summary>
        /// Finds the missing dates from the specified list of DateTime objects
        /// </summary>
        /// <param name="dates">List of DateTimes</param>
        /// <returns>List of CalendarDateRange to add to blackout dates for DatePicker</returns>
        public static List<CalendarDateRange> FindMissingDates(List<DateTime> dates)
        {

            List<CalendarDateRange> datesToBlock = new List<CalendarDateRange>();
            foreach (var date in dates)
            {
                if (dates.LastOrDefault() == date) // är det den sista i listan avbryts loopen
                {
                    break;
                }
                DateTime dateAfter = dates.ElementAt(dates.IndexOf(date) + 1); //datumet efter i listan
                double daysBetween = (dateAfter - date).TotalDays; //hur många dagar det är mellan datumen




                if (daysBetween > 1)
                {
                    //är det mer än 1 dag mellan datumen så skapas en ny CalendarDateRange
                    DateTime startDate = date.AddDays(1); //startdatum för CalendarDateRange, 1 dag efter start
                    DateTime endDate = dateAfter.AddDays(-1); //slutdatum för CalendarDateRange, 1 dag efter start
                    datesToBlock.Add(new CalendarDateRange(startDate, endDate));
                }
            }
            return datesToBlock;
        }
    }
}
