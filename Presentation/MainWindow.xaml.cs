using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using Business;
using Data;
using System.Collections.ObjectModel;

namespace Presentation
{
    /// <summary>
    /// Author Erik Sanne
    /// Last modified: 2017/11/12
    /// Description:
    ///
    /// Logic for interaction with the MainWindow,
    /// Creation of objects and interaction with the DataLayer to save/retrieve Data
    ///</summary>
    public partial class MainWindow : Window
    {
        DataFacade db = DataFacade.GetInstance(); //Get access to the DataBase Facade
        Booking bookingInstance = Booking.GetInstance(); //Get access to the booking instance, singleton as only one booking should be active at a time.
        ObservableCollection<Customer> customerList = new ObservableCollection<Customer>(); //Collection that is displayed on the CustomerPage to present and easy selector when using previous customer's
        ObservableCollection<int> chaletList = new ObservableCollection<int>(); //Collecction that presents the available chalets, availability implementation is missing.
        List<int> guestToDelete = new List<int>(); //Saving a list of guests to be deleted as this will be done after the submit button is clicked.
        List<int> carHireToDelete = new List<int>(); //Saving a list of CarHires to be deleted as this will be done after the submit button is clicked.
        ObservableCollection<int> customerBookingsList = new ObservableCollection<int>(); //Customerbooking list is used to present the Booking related to the selected customer in the Customerpage
        
        int lastCusNum = 0; //Last used customer num, used for auto increment and will be updated at program start and whenever a customer is added
        int lastBookingRef = 0; //Last used Booking reference, used for auto increment and will be updated at program start and whenever a booking is added
        bool existingCus; //Boolean to save wether the Last customer number should be incremented when submitting the customer page or not
        
        //Initilising MainWindow
        public MainWindow()
        {
            InitializeComponent();

            customerList = db.GetCustomerList(); //Retrieve the latest customer list from the Database
            lastCusNum = db.GetLastCustomerNum(); //Retrieve the latest customer number from the Database
            lastBookingRef = db.GetLastBookingRef(); //Retrieve the latest Booking reference from the Database

            CusListBox.ItemsSource = customerList; //Setting the item source for the Customer display (Updates whenever customerList is changed
            GuestListBox.ItemsSource = bookingInstance.GuestList; //Item source for 
            CarHireListBox.ItemsSource = bookingInstance.CarHireList;
            CusBookingsListBox.ItemsSource = customerBookingsList;
            
            for(int i = 1; i < 11; i++)
            {
                chaletList.Add(i);
            }
            ChaletListBox.ItemsSource = chaletList;

            BackBtn.Visibility = Visibility.Hidden;
            StartPage.Visibility = Visibility.Visible;
            BookingPage.Visibility = Visibility.Hidden;
            CustomerPage.Visibility = Visibility.Hidden;
        }

        //Startpage code

        int caseSwitch; //1 - Add booking, 2 - Edit Booking, 3 - View Booking, 4 - Add Customer, 5 - Edit customer, 6 - View Customer, 7 - Edit Bookin from Viewing Customer, 8 - View Booking from Viewing Customer
               

        private void AddBookingBtn_Click(object sender, RoutedEventArgs e)
        {
            caseSwitch = 1;

            ViewInvoiceBtn.Visibility = Visibility.Hidden;
            DeleteBookingBtn.Visibility = Visibility.Hidden;
            CusViewBookingBtn.Visibility = Visibility.Hidden;
            CusEditBookingBtn.Visibility = Visibility.Hidden;
            CusDeleteBtn.Visibility = Visibility.Hidden;
            BackBtn.Visibility = Visibility.Visible;
            CusFindBtn.Visibility = Visibility.Visible;
            CusListBox.IsEnabled = true;
            ChaletListBox.IsEnabled = true;
            DeleteGuestBtn.Visibility = Visibility.Visible;
            DeleteCarHireBtn.Visibility = Visibility.Visible;

            StartPage.Visibility = Visibility.Hidden;
            CustomerPage.Visibility = Visibility.Visible;                    

            BookingRefBox.Text = (lastBookingRef + 1).ToString();

        }              

