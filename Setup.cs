using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using TemperatureWPF.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            AllocConsole();
            using(var context = new TemperatureDBContext())
            {
                bool dataInOutdoorTable = context.Outdoors.Any();
                bool dataInIndoorTable = context.Indoors.Any();

                bool dbCreated = DatabaseExists();
                Console.WriteLine("Database created now=" + dbCreated);

                if (!dataInIndoorTable)
                {
                    Console.WriteLine("Table Indoor does not have data! Press anywhere to import it!");
                    Console.ReadKey();
                    ImportToDB("Inne");
                }
                if (!dataInOutdoorTable)
                {
                    Console.WriteLine("Table Outdoor does not have data! Press anywhere to import it!");
                    Console.ReadKey();
                    ImportToDB("Ute");
                }
            }
            FreeConsole();
        }


        public static void CreateTable(string tableName)
        {
            string sqlQuery = "create table " + tableName + "(" +
                              "ID int IDENTITY NOT NULL," +
                              "Date DateTime2," +
                              "Temperature float," +
                              "Humidity int," +
                              "PRIMARY KEY(ID))";
            using(var context = new TemperatureDBContext())
            {
                int queryResult = context.Database.ExecuteSqlRaw(sqlQuery);
                Console.WriteLine("result="+queryResult);
            }
        }

        public static bool DatabaseExists()
        {
            using (var context = new TemperatureDBContext())
            {
                return context.Database.EnsureCreated();

            }
        }

        public static bool TableExists(string table)
        {
            



            using(var context = new TemperatureDBContext())
            {
                var sqlQuery = $"SELECT COUNT(*) as Count FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{table}'";
                int count = context.Database.ExecuteSqlRaw(sqlQuery);
                return count > 0;

            }
        }
        public static void ImportToDB(string location=null)
        {
            //"2016 - 05 - 31 13:58:30,Inne,24.8,42";
            string[] temperatureFileRows = File.ReadAllLines(Directory.GetCurrentDirectory() + @"\TemperaturData.csv")
                                           .Where(line => line.Contains($",{location},"))
                                           .ToArray();


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
                        Console.WriteLine("Comitting please wait...");
                        transaction.Commit();
                    }
                }
                Console.WriteLine("Saving changes please wait...");
                context.SaveChanges();
                Console.WriteLine("Done! press any key to continue");
                Console.ReadKey();
            }
        }
    }
}
