using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TemperatureWPF.Models;

namespace TemperatureWPF
{
    class SearchDatabase
    {

        private static Func<DateTime, DateTime, bool> FiveDaysInARow
        {
            get
            {
                return (startDate, currentDate) => (currentDate >= startDate) && (currentDate <= startDate.AddDays(4));
            }
        }

        //om medeltemperatur är mindre än 10 och över 0 (meterologisk höst)
        private static bool AutumnTemperatureRange(IGrouping<DateTime, Outdoor> grp) => AverageTemperatureOutdoor(grp) >= 0.00d && AverageTemperatureOutdoor(grp) <= 10.00d;

        //om medeltemperatur är mindre än 0 (meterologisk höst)
        private static bool WinterTemperatureRange(IGrouping<DateTime, Outdoor> grp) => AverageTemperatureOutdoor(grp) <= 0.00d;


        private static double AverageTemperatureOutdoor(IGrouping<DateTime, Outdoor> grp) => Math.Round(grp.Average(data => data.Temperature), 2);
        private static double AverageTemperatureIndoor(IGrouping<DateTime, Indoor> grp) => Math.Round(grp.Average(data => data.Temperature), 2);



        private static double AverageHumidityOutdoor(IGrouping<DateTime, Outdoor> grp) => Math.Round(grp.Average(data => data.Humidity), 2);
        private static double AverageHumidityIndoor(IGrouping<DateTime, Indoor> grp) => Math.Round(grp.Average(data => data.Humidity), 2);




        public static List<DataHolder> GetAverageHumidities<T>()
        {
            List<DataHolder> averageHumidities = new List<DataHolder>();
            Type t = typeof(T);


            using (var context = new TemperatureDBContext())
            {
                //kollar vilken typ som listan innehåller
                if (t == typeof(Outdoor))
                {
                    averageHumidities = context.Outdoors
                                     .AsEnumerable()
                                     .GroupBy(outdoor => Dates.FormatDate(outdoor.Date))
                                     .Select(grp => new DataHolder(value: AverageHumidityOutdoor(grp),
                                                                   date: grp.Key))
                                     .OrderBy(data => data.Value)
                                     .ToList();

                }


                else if (t == typeof(Indoor))
                {
                    averageHumidities = context.Indoors
                                      .AsEnumerable()
                                      .GroupBy(indoor => Dates.FormatDate(indoor.Date))
                                      .Select(grp => new DataHolder(value: AverageHumidityIndoor(grp),
                                                                    date: grp.Key))
                                      .OrderBy(data => data.Value)
                                      .ToList();

                }

                return averageHumidities;
            }
        }


