using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TemperatureWPF.Models;

namespace TemperatureWPF
{
    ///<summary>
    ///Interaction logic for MainWindow.xaml
    ///</summary>
    public partial class MainWindow : Window
    {


        List<DateTime> IndoorDates { get; set; }
        List<DateTime> OutdoorDates { get; set; }
        List<CalendarDateRange> OutdoorBlackoutDates { get; set; }
        List<CalendarDateRange> IndoorBlackoutDates { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            Setup.Verify();
            SetupDatePicker();

        }

        private void SetupDatePicker()
        {

            IndoorDates = Dates.ExtractDates<Indoor>();
            OutdoorDates = Dates.ExtractDates<Outdoor>();
            OutdoorBlackoutDates = Dates.FindMissingDates(OutdoorDates);
            IndoorBlackoutDates = Dates.FindMissingDates(IndoorDates);
            indoorRadioButton.IsChecked = true;
        }

        private void outdoorRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            indoorRadioButton.IsChecked = !outdoorRadioButton.IsChecked;
            EnableDates(OutdoorDates);
            datePicker.BlackoutDates.Clear();
            foreach (var range in OutdoorBlackoutDates)
            {
                datePicker.BlackoutDates.Add(range);
            }
        }

        private void indoorRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            outdoorRadioButton.IsChecked = !indoorRadioButton.IsChecked;
            EnableDates(IndoorDates);

            datePicker.BlackoutDates.Clear();
            foreach (var range in IndoorBlackoutDates)
            {
                datePicker.BlackoutDates.Add(range);
            }
        }


        private void EnableDates(List<DateTime> dates)
        {
            dates = dates.OrderBy(date => date)
                         .ToList();

            datePicker.DisplayDateStart = dates.FirstOrDefault();
            datePicker.DisplayDateEnd = dates.LastOrDefault();
        }

        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

            DateTime selectedDate = datePicker.SelectedDate.Value;
            bool indoorTable = indoorRadioButton.IsChecked.Value;
            double temperatureForSelectedDate;
            string location = "";
            string selectedDateString = selectedDate.Date.ToString("d");


            switch (indoorTable)
            {
                case true:
                    temperatureForSelectedDate = SearchDatabase.AverageTemperatureSpecifiedDate<Indoor>(selectedDate);
                    location = "indoors";
                    break;
                case false:
                    temperatureForSelectedDate = SearchDatabase.AverageTemperatureSpecifiedDate<Outdoor>(selectedDate);
                    location = "outdoors";
                    break;

            }
            MessageBox.Show($"At {selectedDateString} it was an average temperature of {temperatureForSelectedDate} {location}.");
        }



        private void medianTemperature_Click(object sender, RoutedEventArgs e)
        {
            bool indoorTable = indoorRadioButton.IsChecked.Value;
            if (indoorTable)
            {
                dataGrid.ItemsSource = SearchDatabase.GetAverageTemperatures<Indoor>();
            }
            else
            {
                dataGrid.ItemsSource = SearchDatabase.GetAverageTemperatures<Outdoor>();
            }

        }

        private void medianHumidity_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new TemperatureDBContext())
            {
                bool indoorTable = indoorRadioButton.IsChecked.Value;
                if (indoorTable)
                {
                    dataGrid.ItemsSource = SearchDatabase.GetAverageHumidities<Indoor>();
                }
                else
                {
                    dataGrid.ItemsSource = SearchDatabase.GetAverageHumidities<Outdoor>();
                }
            }
        }

        private void autumnStart_Click(object sender, RoutedEventArgs e)
        {
            DateTime? autumnStartDate = SearchDatabase.FindAutumnStart();
            if (!autumnStartDate.HasValue)
            {
                MessageBox.Show("Could not find the start of Autumn");
            }
            else
            {
                MessageBox.Show($"Autumn started at: {Dates.FormatDate(autumnStartDate.Value)}");
            }
        }

        private void winterStartButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime? winterStartDate = SearchDatabase.FindWinterStart();
            if (!winterStartDate.HasValue)
            {
                MessageBox.Show("Could not find the start of Winter");
            }
            else
            {
                string winterStart = winterStartDate.Value.ToString("d");
                MessageBox.Show($"Winter started at: {winterStart}");
            }

        }

        private void moldRiskButton_Click(object sender, RoutedEventArgs e)
        {
            bool indoorTable = indoorRadioButton.IsChecked.Value;
            if (indoorTable)
            {
                dataGrid.ItemsSource = SearchDatabase.ChanceOfMold<Indoor>();
            }
            else
            {
                dataGrid.ItemsSource = SearchDatabase.ChanceOfMold<Outdoor>();
            }

        }
    }
}
