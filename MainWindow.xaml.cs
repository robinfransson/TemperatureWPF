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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        List<DateTime> indoorDates { get; set; } 
        List<DateTime> outdoorDates { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            SetupDatePicker();
        }

        private void SetupDatePicker()
        {
            using (var context = new TemperatureDBContext())
            {
                indoorDates = Dates.ExtractDates<Indoor>(context.Indoors.ToList());
                outdoorDates = Dates.ExtractDates<Outdoor>(context.Outdoors.ToList());
            }
            indoorRadioButton.IsChecked = true;
        }

        private void outdoorRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            indoorRadioButton.IsChecked = !outdoorRadioButton.IsChecked;
            EnableDates(outdoorDates);
            datePicker.BlackoutDates.Clear();
            foreach (var range in Dates.FindMissingDates(outdoorDates))
            {
                datePicker.BlackoutDates.Add(range);
            }
        }

        private void indoorRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            outdoorRadioButton.IsChecked = !indoorRadioButton.IsChecked;
            EnableDates(indoorDates);

            datePicker.BlackoutDates.Clear();
            foreach (var range in Dates.FindMissingDates(indoorDates))
            {
                datePicker.BlackoutDates.Add(range);
            }
        }


        private void EnableDates(List<DateTime> dates)
        {
            dates = dates.OrderBy(date => date.Year)
                .ThenBy(date => date.Month)
                .ThenBy(date => date.Day)
                .ToList();
            datePicker.DisplayDateStart = dates.FirstOrDefault();
            datePicker.DisplayDateEnd = dates.LastOrDefault();
        }

        private void importButton_Click(object sender, RoutedEventArgs e)
        {
            Setup.ImportToDB();
        }

        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            using (var context = new TemperatureDBContext())
            {
                DateTime? selectedDate = datePicker.SelectedDate.Value;
                List<object> table = indoorRadioButton.IsChecked.Value ? context.Indoors.ToList() : context.Outdoors;
                double? temperatureForSelectedDate = Math.Round(SearchDatabase.MedianTemperatureSpecifiedDate(context., selectedDate), 1);
                MessageBox.Show($"At {selectedDate.Value.ToString(Dates.DateFormat)} it was an average temperature of {temperatureForSelectedDate} {table}");
            }
           
            
        }

        private void medianTemperature_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new TemperatureDBContext())
            {
                if (indoorRadioButton.IsChecked.Value)
                {
                    dataGrid.ItemsSource = SearchDatabase.GetAverageTemperatures<Indoor>(context.Indoors.ToList());
                }
                else
                {
                    dataGrid.ItemsSource = SearchDatabase.GetAverageTemperatures<Outdoor>(context.Outdoors.ToList());
                }
            }
        }

        private void medianHumidity_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new TemperatureDBContext())
            {
                if (indoorRadioButton.IsChecked.Value)
                {
                    dataGrid.ItemsSource = SearchDatabase.GetAverageHumidities<Indoor>(context.Indoors.ToList());
                }
                else
                {
                    dataGrid.ItemsSource = SearchDatabase.GetAverageHumidities<Outdoor>(context.Outdoors.ToList());
                }
            }
        }
    }
}
