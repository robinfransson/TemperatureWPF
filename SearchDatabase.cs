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
            if(t1.IsAssignableFrom(t2))
            {
                return true;
            }
            return false;
        }


        public static List<MedianValue> GetAverageHumidities<T>(List<T> table)
        {
            List<MedianValue> medians = new List<MedianValue>();
            if (CompareTypes(typeof(Outdoor), (typeof(T))))
            {
                var groupedAverage = table.OfType<Outdoor>().GroupBy(outdoor => outdoor.Date.Value.ToString(Dates.DateFormat))
                    .OrderBy(group => group.Average(outdoor => outdoor.Humidity));

                foreach (var group in groupedAverage)
                {
                    string date = group.Key;
                    double? humidity = group.Average(temp => temp.Humidity);

                    medians.Add(new MedianValue(humidity, date));

                }
            }

            else if (CompareTypes(typeof(Indoor), typeof(T)))
            {
                var groupedAverage = table.OfType<Indoor>().GroupBy(indoor => indoor.Date.Value.ToString(Dates.DateFormat))
                    .OrderBy(group => group.Average(indoor => indoor.Humidity));

                foreach (var group in groupedAverage)
                {
                    string date = group.Key;
                    double? humidity = group.Average(temp => temp.Humidity);

                    medians.Add(new MedianValue(humidity, date));

                }

            }

            return medians;
        }






        public static List<MedianValue> GetAverageTemperatures<T>(List<T> table)
        {
            List<MedianValue> medians = new List<MedianValue>();
            
            //if(typeof(Outdoor).IsAssignableFrom(typeof(T)))
            if (CompareTypes(typeof(Outdoor), (typeof(T))))
            {
                var groupedAverage = table.OfType<Outdoor>().GroupBy(outdoor => outdoor.Date.Value.ToString(Dates.DateFormat))
                    .OrderByDescending(group => group.Average(outdoor => outdoor.Temperature));

                foreach (var group in groupedAverage)
                {
                    string date = group.Key;
                    double? temperature = (double?)Math.Round((decimal)group.Average(temp => temp.Temperature), 2);

                    medians.Add(new MedianValue(temperature, date));

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

                    medians.Add(new MedianValue(temperature, date));

                }
                return medians;
            }

            else throw new NotImplementedException(message: "Type not implemented" + typeof(T).ToString());
        }


        public static double MedianTemperatureSpecifiedDate(string table, DateTime? date)
        {
            using (var context = new TemperatureDBContext())
            {

                if (table == "Outdoor")
                {

                    return (double)context.Outdoors.AsEnumerable().GroupBy(outdoor => outdoor.Date.Value.ToString(Dates.DateFormat))
                        .Where(group => group.Key == date.Value.ToString(Dates.DateFormat))
                        .Select(group => group.Average(outdoor => outdoor.Temperature))
                        .FirstOrDefault();
                }
                else if (table == "Indoor")
                {
                    return (double)context.Indoors.AsEnumerable().GroupBy(indoor => indoor.Date.Value.ToString(Dates.DateFormat))
                        .Where(group => group.Key == date.Value.ToString(Dates.DateFormat))
                        .Select(group => group.Average(indoor => indoor.Temperature))
                        .FirstOrDefault();

                }


                else throw new NotImplementedException(message: "Type not implemented");
            }
        }
    }
}
