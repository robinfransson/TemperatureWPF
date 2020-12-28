using System;
using System.Collections.Generic;
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
            string dateFormat = "yyyy-MM-dd";
            using(var context = new TemperatureDBContext())
            {
            indoorDates = context.Indoors.AsEnumerable().GroupBy(indoorData => indoorData.Date.Value.ToString(dateFormat))
            .Select(group => DateTime.Parse(group.Key))
            .ToList();


            outdoorDates = context.Indoors.AsEnumerable().GroupBy(outdoorData => outdoorData.Date.Value.ToString(dateFormat))
            .Select(group => DateTime.Parse(group.Key))
            .ToList();

            }
        }

        private void outdoorRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            indoorRadioButton.IsChecked = !outdoorRadioButton.IsChecked;
        }

        private void indoorRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            outdoorRadioButton.IsChecked = !indoorRadioButton.IsChecked;
        }


        private void EnableDates(List<DateTime> dates)
        {
            datePicker.DisplayDateStart = dates.FirstOrDefault();
        }
    }
}
