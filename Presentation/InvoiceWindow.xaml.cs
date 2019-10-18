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
    /// Interaction logic for InvoiceWindow.xaml
    /// </summary>
    public partial class InvoiceWindow : Window
    {
        Booking bookingInstance = Booking.GetInstance();

        public InvoiceWindow()
        {
            InitializeComponent();

            BookingRefBox.Text = bookingInstance.BookingRef.ToString();
            ChaletCostBox.Text = bookingInstance.ChaletCost.ToString();
            BreakfastCostBox.Text = bookingInstance.BreakFastMealsCost.ToString();
            EveningMealsCostBox.Text = bookingInstance.EveningMealsCost.ToString();
            CarHireCostBox.Text = bookingInstance.CarHireCost.ToString();
            TotalCostBox.Text = bookingInstance.TotalCost.ToString();
            
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