        private void EditBookingBtn_Click(object sender, RoutedEventArgs e)
        {
            int bookingRef;
            int result;

            if (CustomerPage.Visibility == Visibility.Visible)
            {
                caseSwitch = 7;

                bookingRef = (int)CusBookingsListBox.SelectedItem;

                result = FindAndSetBookingInfo(bookingRef);

                if (result == -1)
                {
                    return;
                }

                BackBtn.Visibility = Visibility.Visible;
                DeleteBookingBtn.Visibility = Visibility.Visible;
                ViewInvoiceBtn.Visibility = Visibility.Visible;
                ChaletListBox.IsEnabled = true;
                DeleteGuestBtn.Visibility = Visibility.Visible;
                DeleteCarHireBtn.Visibility = Visibility.Visible;

                CustomerPage.Visibility = Visibility.Hidden;
                BookingPage.Visibility = Visibility.Visible;

                return;
            }

            caseSwitch = 2;

            if (string.IsNullOrEmpty(StartBookingRefBox.Text))
            {
                MessageBox.Show("Please enter a Booking reference!");
                return;
            }

            bookingRef = Convert.ToInt32(StartBookingRefBox.Text);

            result = FindAndSetBookingInfo(bookingRef);

            if (result == -1)
            {
                return;
            }

            BackBtn.Visibility = Visibility.Visible;
            DeleteBookingBtn.Visibility = Visibility.Visible;
            ViewInvoiceBtn.Visibility = Visibility.Visible;
            CusDeleteBtn.Visibility = Visibility.Hidden;
            DeleteGuestBtn.Visibility = Visibility.Visible;
            DeleteCarHireBtn.Visibility = Visibility.Visible;
            ChaletListBox.IsEnabled = true;
            CusListBox.IsEnabled = true;
            CusNumBox.IsReadOnly = false;
            CusNameBox.IsReadOnly = false;
            CusAddressBox.IsReadOnly = false;

            StartPage.Visibility = Visibility.Hidden;
            BookingPage.Visibility = Visibility.Visible;
        }
        
        private void ViewBookingBtn_Click(object sender, RoutedEventArgs e)
        {
            int bookingRef;
            int result;

            if (CustomerPage.Visibility == Visibility.Visible)
            {
                caseSwitch = 8;

                bookingRef = (int)CusBookingsListBox.SelectedItem;

                result = FindAndSetBookingInfo(bookingRef);

                if (result == -1)
                {
                    return;
                }

                BreakfastCheck.IsHitTestVisible = false;
                EveningCheck.IsHitTestVisible = false;
                StartDatePicker.Focusable = false;
                EndDatePicker.Focusable = false;
                ChaletListBox.IsEnabled = false;
                DeleteGuestBtn.Visibility = Visibility.Hidden;
                DeleteCarHireBtn.Visibility = Visibility.Hidden;
                AddGuestBtn.Visibility = Visibility.Hidden;
                AddCarHireBtn.Visibility = Visibility.Hidden;
                BackBtn.Visibility = Visibility.Visible;
                DeleteBookingBtn.Visibility = Visibility.Hidden;
                ViewInvoiceBtn.Visibility = Visibility.Visible;

                CustomerPage.Visibility = Visibility.Hidden;
                BookingPage.Visibility = Visibility.Visible;
                return;
            }


            caseSwitch = 3;

            if (string.IsNullOrEmpty(StartBookingRefBox.Text))
            {
                MessageBox.Show("Please enter a Booking reference!");
                return;
            }

            bookingRef = Convert.ToInt32(StartBookingRefBox.Text);
            
            result = FindAndSetBookingInfo(bookingRef);

            if (result == -1)
            {
                return;
            }

            BreakfastCheck.IsHitTestVisible = false;
            EveningCheck.IsHitTestVisible = false;
            StartDatePicker.Focusable = false;
            EndDatePicker.Focusable = false;
            AddGuestBtn.Visibility = Visibility.Hidden;
            AddCarHireBtn.Visibility = Visibility.Hidden;
            BackBtn.Visibility = Visibility.Visible;
            DeleteBookingBtn.Visibility = Visibility.Hidden;
            ViewInvoiceBtn.Visibility = Visibility.Visible;

            StartPage.Visibility = Visibility.Hidden;
            BookingPage.Visibility = Visibility.Visible;                  

        }

