using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemperatureWPF.Models;

namespace TemperatureWPF
{
    class SearchDatabase
    {
        private static Func<DateTime, string> formatDate = (dt) => dt.ToString(Dates.DateFormat);
        private static Func<IGrouping<string, Outdoor>, double> averageTemperatureForGroup = (grp) => grp.Average(outdoor => outdoor.Temperature.Value);
        public static List<DataHolder> GetAverageHumidities<T>(List<T> table)
        {
            List<DataHolder> medians = new List<DataHolder>();
            Type t = typeof(T);
            if (t.Equals(typeof(Outdoor)))
            {
                var groupedAverage = table.OfType<Outdoor>().GroupBy(outdoor => outdoor.Date.Value.ToString(Dates.DateFormat))
                    .OrderBy(group => group.Average(outdoor => outdoor.Humidity));

                foreach (var group in groupedAverage)
                {
                    string date = group.Key;
                    double humidity = (double)Math.Round(group.Average(temp => temp.Humidity).Value, 2);

                    medians.Add(new DataHolder(humidity, date));

                }
            }

            else if (t.Equals(typeof(Indoor)))
            {
                var groupedAverage = table.OfType<Indoor>().GroupBy(indoor => formatDate(indoor.Date.Value))
                    .OrderBy(group => group.Average(indoor => indoor.Humidity));

                foreach (var group in groupedAverage)
                {
                    string date = group.Key;
                    double humidity = (double)Math.Round(group.Average(temp => temp.Humidity).Value, 2);

                    medians.Add(new DataHolder(humidity, date));

                }

            }

            return medians;
        }


