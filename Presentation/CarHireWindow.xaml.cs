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
using System.Windows.Shapes;
using Business;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for CarHire.xaml
    /// </summary>
    public partial class CarHireWindow : Window
    {
        public CarHireWindow()
        {
            InitializeComponent();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            CarHire newHire = new CarHire();
            Booking instance = Booking.GetInstance();

            try
            {
                newHire.Driver = DriverBox.Text;
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }

            try
            {
                newHire.StartDate = StartDatePicker.SelectedDate.GetValueOrDefault();
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }

            try
            {
                newHire.EndDate = EndDatePicker.SelectedDate.GetValueOrDefault();
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }

            instance.AddCarHire(newHire);
            this.Close();

        }

        private void QuitBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(1);
        }
    }
}