        private void AddCustomerBtn_Click(object sender, RoutedEventArgs e)
        {
            caseSwitch = 4;

            CusNumBox.Text = (lastCusNum + 1).ToString();
            CusNumBox.IsReadOnly = true;

            BackBtn.Visibility = Visibility.Visible;
            CusDeleteBtn.Visibility = Visibility.Hidden;
            CusViewBookingBtn.Visibility = Visibility.Hidden;
            CusEditBookingBtn.Visibility = Visibility.Hidden;
            CusBookingsListBox.Visibility = Visibility.Hidden;
            CusFindBtn.Visibility = Visibility.Hidden;
            CusListBox.IsEnabled = false;

            StartPage.Visibility = Visibility.Hidden;
            CustomerPage.Visibility = Visibility.Visible;

        }

        private void EditCustomerBtn_Click(object sender, RoutedEventArgs e)
        {
            caseSwitch = 5;

            if (string.IsNullOrEmpty(StartCustomerNumBox.Text))
            {
                MessageBox.Show("Please enter a Customer number!");
                return;
            }

            int customerNum = Convert.ToInt32(StartCustomerNumBox.Text);

            int result = FindAndSetCustomerInfo(customerNum);

            if(result == -1)
            {
                return;
            }

            BackBtn.Visibility = Visibility.Visible;
            CusDeleteBtn.Visibility = Visibility.Visible;
            CusViewBookingBtn.Visibility = Visibility.Hidden;
            CusEditBookingBtn.Visibility = Visibility.Hidden;
            CusBookingsListBox.Visibility = Visibility.Visible;
            CusFindBtn.Visibility = Visibility.Hidden;
            CusListBox.IsEnabled = false;
            CusNumBox.IsReadOnly = true;
            CusNameBox.IsReadOnly = false;
            CusAddressBox.IsReadOnly = false;

            StartPage.Visibility = Visibility.Hidden;
            CustomerPage.Visibility = Visibility.Visible;


        }

        private void ViewCustomerBtn_Click(object sender, RoutedEventArgs e)
        {
            caseSwitch = 6;

            if (string.IsNullOrEmpty(StartCustomerNumBox.Text))
            {
                MessageBox.Show("Please enter a Customer number!");
                return;
            }

            int customerNum = Convert.ToInt32(StartCustomerNumBox.Text);

            int result = FindAndSetCustomerInfo(customerNum);

            if (result == -1)
            {
                return;
            }

            customerBookingsList = db.GetCustomerBookingsList(customerNum);
            CusBookingsListBox.ItemsSource = customerBookingsList;

            BackBtn.Visibility = Visibility.Visible;
            CusViewBookingBtn.Visibility = Visibility.Visible;
            CusEditBookingBtn.Visibility = Visibility.Visible;
            CusBookingsListBox.Visibility = Visibility.Visible;
            CusFindBtn.Visibility = Visibility.Visible;
            CusListBox.IsEnabled = true;
            CusSubmitBtn.Visibility = Visibility.Hidden;
            CusNumBox.IsReadOnly = true;
            CusNameBox.IsReadOnly = true;
            CusAddressBox.IsReadOnly = true;


            StartPage.Visibility = Visibility.Hidden;
            CustomerPage.Visibility = Visibility.Visible;
            

        }

