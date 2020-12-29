using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TemperatureWPF.Models;

namespace TemperatureWPF
{
    class Setup
    {

        [DllImport("Kernel32")]
        public static extern void AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();
        public static void ImportToDB()
        {
            AllocConsole();
            var j = "2016 - 05 - 31 13:58:30,Inne,24.8,42";
            string[] temperatureFileRows = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\TemperaturData.csv");


            using (var context = new TemperatureDBContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        int i = 1;
                        foreach (string row in temperatureFileRows)
                        {

                            string debugData = "";

                            string[] rowData = row.Split(',');
                            DateTime dateFromFile = DateTime.Parse(rowData[0]);
                            double? temperatureFromFile = double.Parse(rowData[2].Replace('.', ',')); //ersätter . med , för att kunda köra parse
                            int humidityFromFile = int.Parse(rowData[3]);

                            if (rowData[1] == "Inne")
                            {
                                Indoor temperatureIndoor = new Indoor()
                                {
                                    Date = dateFromFile,
                                    Temperature = temperatureFromFile,
                                    Humidity = humidityFromFile
                                };
                                context.Indoors.Add(temperatureIndoor);
                                debugData = $"{i}. Added indoor date: {temperatureIndoor.Date.ToString()},\n\t Deg: {temperatureIndoor.Temperature} Hum: {temperatureIndoor.Humidity}";
                            }
                            else
                            {
                                Outdoor temperatureOutdoor = new Outdoor()
                                {
                                    Date = dateFromFile,
                                    Temperature = temperatureFromFile,
                                    Humidity = humidityFromFile
                                };

                                context.Outdoors.Add(temperatureOutdoor);

                                debugData = $"{i}. Added outdoor date: {temperatureOutdoor.Date.ToString()},\n\t Deg: {temperatureOutdoor.Temperature} Hum: {temperatureOutdoor.Humidity}";

                            }
                            Console.WriteLine(debugData);
                            i++;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + " " + e.InnerException);
                        transaction.Rollback();
                    }
                    finally
                    {
                        transaction.Commit();
                    }
                }
                context.SaveChanges();
            }
            FreeConsole();
        }
    }
}
