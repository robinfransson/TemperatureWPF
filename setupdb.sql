/*
Scaffold-DbContext "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TemperatureDB;Integrated Security=True;" microsoft.entityframeworkcore.sqlserver -OutputDir Models -f
IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("TemperatureDB"));--drop database TemperatureDB

Select Count(i.Date)
from Indoor as i


select count(o.Date)
from Outdoor as o



join Outdoor as O
on i.Date = O.Date
select AVG(Temperature) from Temperatures
WHERE Date like '2016-11-10%' AND Location = 'Ute'

Select avg(Humidity) from Outdoor
group by Humidity
order by Humidity desc
*/

/*
create table Temperatures (
			ID int IDENTITY NOT NULL,
			Date DateTime2,
			Location varchar(max),
			Temperature float,
			Humidity int,
			PRIMARY KEY (ID))
*/

drop table Indoor, Outdoor

create database TemperatureDB

use TemperatureDB







create table Indoor (
					  ID int IDENTITY NOT NULL,
					  Date DateTime2 NOT NULL,
					  Temperature float NOT NULL,
					  Humidity int NOT NULL,
					  PRIMARY KEY (ID))

create table Outdoor (
						ID int IDENTITY NOT NULL,
						Date DateTime2 NOT NULL,
						Temperature float NOT NULL,
					  Humidity int NOT NULL,
					  PRIMARY KEY (ID))

select AVG(Temperature) from Indoor 
WHERE CONVERT(VARCHAR(25), Date, 126) LIKE '2016-06-16%'