        //Bookingpage code
        private void BookingSubmitBtn_Click(object sender, RoutedEventArgs e)
        {

            //Setting booking ref
            try
            {
                bookingInstance.BookingRef = Convert.ToInt32(BookingRefBox.Text);
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }

            //Setting Customer_num
            try
            {
                //The customer num is generated/determined in the previous step and then saved to the CustomerNumberBox
                bookingInstance.CustomerNum = Convert.ToInt32(CustomerNumberBox.Text);
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }

            //Start/Arrival Date
            try
            {
                bookingInstance.ArrivalDate = StartDatePicker.SelectedDate.GetValueOrDefault();

            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }

            //End/Departure Date
            try
            {
                bookingInstance.DepartureDate = EndDatePicker.SelectedDate.GetValueOrDefault();

            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }

            //Breakfast Meals
            try
            {
                if (BreakfastCheck.IsChecked.HasValue && BreakfastCheck.IsChecked.Value)
                {
                    bookingInstance.BreakfastMeals = true;
                }
                else
                {
                    bookingInstance.BreakfastMeals = false;
                }
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }

            //Evening Meals
            try
            {
                if (EveningCheck.IsChecked.HasValue && EveningCheck.IsChecked.Value)
                {
                    bookingInstance.EveningMeals = true;
                }
                else
                {
                    bookingInstance.EveningMeals = false;
                }
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }

            //Setting ChaletID
            try
            {
                bookingInstance.ChaletID = ChaletListBox.SelectedIndex;
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return;
            }

            //1 - Add new booking, 2 - Update existing booking, determined during button click on startpage
            switch (caseSwitch)
            {
                case 1:
                    db.AddBooking(bookingInstance);
                    break;
                case 2:
                    foreach(int guestRef in guestToDelete)
                    {
                        db.DeleteGuest(guestRef);
                    }

                    foreach (int carHireRef in carHireToDelete)
                    {
                        db.DeleteCarHire(carHireRef);
                    }

                    db.UpdateBooking(bookingInstance);

                    break;
            }

            lastBookingRef += 1; //Update last booking ref 
            bookingInstance.ClearBookingLists(); //Clear guest and car hire list so the instance can be used again

            //Reset UI to startpage
            BookingPage.Visibility = Visibility.Hidden;
            StartPage.Visibility = Visibility.Visible;
            
        }
        
        //Add guest to the current booking
        private void AddGuestBtn_Click(object sender, RoutedEventArgs e)
        {
            if(bookingInstance.GuestList.Count >= 6)
            {
                MessageBox.Show("Maximun number(6) of guests reached!");
                return;
            }

            GuestWindow win = new GuestWindow();
            win.ShowDialog();

            win.NameBox.IsReadOnly = false;
            win.AgeBox.IsReadOnly = false;
            win.PassportBox.IsReadOnly = false;

            //Any guest related logic is handled in the GuesWindow class
        }

        //Add a car hire to the current booking
        private void AddCarHireBtn_Click(object sender, RoutedEventArgs e)
        {
            CarHireWindow win = new CarHireWindow();

            win.StartDatePicker.Focusable = true;
            win.EndDatePicker.Focusable = true;
            win.DriverBox.IsReadOnly = false;

            win.ShowDialog();
            //Any car hire related logic is handled in the CarHireWindow class

        }

        //Display invoice for the current booking
        private void ViewInvoiceBtn_Click(object sender, RoutedEventArgs e)
        {
            InvoiceWindow win = new InvoiceWindow();
            win.ShowDialog();
        }

        //Delete the current booking
        private void DeleteBookingBtn_Click(object sender, RoutedEventArgs e)
        {
            db.DeleteBooking(bookingInstance.BookingRef);
            BookingPage.Visibility = Visibility.Hidden;
            StartPage.Visibility = Visibility.Visible;
            
        }

        //Delete Guest from the current booking
        private void DeleteGuestBtn_Click(object sender, RoutedEventArgs e)
        {
            Guest guest = GuestListBox.SelectedItem as Guest;

            switch (caseSwitch)
            {
                case 1:
                    bookingInstance.GuestList.Remove(guest);
                    break;
                case 2:
                    bookingInstance.GuestList.Remove(guest);
                    carHireToDelete.Add(guest.GuestRef);
                    break;
            }

            guestToDelete.Add(guest.GuestRef);

        }

