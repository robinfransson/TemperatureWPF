using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TemperatureWPF.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace TemperatureWPF
{
    class Setup
    {

        [DllImport("Kernel32")]
        public static extern void AllocConsole();
        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        public static void Verify()
        {
            bool dbCreatedNow;
            bool dataInOutdoorTable;
            bool dataInIndoorTable;
            using (var context = new TemperatureDBContext())
            {

                



                //finns inte databasen och tabellerna skapas dom här
                dbCreatedNow = context.Database.EnsureCreated();
                dataInOutdoorTable = context.Outdoors.Any();
                dataInIndoorTable = context.Indoors.Any();
                if (dbCreatedNow)
                {
                    MessageBox.Show("Tables created now=" + dbCreatedNow);
                }

            }
            CheckIfDataInTables(dataInIndoorTable, dataInOutdoorTable);
            //FreeConsole();
        }

        private static void CheckIfDataInTables(bool dataInIndoorTable, bool dataInOutdoorTable)
        {
            bool consoleOpen = false;
            if (!dataInOutdoorTable || !dataInOutdoorTable)
            {
                //saknas det data i någon av tabellerna öppnas ett konsolfönster för output
                //när data ska importeras till databasen
                consoleOpen = true;
                AllocConsole();
            }
            if (!dataInIndoorTable)
            {
                Console.WriteLine("Table Indoor does not have data! Press anywhere to import it!");
                Console.ReadKey();
                ImportToDB<Indoor>();
            }
            if (!dataInOutdoorTable)
            {
                Console.WriteLine("Table Outdoor does not have data! Press anywhere to import it!");
                Console.ReadKey();
                ImportToDB<Outdoor>();
            }
            if(consoleOpen)
            {
                FreeConsole();
            }
        }
        /// <summary>
        /// Startar en ny transaktion där datat läggs in i databasen, beroende på vilken typ T är
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void ImportToDB<T>()
        {
            //format "2016 - 05 - 31 13:58:30,Inne,24.8,42";
            string[] temperatureFileRows;

            Type t = typeof(T);

            if(t == typeof(Outdoor))
            {
               temperatureFileRows = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\TemperaturData.csv")
                                           .Where(line => line.Contains($",Ute,"))
                                           .ToArray();
            }
            else
            {
                temperatureFileRows = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\TemperaturData.csv")
                                           .Where(line => line.Contains($",Inne,"))
                                           .ToArray();
            }

            int i = 1;


            using (var context = new TemperatureDBContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {

                    try
                    {
                        foreach (string row in temperatureFileRows)
                        {
                            string consoleOutput = "";

                            string[] rowData = row.Split(',');
                            DateTime dateFromFile = DateTime.Parse(rowData[0]);
                            double temperatureFromFile = double.Parse(rowData[2].Replace('.', ',')); //ersätter . med , för att kunda köra parse
                            int humidityFromFile = int.Parse(rowData[3]);

                            if (t == typeof(Indoor))
                            {
                                Indoor temperatureIndoor = new Indoor()
                                {
                                    Date = dateFromFile,
                                    Temperature = temperatureFromFile,
                                    Humidity = humidityFromFile
                                };
                                context.Indoors.Add(temperatureIndoor);
                                consoleOutput = $"#{i}. Added indoor date: {temperatureIndoor.Date}," +
                                                $"\n\t Deg: {temperatureIndoor.Temperature} Hum: {temperatureIndoor.Humidity}";
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

                                consoleOutput = $"#{i}. Added outdoor date: {temperatureOutdoor.Date}," +
                                                $"\n\t Deg: {temperatureOutdoor.Temperature} Hum: {temperatureOutdoor.Humidity}";

                            }
                            Console.WriteLine(consoleOutput);
                            i++;
                        }
                        Console.WriteLine("Saving changes please wait...");
                        context.SaveChanges();
                        Console.WriteLine("Done! press any key to continue");
                        Console.ReadKey();

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + "\n\t" + e.InnerException);
                        transaction.Rollback();
                    }
                    finally
                    {
                        Console.WriteLine("Comitting please wait...");
                        transaction.Commit();
                    }
                }
            }
        }
    }
}























//finns inte databasen skapas den
//if (!context.Database.GetService<IRelationalDatabaseCreator>().Exists())
//{
//    context.Database.GetService<IRelationalDatabaseCreator>().Create();
//}
//