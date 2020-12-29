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



        private static bool CompareTypes(Type t1, Type t2)
        {
            if (t1.IsAssignableFrom(t2))
            {
                return true;
            }
            return false;
        }


        public static List<DataHolder> GetAverageHumidities<T>(List<T> table)
        {
            List<DataHolder> medians = new List<DataHolder>();
            if (CompareTypes(typeof(Outdoor), (typeof(T))))
            {
                var groupedAverage = table.OfType<Outdoor>().GroupBy(outdoor => outdoor.Date.Value.ToString(Dates.DateFormat))
                    .OrderBy(group => group.Average(outdoor => outdoor.Humidity));

                foreach (var group in groupedAverage)
                {
                    string date = group.Key;
                    double? humidity = group.Average(temp => temp.Humidity);

                    medians.Add(new DataHolder(humidity, date));

                }
            }

            else if (CompareTypes(typeof(Indoor), typeof(T)))
            {
                var groupedAverage = table.OfType<Indoor>().GroupBy(indoor => indoor.Date.Value.ToString(Dates.DateFormat))
                    .OrderBy(group => group.Average(indoor => indoor.Humidity));

                foreach (var group in groupedAverage)
                {
                    string date = group.Key;
                    double? humidity = (double?)Math.Round((decimal)group.Average(temp => temp.Humidity), 2);

                    medians.Add(new DataHolder(humidity, date));

                }

            }

            return medians;
        }






        public static List<DataHolder> GetAverageTemperatures<T>(List<T> table)
        {
            List<DataHolder> medians = new List<DataHolder>();

            if (CompareTypes(typeof(Outdoor), (typeof(T))))
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
            else if (CompareTypes(typeof(Indoor), typeof(T)))
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

            if (CompareTypes(typeof(Outdoor), (typeof(T))))
            {

                return (double)table.OfType<Outdoor>().AsEnumerable().GroupBy(outdoor => outdoor.Date.Value.ToString(Dates.DateFormat))
                    .Where(group => group.Key == date.Value.ToString(Dates.DateFormat))
                    .Select(group => group.Average(outdoor => outdoor.Temperature))
                    .FirstOrDefault();
            }
            else if (CompareTypes(typeof(Indoor), typeof(T)))
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
            List<MoldChance> groupedAverage;
            if (CompareTypes(typeof(Outdoor), (typeof(T))))
            {
                groupedAverage = table.OfType<Outdoor>()
                   .GroupBy(outdoor => outdoor.Date.Value.ToString(Dates.DateFormat))
                   .OrderByDescending(group => group.Average(outdoor => outdoor.Humidity))
                   .ThenBy(group => group.Average(outdoor => outdoor.Temperature))
                   .Select(s => new MoldChance(s.Key, s.Average(grp => grp.Humidity), s.Average(grp => grp.Temperature))).ToList();
            }
            else if (CompareTypes(typeof(Indoor), typeof(T)))
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