        //Delete Car Hire from the current booking
        private void DeleteCarHireBtn_Click(object sender, RoutedEventArgs e)
        {
            CarHire hire = CarHireListBox.SelectedItem as CarHire;

            switch (caseSwitch)
            {
                case 1:
                    bookingInstance.CarHireList.Remove(hire);
                    break;
                case 2:

                    bookingInstance.CarHireList.Remove(hire);
                    carHireToDelete.Add(hire.CarHireRef);
                    break;
            }

        }

        //View the currently selected Guest
        private void ViewGuestBtn_Click(object sender, RoutedEventArgs e)
        {
            Guest guest = GuestListBox.SelectedItem as Guest;
            GuestWindow win = new GuestWindow();

            win.NameBox.Text = guest.Name;
            win.AgeBox.Text = guest.Age.ToString();
            win.PassportBox.Text = guest.PassportNum.ToString();

            win.NameBox.IsReadOnly = true;
            win.AgeBox.IsReadOnly = true;
            win.PassportBox.IsReadOnly = true;

            win.ShowDialog();

        }

        //View the currently selected Car_Hire
        private void ViewCarHireBtn_Click(object sender, RoutedEventArgs e)
        {
            CarHire hire = CarHireListBox.SelectedItem as CarHire;
            CarHireWindow win = new CarHireWindow();

            win.StartDatePicker.DisplayDate = hire.StartDate;
            win.EndDatePicker.DisplayDate = hire.EndDate;
            win.DriverBox.Text = hire.Driver;

            win.StartDatePicker.Focusable = false;
            win.EndDatePicker.Focusable = false;
            win.DriverBox.IsReadOnly = true;

        }

        //Done bookingpage code

        //Customerpage code

        //Submit the customer page
        //Depending on the case (see caseSwtich declaration) it will add or update the customer
        //For the cases not listed it is not visible
        private void CusSubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            Customer cus = new Customer();
            switch (caseSwitch)
            {
                case 1:
                                        
                    if (existingCus)
                    {
                        CustomerPage.Visibility = Visibility.Hidden;
                        BookingPage.Visibility = Visibility.Visible;
                        CustomerNumberBox.Text = CusNumBox.Text;
                        BookingRefBox.Text = (lastBookingRef + 1).ToString();
                        break;
                    }

                    SetCustomerObjectData(cus);
                    db.AddCustomer(cus);
                    lastCusNum += 1;

                    CustomerNumberBox.Text = lastCusNum.ToString();
                    BookingRefBox.Text = (lastBookingRef + 1).ToString();

                    CustomerPage.Visibility = Visibility.Hidden;
                    BookingPage.Visibility = Visibility.Visible;

                    break;

                case 4:

                    cus = SetCustomerObjectData(cus);

                    db.AddCustomer(cus);
                    
                    customerList.Add(cus);

                    lastCusNum += 1;

                    CustomerPage.Visibility = Visibility.Hidden;
                    StartPage.Visibility = Visibility.Visible;

                    break;

                case 5:

                    cus = SetCustomerObjectData(cus);

                    db.UpdateCustomer(cus);

                    CustomerPage.Visibility = Visibility.Hidden;
                    StartPage.Visibility = Visibility.Visible;

                    break;
            }
        }