        public static DateTime? FindWinterStart(int year)
        {
            DateTime firstDateToCheck = new DateTime(year, 08, 01);
            DateTime? winterStartDate = null;
            using (var context = new TemperatureDBContext())
            {
                var dateGroups = context.Outdoors.AsEnumerable()
                                            .Where(outdoor => outdoor.Date.Value >= firstDateToCheck)
                                            .GroupBy(outdoor => formatDate(outdoor.Date.Value))
                                            .OrderBy(grp => grp.Key)
                                            .Where(grp => averageTemperatureForGroup(grp) <= 0.00d)
                                            .ToList();
                foreach (var group in dateGroups)
                {
                    double? averageTemperature = group.Average(outdoor => outdoor.Temperature);
                    DateTime currentDate = DateTime.Parse(group.Key);

                    var followingDays = dateGroups.Where(grp => DateTime.Parse(grp.Key) >= currentDate && DateTime.Parse(grp.Key) <= currentDate.AddDays(4))
                        //.Select(grp => grp.Average(grp => grp.Temperature))
                        .ToList();


                    if (followingDays.Count == 5)
                    {
                        winterStartDate = currentDate;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return winterStartDate;
        }


        public static DateTime? FindAutumnStart(int year)
        {
            DateTime firstDateToCheck = new DateTime(year, 08, 01);
            DateTime? autumnStartDate = null;
            using (var context = new TemperatureDBContext())
            {
                var dateGroups = context.Outdoors.AsEnumerable()
                                            .Where(outdoor => outdoor.Date.Value >= firstDateToCheck)
                                            .GroupBy(outdoor => formatDate(outdoor.Date.Value))
                                            .OrderBy(grp => grp.Key)
                                            .Where(grp => averageTemperatureForGroup(grp) >= 0.00d && averageTemperatureForGroup(grp) <= 10.00d)
                                            .ToList();
                foreach (var group in dateGroups)
                {
                    double? averageTemperature = group.Average(outdoor => outdoor.Temperature);
                    DateTime currentDate = DateTime.Parse(group.Key);

                    var followingDays = dateGroups.Where(group => DateTime.Parse(group.Key) >= currentDate && DateTime.Parse(group.Key) <= currentDate.AddDays(4))
                        //.Select(grp => grp.Average(outdoor => outdoor.Temperature))
                        .ToList();


                    if(followingDays.Count == 5)
                    {
                        autumnStartDate = currentDate;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return autumnStartDate;
        }


        public static List<DataHolder> GetAverageTemperatures<T>(List<T> table)
        {
            List<DataHolder> medians = new List<DataHolder>();
            Type t = typeof(T);

            if (t.Equals(typeof(Outdoor)))
            {
                var groupedAverage = table.OfType<Outdoor>().GroupBy(outdoor => outdoor.Date.Value.ToString(Dates.DateFormat))
                    .OrderByDescending(group => group.Average(outdoor => outdoor.Temperature));

                foreach (var group in groupedAverage)
                {
                    string date = group.Key;
                    double? temperature = (double?)Math.Round((decimal)group.Average(temp => temp.Temperature), 2);

                    medians.Add(new DataHolder(temperature, date));

                }
                return medians;
            }
            else if (t.Equals(typeof(Indoor)))
            {
                var groupedAverage = table.OfType<Indoor>().GroupBy(indoor => indoor.Date.Value.ToString(Dates.DateFormat))
                    .OrderByDescending(group => group.Average(indoor => indoor.Temperature));

                foreach (var group in groupedAverage)
                {
                    string date = group.Key;
                    double? temperature = (double?)Math.Round((decimal)group.Average(temp => temp.Temperature), 2);

                    medians.Add(new DataHolder(temperature, date));

                }
                return medians;
            }

            else throw new NotImplementedException(message: "Type not implemented" + typeof(T).ToString());
        }


        public static double MedianTemperatureSpecifiedDate<T>(List<T> table, DateTime? date)
        {
            Type t = typeof(T);
            if (t.Equals(typeof(Outdoor)))
            {

                return (double)table.OfType<Outdoor>().AsEnumerable().GroupBy(outdoor => outdoor.Date.Value.ToString(Dates.DateFormat))
                    .Where(group => group.Key == date.Value.ToString(Dates.DateFormat))
                    .Select(group => group.Average(outdoor => outdoor.Temperature))
                    .FirstOrDefault();
            }
            else if (t.Equals(typeof(Indoor)))
            {
                return (double)table.OfType<Indoor>().GroupBy(indoor => indoor.Date.Value.ToString(Dates.DateFormat))
                    .Where(group => group.Key == date.Value.ToString(Dates.DateFormat))
                    .Select(group => group.Average(indoor => indoor.Temperature))
                    .FirstOrDefault();

            }


            else throw new NotImplementedException(message: "Type not implemented");
        }
        public static List<MoldChance> ChanceOfMold<T>(List<T> table)
        {
            Type t = typeof(T);
            List<MoldChance> groupedAverage;
            if (t.Equals(typeof(Outdoor)))
            {
                groupedAverage = table.OfType<Outdoor>()
                   .GroupBy(outdoor => outdoor.Date.Value.ToString(Dates.DateFormat))
                   .OrderByDescending(group => group.Average(outdoor => outdoor.Humidity))
                   .ThenBy(group => group.Average(outdoor => outdoor.Temperature))
                   .Select(s => new MoldChance(s.Key, s.Average(grp => grp.Humidity), s.Average(grp => grp.Temperature))).ToList();
            }
            else if (t.Equals(typeof(Indoor)))
            {
                groupedAverage = table.OfType<Indoor>()
                   .GroupBy(indoor => indoor.Date.Value.ToString(Dates.DateFormat))
                   .OrderByDescending(group => group.Average(indoor => indoor.Humidity))
                   .ThenBy(group => group.Average(indoor => indoor.Temperature))
                   .Select(s => new MoldChance(s.Key, s.Average(grp => grp.Humidity), s.Average(grp => grp.Temperature))).ToList();

            }
            else
            {
                throw new NotImplementedException();
            }
            return groupedAverage;
        }

    }


}