        public static DateTime? FindWinterStart()
        {
            DateTime firstDateToCheck = new DateTime(2016, 08, 01); //vintern börjar tidigast den 1a augusti
            DateTime? winterStartDate = null;

            using (var context = new TemperatureDBContext())
            {
                var groupedByDate = context.Outdoors.AsEnumerable()
                                            .Where(outdoor => outdoor.Date >= firstDateToCheck) //allt som är senare än firstDate
                                            .GroupBy(outdoor => Dates.FormatDate(outdoor.Date)) //grupperar dom 
                                            .OrderBy(group => group.Key)
                                            .Where(WinterTemperatureRange)
                                            .ToList();

                foreach (var grp in groupedByDate)
                {
                    //Key är yyyy-MM-dd i format

                    DateTime firstDate = grp.Key;

                    var followingDays = groupedByDate.Count(group => FiveDaysInARow(firstDate, group.Key));


                    if (followingDays >= 5)
                    {
                        //om det finns minst 5 dagar i följd där medeltemperaturen är under 0 så är det meterologisk vinter
                        winterStartDate = firstDate;
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


        public static DateTime? FindAutumnStart()
        {
            DateTime firstDateToCheck = new DateTime(2016, 08, 01); // hösten börjar tidigast 1a augusti
            DateTime? autumnStartDate = null;



            using (var context = new TemperatureDBContext())
            {
                var groupedByDate = context.Outdoors.AsEnumerable()
                                            .Where(outdoor => outdoor.Date >= firstDateToCheck) //alla datum senare än första datumet
                                            .GroupBy(outdoor => Dates.FormatDate(outdoor.Date))
                                            .Where(AutumnTemperatureRange)
                                            .OrderBy(group => group.Key)
                                            .ToList();
                foreach (var grp in groupedByDate)
                {
                    DateTime startDate = grp.Key;
                    DateTime endDate = grp.Key.AddDays(4);
                    var followingDays = groupedByDate.Count(group => FiveDaysInARow(startDate, group.Key));


                    //om det finns 5 dagar i följd där medeltemperaturen är över 0 och under 10 så är det meterologisk höst
                    if (followingDays >= 5)
                    {
                        //då sätts autumnStartDate till det datumet som kollades
                        autumnStartDate = startDate;
                        break;
                    }
                    else
                    {
                        continue; //annars fortsätter loopen
                    }
                }
            }
            return autumnStartDate;
        }


        public static List<DataHolder> GetAverageTemperatures<T>()
        {
            List<DataHolder> averageTemperatures = new List<DataHolder>();
            Type t = typeof(T);

            using (var context = new TemperatureDBContext())
            {
                if (t == typeof(Outdoor))
                {
                    averageTemperatures = context.Outdoors
                                      .AsEnumerable()
                                      .GroupBy(indoor => Dates.FormatDate(indoor.Date))
                                      .Select(grp => new DataHolder(value: AverageTemperatureOutdoor(grp),
                                                                    date: grp.Key))
                                      .OrderByDescending(data => data.Value)
                                      .ToList();

                }
                else if (t == typeof(Indoor))
                {
                    averageTemperatures = context.Indoors
                                       .AsEnumerable()
                                       .GroupBy(indoor => Dates.FormatDate(indoor.Date))
                                       .Select(grp => new DataHolder(value: AverageTemperatureIndoor(grp),
                                                                     date: grp.Key))
                                       .OrderByDescending(data => data.Value)
                                       .ToList();
                }
                else
                {
                    throw new NotImplementedException(message: "Type not implemented" + typeof(T));
                }

                return averageTemperatures;
            }
        }


        public static double AverageTemperatureSpecifiedDate<T>(DateTime date)
        {
            double temperature;
            Type t = typeof(T);




            using (var context = new TemperatureDBContext())
            {

                if (t == typeof(Outdoor))
                {
                    temperature = context.Outdoors.AsEnumerable()
                                                  .GroupBy(outdoor => Dates.FormatDate(outdoor.Date)) //grupperar 
                                                  .Where(group => group.Key == date)
                                                  .Select(AverageTemperatureOutdoor)
                                                  .FirstOrDefault();
                }
                else if (t == typeof(Indoor))
                {
                    temperature = context.Indoors.AsEnumerable()
                                                 .GroupBy(indoor => Dates.FormatDate(indoor.Date))
                                                 .Where(group => group.Key == date)
                                                 .Select(AverageTemperatureIndoor)
                                                 .FirstOrDefault();
                }
                else
                {
                    throw new NotImplementedException(message: "Type " + typeof(T) + "not implemented");
                }

                return temperature;
            }
        }




        public static List<MoldRisk> ChanceOfMold<T>()
        {
            //mögelrisk =((fuktighet -78) * (temperatur/15))/0,22
            Type t = typeof(T);
            List<MoldRisk> listOfMoldRisks = new List<MoldRisk>();




            using (var context = new TemperatureDBContext())
            {
                if (t == typeof(Outdoor))
                {
                    listOfMoldRisks = context.Outdoors.AsEnumerable()
                                                      .GroupBy(outdoor => Dates.FormatDate(outdoor.Date))
                                                      .Select(s => new MoldRisk(date: s.Key,
                                                                                  humidity: s.Average(grp => grp.Humidity),
                                                                                  temperature: s.Average(grp => grp.Temperature)))
                                                      .OrderBy(moldChance => moldChance.RiskPercent)
                                                      .ToList();
                }




                else if (t == typeof(Indoor))
                {
                    listOfMoldRisks = context.Indoors
                                             .AsEnumerable()
                                             .GroupBy(indoor => Dates.FormatDate(indoor.Date))
                                             .Select(s => new MoldRisk(date: s.Key,
                                                                        humidity: s.Average(grp => grp.Humidity),
                                                                        temperature: s.Average(grp => grp.Temperature)))
                                             .OrderBy(moldChance => moldChance.RiskPercent)
                                             .ToList();

                }
                else
                {
                    throw new NotImplementedException();
                }



                return listOfMoldRisks;
            }
        }

    }
}