        private void CusDeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CusNumBox.Text))
            {
                MessageBox.Show("Please enter a Customer number.");
                return;
            }

            int cusNum = Convert.ToInt32(CusNumBox.Text);

            try
            {
                db.FindCustomer(cusNum);
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
            }                     

            db.DeleteCustomer(cusNum);
        }

        private void CusListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Customer cus = CusListBox.SelectedItem as Customer;

            try
            {
                customerBookingsList = db.GetCustomerBookingsList(cus.CustomerNum);
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
            }

            CusBookingsListBox.ItemsSource = customerBookingsList;
            int result = FindAndSetCustomerInfo(cus.CustomerNum);

            if (result == -1)
            {
                return;
            }

            existingCus = true;
            
            
        }

        private void CusFindBtn_Click(object sender, RoutedEventArgs e)
        {
            int cusNum = Convert.ToInt32(CusNumBox.Text);

            int result = FindAndSetCustomerInfo(cusNum);

            if (result == -1)
            {
                return;
            }

        }

        private void CusNumBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            existingCus = false;
        }
        private void CusNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            existingCus = false;
        }

        private void CusAddressBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            existingCus = false;
        }

        //Customerpage done

        private int FindAndSetBookingInfo(int bookingRef)
        {
            try
            {
                bookingInstance = db.FindBooking(bookingRef);
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return -1;
            }

            CustomerNumberBox.Text = bookingInstance.CustomerNum.ToString();
            BookingRefBox.Text = bookingInstance.BookingRef.ToString();
            StartDatePicker.SelectedDate = bookingInstance.ArrivalDate;
            EndDatePicker.SelectedDate = bookingInstance.DepartureDate;
            ChaletListBox.SelectedIndex = bookingInstance.ChaletID;

            if (bookingInstance.BreakfastMeals)
                BreakfastCheck.IsChecked = true;

            if (bookingInstance.EveningMeals)
                EveningCheck.IsChecked = true;

            //Resetting the ItemsSource for Listboxes within the bookingpages
            //This forces them to update and display the associated guests and car hires
            CarHireListBox.ItemsSource = bookingInstance.CarHireList;
            GuestListBox.ItemsSource = bookingInstance.GuestList;

            return 0;
        }

        private int FindAndSetCustomerInfo(int customerNum)
        {
            Customer cus = new Customer();

            try
            {
                cus = db.FindCustomer(customerNum);
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return -1;
            }

            CusNumBox.Text = cus.CustomerNum.ToString();
            CusNameBox.Text = cus.Name;
            CusAddressBox.Text = cus.Address;

            return 0;
        }

        private Customer SetCustomerObjectData(Customer cus)
        {

            try
            {
                cus.CustomerNum = Convert.ToInt32(CusNumBox.Text);
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return null;
            }

            try
            {
                cus.Name = CusNameBox.Text;
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return null;
            }

            try
            {
                cus.Address = CusAddressBox.Text;
            }
            catch (Exception excep)
            {
                MessageBox.Show(excep.Message);
                return null;
            }

            return cus;
        }

        private void BackBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerPage.Visibility == Visibility.Visible)
            {
                CusNumBox.Text = string.Empty;
                CusNameBox.Text = string.Empty;
                CusAddressBox.Text = string.Empty;

                CustomerPage.Visibility = Visibility.Hidden;
                StartPage.Visibility = Visibility.Visible;
            }

            if (BookingPage.Visibility == Visibility.Visible)
            {
                BookingRefBox.Text = string.Empty;
                CustomerNumberBox.Text = string.Empty;
                StartDatePicker.SelectedDate = null;
                EndDatePicker.SelectedDate = null;
                EveningCheck.IsChecked = false;
                BreakfastCheck.IsChecked = false;

                BreakfastCheck.IsHitTestVisible = true;
                EveningCheck.IsHitTestVisible = true;
                StartDatePicker.Focusable = true;
                EndDatePicker.Focusable = true;
                AddGuestBtn.Visibility = Visibility.Visible;
                AddCarHireBtn.Visibility = Visibility.Visible;

                if(caseSwitch == 7 || caseSwitch == 8)
                {
                    BookingPage.Visibility = Visibility.Hidden;
                    CustomerPage.Visibility = Visibility.Visible;
                }

                if (caseSwitch == 2 || caseSwitch == 3)
                {
                    BookingPage.Visibility = Visibility.Hidden;
                    StartPage.Visibility = Visibility.Visible;
                    return;
                }

                BookingPage.Visibility = Visibility.Hidden;
                CustomerPage.Visibility = Visibility.Visible;
            }

        }

        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(1);
        }

    }
}
