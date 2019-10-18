using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business;
using System.Data.SqlClient;
using System.Collections.ObjectModel;

namespace Data
{
    public class DataFacade
    {
        //Singleton
        private static DataFacade instance;

        private DataFacade() { }

        public static DataFacade GetInstance()
        {
            if (instance == null)
            {
                instance = new DataFacade();
            }
            return instance;
        }
        //Singleton done

        private DBConnection db = DBConnection.Connection();
        
        public Booking FindBooking(int Booking_ref)
        {
            Booking booking = db.FindBooking(Booking_ref);

            return booking;           
        }

        public void UpdateBooking(Booking booking)
        {
            db.UpdateBooking(booking);
        }

        public void AddBooking(Booking booking)
        {
            db.AddBooking(booking);
        }

        public void DeleteBooking(int booking_ref)
        {
            db.DeleteBooking(booking_ref);
        }

        public Customer FindCustomer(int customerNum)
        {
            Customer cus = db.FindCustomer(customerNum);
            return cus;
        }

        public void UpdateCustomer(Customer customer)
        {
            db.UpdateCustomer(customer);
        }

        public void AddCustomer(Customer customer)
        {
            db.AddCustomer(customer);
        }

        public void DeleteCustomer(int customerNum)
        {
            db.DeleteCustomer(customerNum);
        }

        public ObservableCollection<int> GetCustomerBookingsList(int cusNum)
        {
            ObservableCollection<int> list = db.getCustomerBookingsList(cusNum);

            return list; 
        }

        public ObservableCollection<Customer> GetCustomerList()
        {
            ObservableCollection<Customer> cusList = db.GetCustomerList();
            return cusList;
        }

        public int GetLastCustomerNum()
        {
            int lastCusNum = db.GetLastCusNum();

            return lastCusNum;
        }

        public int GetLastBookingRef()
        {
            int lastBookingRef = db.GetLastBookingRef();

            return lastBookingRef;
        }

        public void DeleteGuest(int guestRef)
        {
            db.DeleteGuest(guestRef);
        }

        public void DeleteCarHire(int carHireRef)
        {
            db.DeleteCarHire(carHireRef);
        }
        
    }
}
