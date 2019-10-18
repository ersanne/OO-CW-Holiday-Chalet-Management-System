using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Business;
using System.Collections.ObjectModel;

namespace Data
{
    class DBConnection
    {
        private static DBConnection connection;

        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\Erik\OneDrive - Edinburgh Napier University\Semester 3\Object Oriented Software Development\oocoursework2\napierchalet.mdf';Integrated Security = True; Connect Timeout = 30";

        private DBConnection() { }

        public static DBConnection Connection()
        {
                if (connection == null)
                {
                    connection = new DBConnection();
                }
                return connection;
        }

        //Get the last customer num assigned
        public int GetLastCusNum()
        {
            int cusNum = 0;

            SqlConnection napierchalet = new SqlConnection(connectionString);

            using (napierchalet)
            {
                string query = "SELECT MAX(Customer_num) AS max FROM Customers";

                SqlCommand cmd = new SqlCommand(query, napierchalet);

                napierchalet.Open();

                var result = cmd.ExecuteScalar();

                if (result == DBNull.Value)
                {
                    return cusNum;
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    
                    if (!reader.HasRows)
                    {
                        return cusNum;
                    }  

                    while (reader.Read())
                    {
                        cusNum = Convert.ToInt32(reader["max"]);
                    }

                }

                napierchalet.Close();
            }

            return cusNum;
        }

        public int GetLastBookingRef()
        {
            int bookingRef = 0;

            SqlConnection napierchalet = new SqlConnection(connectionString);

            using (napierchalet)
            {
                string query = "SELECT MAX(Booking_ref) AS max FROM Bookings";

                SqlCommand cmd = new SqlCommand(query, napierchalet);

                napierchalet.Open();

                var result = cmd.ExecuteScalar();

                if (result == DBNull.Value)
                {
                    return bookingRef;
                }

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        return bookingRef;
                    }

                    while (reader.Read())
                    {
                        bookingRef = Convert.ToInt32(reader["max"]);
                    }
                                        
                }
                
                napierchalet.Close();
            }
            
