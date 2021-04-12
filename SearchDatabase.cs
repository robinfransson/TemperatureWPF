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
                //beroende på vilken typ T är så väljs tabell
                if (t == typeof(Outdoor))
                {
                    averageHumidities = context.Outdoors
                                     .AsEnumerable()
                                     .GroupBy(outdoor => Dates.FormatDate(outdoor.Date))//medel fuktighet för datumet (gruppen)
                                     .Select(grp => new DataHolder(date: grp.Key, 
                                                                   value: AverageHumidityOutdoor(grp)))
                                     .OrderBy(data => data.Value) //Value är i det här fallet Medelluftfuktighet
                                     .ToList();

                }


                else if (t == typeof(Indoor))
                {
                    averageHumidities = context.Indoors
                                      .AsEnumerable()
                                      .GroupBy(indoor => Dates.FormatDate(indoor.Date))
                                      .Select(grp => new DataHolder(date: grp.Key, 
                                                                    value: AverageHumidityIndoor(grp)))
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
                                            .Where(outdoor => outdoor.Date >= firstDateToCheck) //alla datum senare än tidigaste höstdatumet
                                            .GroupBy(outdoor => Dates.FormatDate(outdoor.Date)) //grupperar på datum yyyy-MM-dd
                                            .Where(AutumnTemperatureRange) //där temperaturen är under 10 och över 0
                                            .OrderBy(group => group.Key) //grupperar på datumet i fallande ordning
                                            .ToList();
                foreach (var grp in groupedByDate)
                {
                    DateTime startDate = grp.Key;


                    //kollar hur många dagar i följd det finns i groupedByDate, 

                    //antalet dagar det är innan startDate + 4 dagar
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
                                      .GroupBy(indoor => Dates.FormatDate(indoor.Date))//grupperar år-månad-dag
                                      .Select(grp => new DataHolder(date: grp.Key, 
                                                                    value: AverageTemperatureOutdoor(grp)))
                                      .OrderByDescending(data => data.Value) //Value är i den här contexten Medel temperatur
                                      .ToList();

                }
                else if (t == typeof(Indoor))
                {
                    averageTemperatures = context.Indoors
                                       .AsEnumerable()
                                       .GroupBy(indoor => Dates.FormatDate(indoor.Date))
                                       .Select(grp => new DataHolder(date: grp.Key, value: AverageTemperatureIndoor(grp)))
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
                                                  .GroupBy(outdoor => Dates.FormatDate(outdoor.Date)) //grupperar på år-månad-dag
                                                  .Where(group => group.Key == date) //väljer den grupp som har det datum som kommer från DatePicker
                                                  .Select(AverageTemperatureOutdoor) //väljer värdet för medeltemperaturen
                                                  .FirstOrDefault(); //första eller 0
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
                                                      .GroupBy(outdoor => Dates.FormatDate(outdoor.Date))//grupperar på datum yyyy-MM-dd
                                                      .Select(group => new MoldRisk(date: group.Key,
                                                                                  humidity: group.Average(grp => grp.Humidity), //medel fuktighet för datumet (gruppen)
                                                                                  temperature: group.Average(grp => grp.Temperature)))//medel temp för datumet (gruppen)
                                                      .OrderBy(moldChance => moldChance.RiskPercent) //sorterar på mögelrisk
                                                      .ToList();
                }




                else if (t == typeof(Indoor))
                {
                    listOfMoldRisks = context.Indoors
                                             .AsEnumerable()
                                             .GroupBy(indoor => Dates.FormatDate(indoor.Date))
                                             .Select(group => new MoldRisk(date: group.Key,
                                                                        humidity: group.Average(grp => grp.Humidity),
                                                                        temperature: group.Average(grp => grp.Temperature)))
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