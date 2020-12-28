create database TemperatureDB

use TemperatureDB



drop table Temperatures
/*
create table Temperatures (
			ID int IDENTITY NOT NULL,
			Date DateTime2,
			Location varchar(max),
			Temperature float,
			Humidity int,
			PRIMARY KEY (ID))
*/


create table Indoor (
						ID int IDENTITY NOT NULL,
						Date DateTime2,
						Temperature float,
						Humidity int,
						PRIMARY KEY (ID))
create table Indoor (
						ID int IDENTITY NOT NULL,
						Date DateTime2,
						Temperature float,
					  Humidity int,
					  PRIMARY KEY (ID))




select AVG(Temperature) from Temperatures
WHERE Date like '2016-11-10%' AND Location = 'Ute'