            return bookingRef;
        }

        public Booking FindBooking(int bookingRef)
        {
            Booking booking = Booking.GetInstance();
            ObservableCollection<Guest> guestList = new ObservableCollection<Guest>();
            ObservableCollection<CarHire> carHireList = new ObservableCollection<CarHire>();

            SqlConnection napierchalet = new SqlConnection(connectionString);

            using (napierchalet)
            {

                string bookingQuery = "SELECT * FROM Bookings WHERE Booking_ref=@bookingRef";
                string guestQuery = "SELECT * FROM Guests WHERE Booking_ref=@bookingRef";
                string carHireQuery = "SELECT * FROM Car_hires WHERE Booking_ref=@bookingRef";

                SqlCommand bookingCmd = new SqlCommand(bookingQuery, napierchalet);
                bookingCmd.Parameters.AddWithValue("@bookingRef", bookingRef);
                SqlCommand guestCmd = new SqlCommand(guestQuery, napierchalet);
                guestCmd.Parameters.AddWithValue("@bookingRef", bookingRef);
                SqlCommand carHireCmd = new SqlCommand(carHireQuery, napierchalet);
                carHireCmd.Parameters.AddWithValue("@bookingRef", bookingRef);

                napierchalet.Open();

                using (SqlDataReader bookingReader = bookingCmd.ExecuteReader())
                {
                    if (!bookingReader.HasRows)
                    {
                        throw new ArgumentException("Booking not found!");
                    }

                    while (bookingReader.Read())
                    {
                        booking.BookingRef = Convert.ToInt32(bookingReader["Booking_ref"].ToString());
                        booking.CustomerNum = Convert.ToInt32(bookingReader["Customer_num"].ToString());
                        booking.ChaletID = Convert.ToInt32(bookingReader["Chalet_id"].ToString());
                        booking.ArrivalDate = Convert.ToDateTime(bookingReader["Start_date"].ToString());
                        booking.DepartureDate = Convert.ToDateTime(bookingReader["End_date"].ToString());
                        booking.EveningMeals = Convert.ToBoolean(bookingReader["Evening_meals"]);
                        booking.BreakfastMeals = Convert.ToBoolean(bookingReader["Breakfast_meals"]);
                    }
                }
                
                using(SqlDataReader guestReader = guestCmd.ExecuteReader())
                {
                    if (guestReader.HasRows)
                    {
                        while (guestReader.Read())
                        {
                            Guest newGuest = new Guest();
                                                        
                            newGuest.GuestRef = Convert.ToInt32(guestReader["Guest_ref"].ToString());
                            newGuest.Name = guestReader["Name"].ToString();
                            newGuest.PassportNum = guestReader["Passport_num"].ToString();
                            newGuest.Age = Convert.ToInt32(guestReader["Age"].ToString());

                            guestList.Add(newGuest);
                        }
                    }
                }                      
                        
                booking.GuestList = guestList;

                using (SqlDataReader carHireReader = carHireCmd.ExecuteReader())
                {
                    if (carHireReader.HasRows)
                    {
                        while (carHireReader.Read())
                        {
                            CarHire newHire = new CarHire();
                            newHire.CarHireRef = Convert.ToInt32(carHireReader["Car_hire_ref"].ToString());
                            newHire.StartDate = Convert.ToDateTime(carHireReader["Start_date"].ToString());
                            newHire.EndDate = Convert.ToDateTime(carHireReader["End_date"].ToString());
                            newHire.Driver = carHireReader["Driver"].ToString();                            
                            carHireList.Add(newHire);
                        }
                    }
                }
                
                booking.CarHireList = carHireList;
                
                napierchalet.Close();
            }

            return booking;
        }

        public void AddBooking(Booking booking)
        {

            SqlConnection napierchalet = new SqlConnection(connectionString);

            using (napierchalet)
            {
                string bookingQuery = "INSERT INTO Bookings (Customer_num, Chalet_id, Start_date, End_date, Evening_meals, Breakfast_meals)";
                bookingQuery += " VALUES (@Customer_num, @Chalet_id, @Start_date, @End_date, @Evening_meals, @Breakfast_meals)";

                SqlCommand bookingCmd = new SqlCommand(bookingQuery, napierchalet);

                napierchalet.Open();

                try
                {
                    bookingCmd.Parameters.AddWithValue("@Customer_num", booking.CustomerNum);
                    bookingCmd.Parameters.AddWithValue("@Chalet_id", booking.ChaletID);
                    bookingCmd.Parameters.AddWithValue("@Start_date", booking.ArrivalDate);
                    bookingCmd.Parameters.AddWithValue("@End_date", booking.DepartureDate);
                    bookingCmd.Parameters.AddWithValue("@Evening_meals", booking.EveningMeals);
                    bookingCmd.Parameters.AddWithValue("@Breakfast_meals", booking.BreakfastMeals);
                    bookingCmd.ExecuteNonQuery();
                }
                catch (Exception excep)
                {
                    Console.WriteLine(excep.Message);
                }

                ObservableCollection<Guest> guests = booking.GuestList;

                string guestQuery = "INSERT INTO Guests (Booking_ref, Name, Passport_num, Age)";
                guestQuery += " VALUES (@Booking_ref, @Name, @Passport_num, @Age)";
                SqlCommand guestCmd = new SqlCommand(guestQuery, napierchalet);

                foreach (Guest guest in guests)
                {
                    try
                    {
                        guestCmd.Parameters.Clear();
                        guestCmd.Parameters.AddWithValue("@Booking_ref", booking.BookingRef);
                        guestCmd.Parameters.AddWithValue("@Name", guest.Name);
                        guestCmd.Parameters.AddWithValue("@Passport_num", guest.PassportNum);
                        guestCmd.Parameters.AddWithValue("@Age", guest.Age);
                        guestCmd.ExecuteNonQuery();
                    }
                    catch (Exception excep)
                    {
                        Console.WriteLine(excep.Message);
                    }
                }

                ObservableCollection<CarHire> carHires = booking.CarHireList;

                string carHireQuery = "INSERT INTO Car_hires (Booking_ref, Start_date, End_date, Driver)";
                carHireQuery += " VALUES (@Booking_ref, @Start_date, @End_date, @Driver)";
                SqlCommand carHireCmd = new SqlCommand(carHireQuery, napierchalet);

                foreach (CarHire carHire in carHires)
                {
                    try
                    {

                        carHireCmd.Parameters.Clear();
                        carHireCmd.Parameters.AddWithValue("@Booking_ref", booking.BookingRef);
                        carHireCmd.Parameters.AddWithValue("@Start_date", carHire.StartDate);
                        carHireCmd.Parameters.AddWithValue("@End_date", carHire.EndDate);
                        carHireCmd.Parameters.AddWithValue("@Driver", carHire.Driver);
                        carHireCmd.ExecuteNonQuery();
                    }
                    catch (Exception excep)
                    {
                        Console.WriteLine(excep.Message);
                    }
                }

                napierchalet.Close();
            }

        }

        public void UpdateBooking(Booking booking)
        {
                  
            SqlConnection napierchalet = new SqlConnection(connectionString);
            
            using (napierchalet)
            {
                string bookingQuery = "UPDATE Bookings SET Customer_num = @Customer_num, Chalet_id = @Chalet_id, Start_date = @Start_date,";
                bookingQuery += " End_date =  @End_date, Evening_meals = @Evening_meals, Breakfast_meals = @Breakfast_meals";
                bookingQuery += " WHERE Booking_ref = @Booking_ref";
                
                SqlCommand bookingCmd = new SqlCommand(bookingQuery, napierchalet);

                napierchalet.Open();

                try
                {
                    bookingCmd.Parameters.AddWithValue("@Booking_ref", booking.BookingRef);
                    bookingCmd.Parameters.AddWithValue("@Customer_num", booking.CustomerNum);
                    bookingCmd.Parameters.AddWithValue("@Chalet_id", booking.ChaletID);
                    bookingCmd.Parameters.AddWithValue("@Start_date", booking.ArrivalDate);
                    bookingCmd.Parameters.AddWithValue("@End_date", booking.DepartureDate);
                    bookingCmd.Parameters.AddWithValue("@Evening_meals", booking.EveningMeals);
                    bookingCmd.Parameters.AddWithValue("@Breakfast_meals", booking.BreakfastMeals);
                    bookingCmd.ExecuteNonQuery();
                }
                catch (Exception excep)
                {
                    Console.WriteLine(excep.Message);
                }


                ObservableCollection<Guest> guests = booking.GuestList;

                string guestQuery = "UPDATE Guests SET Name = @Name, Passport_num = @Passport_num, Age = @Age";
                guestQuery += " WHERE Booking_ref = @Booking_ref AND Guest_ref = @Guest_ref";

                string guestCheckQuery = "SELECT COUNT(*) FROM Guests WHERE Guest_ref = @Guest_ref";

                string addGuestQuery = "INSERT INTO Guests (Booking_ref, Name, Passport_num, Age)";
                addGuestQuery += " VALUES (@Booking_ref, @Name, @Passport_num, @Age)";

                SqlCommand guestCmd = new SqlCommand(guestQuery, napierchalet);
                SqlCommand guestCheckCmd = new SqlCommand(guestCheckQuery, napierchalet);
                SqlCommand addGuestCmd = new SqlCommand(addGuestQuery, napierchalet);

                foreach (Guest guest in guests)
                {

                    guestCheckCmd.Parameters.Clear();
                    guestCheckCmd.Parameters.AddWithValue("@Guest_ref", guest.GuestRef);

                    if((int)guestCheckCmd.ExecuteScalar() > 0)
                    {
                        try
                        {
                            guestCmd.Parameters.Clear();
                            guestCmd.Parameters.AddWithValue("@Guest_ref", guest.GuestRef);
                            guestCmd.Parameters.AddWithValue("@Booking_ref", booking.BookingRef);
                            guestCmd.Parameters.AddWithValue("@Name", guest.Name);
                            guestCmd.Parameters.AddWithValue("@Passport_num", guest.PassportNum);
                            guestCmd.Parameters.AddWithValue("@Age", guest.Age);
                            guestCmd.ExecuteNonQuery();
                        }
                        catch (Exception excep)
                        {
                            Console.WriteLine(excep.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            addGuestCmd.Parameters.Clear();
                            addGuestCmd.Parameters.AddWithValue("@Booking_ref", booking.BookingRef);
                            addGuestCmd.Parameters.AddWithValue("@Name", guest.Name);
                            addGuestCmd.Parameters.AddWithValue("@Passport_num", guest.PassportNum);
                            addGuestCmd.Parameters.AddWithValue("@Age", guest.Age);
                            addGuestCmd.ExecuteNonQuery();
                        }
                        catch (Exception excep)
                        {
                            Console.WriteLine(excep.Message);
                        }
                    }

                }

                ObservableCollection<CarHire> carHires = booking.CarHireList;

                string carHireQuery = "UPDATE Car_hires SET Start_date = @Start_date, End_date = @End_date, Driver = @Driver";
                carHireQuery += " WHERE Booking_ref = @Booking_ref AND Car_hire_ref = @Car_hire_ref";

                string carHireCheckQuery = "SELECT COUNT(*) FROM Car_hires WHERE Car_hire_ref = @Car_hire_ref";

                string addCarHireQuery = "INSERT INTO Car_hires (Booking_ref, Start_date, End_date, Driver)";
                addCarHireQuery += " VALUES (@Booking_ref, @Start_date, @End_date, @Driver)";

                SqlCommand carHireCmd = new SqlCommand(carHireQuery, napierchalet);
                SqlCommand carHireCheckCmd = new SqlCommand(carHireCheckQuery, napierchalet);
                SqlCommand addCarHireCmd = new SqlCommand(addCarHireQuery, napierchalet);

                foreach (CarHire carHire in carHires)
                {

                    carHireCheckCmd.Parameters.Clear();
                    carHireCheckCmd.Parameters.AddWithValue("@Car_hire_ref", carHire.CarHireRef);

                    if ((int)carHireCheckCmd.ExecuteScalar() > 0)
                    {
                        try
                        {
                            carHireCmd.Parameters.Clear();
                            carHireCmd.Parameters.AddWithValue("@Car_hire_ref", carHire.CarHireRef);
                            carHireCmd.Parameters.AddWithValue("@Booking_ref", booking.BookingRef);
                            carHireCmd.Parameters.AddWithValue("@Start_date", carHire.StartDate);
                            carHireCmd.Parameters.AddWithValue("@End_date", carHire.EndDate);
                            carHireCmd.Parameters.AddWithValue("@Driver", carHire.Driver);
                            carHireCmd.ExecuteNonQuery();
                        }
                        catch (Exception excep)
                        {
                            Console.WriteLine(excep.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            addCarHireCmd.Parameters.Clear();
                            addCarHireCmd.Parameters.AddWithValue("@Booking_ref", booking.BookingRef);
                            addCarHireCmd.Parameters.AddWithValue("@Start_date", carHire.StartDate);
                            addCarHireCmd.Parameters.AddWithValue("@End_date", carHire.EndDate);
                            addCarHireCmd.Parameters.AddWithValue("@Driver", carHire.Driver);
                            addCarHireCmd.ExecuteNonQuery();
                        }
                        catch (Exception excep)
                        {
                            Console.WriteLine(excep.Message);
                        }
                    }


                }

                napierchalet.Close();
            }

        }

        public void DeleteBooking(int booking_ref)
        {

            SqlConnection napierchalet = new SqlConnection(connectionString);

            using (napierchalet)
            {
                string bookingQuery = "DELETE FROM Bookings WHERE Booking_ref = @Booking_ref";

                SqlCommand cmd = new SqlCommand(bookingQuery, napierchalet);

                napierchalet.Open();

                try
                {
                    cmd.Parameters.AddWithValue("@Booking_ref", booking_ref);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception excep)
                {
                    Console.WriteLine(excep.Message);
                }

                napierchalet.Close();

            }

        }
        
        public void AddCustomer(Customer customer)
        {

            string query = "INSERT INTO Customers (Name, Address)";
            query += " VALUES (@Name, @Address)";

            SqlConnection napierchalet = new SqlConnection(connectionString);

            SqlCommand cmd = new SqlCommand(query, napierchalet);

            using (napierchalet)
            {

                napierchalet.Open();

                try
                {
                    cmd.Parameters.AddWithValue("@Name", customer.Name);
                    cmd.Parameters.AddWithValue("@Address", customer.Address);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception excep)
                {
                    Console.WriteLine(excep.Message);
                }

                napierchalet.Close();
            }

        }

        public Customer FindCustomer(int customerNum)
        {
            Customer cus = new Customer();

            string query = "SELECT * FROM Customers WHERE Customer_num=@Customer_num";

            SqlConnection napierchalet = new SqlConnection(connectionString);

            using (napierchalet)
            {

                SqlCommand cmd = new SqlCommand(query, napierchalet);
                cmd.Parameters.AddWithValue("@Customer_num", customerNum);

                napierchalet.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    throw new ArgumentException("Customer not found!");
                }

                while (reader.Read())
                {
                    cus.CustomerNum = Convert.ToInt32(reader["Customer_num"].ToString());
                    cus.Name = reader["Name"].ToString();
                    cus.Address = reader["Address"].ToString();
                }

                napierchalet.Close();
            }


            return cus;
        }

        public void UpdateCustomer(Customer cus)
        {
            SqlConnection napierchalet = new SqlConnection(connectionString);

            using (napierchalet)
            {
                string query = "UPDATE Customers SET Name = @Name, Address = @Address";
                query += " WHERE Customer_num = @Customer_num";

                SqlCommand cmd = new SqlCommand(query, napierchalet);
                
                napierchalet.Open();

                try
                {
                    cmd.Parameters.AddWithValue("@Customer_num", cus.CustomerNum);
                    cmd.Parameters.AddWithValue("@Name", cus.Name);
                    cmd.Parameters.AddWithValue("@Address", cus.Address);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception excep)
                {
                    Console.WriteLine(excep.Message);
                }
            }
        }

        public void DeleteCustomer(int customerNum)
        {
            SqlConnection napierchalet = new SqlConnection(connectionString);
                     

            using (napierchalet)
            {
                string query = "DELETE FROM Customers WHERE Customer_num = @Customer_num";

                SqlCommand cmd = new SqlCommand(query, napierchalet);

                napierchalet.Open();

                try
                {
                    cmd.Parameters.AddWithValue("@Customer_num", customerNum);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception excep)
                {
                    Console.WriteLine(excep.Message);
                }

                napierchalet.Close();

            }
        }

        public ObservableCollection<int> getCustomerBookingsList(int customerNum)
        {
            ObservableCollection<int> list = new ObservableCollection<int>();

            string query = "SELECT * FROM Bookings WHERE Customer_num=@Customer_num";

            SqlConnection napierchalet = new SqlConnection(connectionString);

            using (napierchalet)
            {

                SqlCommand cmd = new SqlCommand(query, napierchalet);
                cmd.Parameters.AddWithValue("@Customer_num", customerNum);

                napierchalet.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                {
                    //wip
                }

                while (reader.Read())
                {
                    int bookingRef = Convert.ToInt32(reader["Booking_ref"].ToString());
                    list.Add(bookingRef);
                }

                napierchalet.Close();
            }
            
            return list;
        }

        public ObservableCollection<Customer> GetCustomerList()
        {

            ObservableCollection<Customer> customerList = new ObservableCollection<Customer>();

            SqlConnection napierchalet = new SqlConnection(connectionString);
           
            using (napierchalet)
            {

                string query = "SELECT * FROM Customers";

                SqlCommand cmd = new SqlCommand(query, napierchalet);

                napierchalet.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Customer customer = new Customer();

                    customer.CustomerNum = Convert.ToInt32(reader["Customer_num"].ToString());
                    customer.Name = reader["Name"].ToString();
                    customer.Address = reader["Address"].ToString();

                    customerList.Add(customer);
                }

                napierchalet.Close();
            }

            return customerList;
        }

        public void DeleteGuest(int Guest_ref)
        {
            SqlConnection napierchalet = new SqlConnection(connectionString);

            using (napierchalet)
            {
                string query = "DELETE FROM Guests WHERE Guest_ref=@Guest_ref";

                SqlCommand cmd = new SqlCommand(query, napierchalet);

                napierchalet.Open();

                try
                {
                    cmd.Parameters.AddWithValue("@Guest_ref", Guest_ref);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception excep)
                {
                    Console.WriteLine(excep.Message);
                }

                napierchalet.Close();
            }
        }

        public void DeleteCarHire(int Car_hire_ref)
        {
            SqlConnection napierchalet = new SqlConnection(connectionString);

            using (napierchalet)
            {
                string query = "DELETE FROM Car_hires WHERE Car_hire_ref=@Car_hire_ref";

                SqlCommand cmd = new SqlCommand(query, napierchalet);

                napierchalet.Open();

                try
                {
                    cmd.Parameters.AddWithValue("@Car_hire_ref", Car_hire_ref);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception excep)
                {
                    Console.WriteLine(excep.Message);
                }

                napierchalet.Close();
            }
        }

    }
}
