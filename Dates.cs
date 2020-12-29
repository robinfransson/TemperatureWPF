using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TemperatureWPF.Models;

namespace TemperatureWPF
{
    class Dates
    {

        public static readonly string DateFormat = "yyyy-MM-dd";
        public static List<DateTime> ExtractDates<T>(List<T> temperatureData)
        {

            
            if (typeof(Indoor).IsAssignableFrom(typeof(T)))
            {
                return temperatureData.OfType<Indoor>().AsEnumerable()
                                     .GroupBy(indoorData => indoorData.Date.Value.ToString(DateFormat))
                                     .Select(group => DateTime.Parse(group.Key))

                                     .OrderBy(date => date.Date)
                                     .ToList();
            }
            else
            {
                return temperatureData.OfType<Outdoor>().AsEnumerable()
                                     .GroupBy(indoorData => indoorData.Date.Value.ToString(DateFormat))
                                     .Select(group => DateTime.Parse(group.Key))
                                     .OrderBy(date => date.Date)
                                     .ToList();
            }
        }


        public static List<CalendarDateRange> FindMissingDates(List<DateTime> dates)
        {
            List<CalendarDateRange> datesToBlock = new List<CalendarDateRange>();
            foreach(var date in dates)
            {
                if(dates.IndexOf(date) + 1 >= dates.Count)
                {
                    break;
                }
                DateTime dateAfter = dates.ElementAt(dates.IndexOf(date) + 1);
                double daysBetween = (dateAfter - date).TotalDays;
                if(daysBetween > 1)
                {
                    datesToBlock.Add(new CalendarDateRange(date.AddDays(1), dateAfter.AddDays(-1)));
                }
            }
            return datesToBlock;
        }
    }
}
