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
using Data;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for AddGuest.xaml
    /// </summary>
    public partial class GuestWindow : Window
    {
        public GuestWindow()
        {
            InitializeComponent();
        }

        public String NameProp
        {
            get
            {
                return NameBox.Text;
            }
        }

        public void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            Guest newGuest = new Guest();

            try
            {
                newGuest.Name = NameBox.Text;
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }

            try
            {

                int age;

                if (!int.TryParse(AgeBox.Text, out age))
                {
                    AgeBox.Text = string.Empty;
                    throw new ArgumentException("Please enter a numeric Matriculation number.");
                }

                newGuest.Age = age;
                
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }

            try
            {
                newGuest.PassportNum = PassportBox.Text;
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }

            Booking Instance = Booking.GetInstance();
            Instance.AddGuest(newGuest);
            this.Close();
        }

        private void QuitBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close() ;
        }
    }
}
