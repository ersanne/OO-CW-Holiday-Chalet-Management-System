using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class Booking
    {

        private static Booking instance;

        private int _bookingRef;
        private int _chaletID;
        private int _customerNum;
        private DateTime _arrivalDate;
        private DateTime _departureDate;
        private ObservableCollection<Guest> _guestList = new ObservableCollection<Guest>();
        private ObservableCollection<CarHire> _carHireList = new ObservableCollection<CarHire>();
        private bool _breakfastMeals;
        private bool _eveningMeals;
        private int _chaletCost;
        private int _eveningMealsCost;
        private int _breakfastMealsCost;
        private int _carHireCost;
        private int _totalCost;


        private Booking() { }

        public static Booking GetInstance()
        {
                if (instance == null)
                {
                    instance = new Booking();
                }
                return instance;
        }

        public void ClearBookingLists()
        {
            _guestList.Clear();
            _carHireList.Clear();
        }

        //Singleton done

        public int CustomerNum
        {
            get
            {
                return _customerNum;
            }
            set
            {
                _customerNum = value;
            }
        }

        public DateTime ArrivalDate
        {
            get
            {
                return _arrivalDate;
            }
            set
            {
                if (value == DateTime.MinValue || value == null)
                {
                    throw new ArgumentException("Please set a valid arrival date!");
                }

                _arrivalDate = value;
            }
        }

        public DateTime DepartureDate
        {
            get
            {
                return _departureDate;
            }
            set
            {
                if (value == DateTime.MinValue || value == null)
                {
                    throw new ArgumentException("Please set a valid departure date!");
                }

                _departureDate = value;
            }
        }

        public int ChaletID
        {
            get
            {
                return _chaletID;
            }
            set
            {
                if (value < 1 || value > 10)
                {
                    throw new ArgumentException("Please select a valid chalet (ID 1-10)");
                }
                _chaletID = value;
            }
        }

        public int BookingRef
        {
            get
            {
                return _bookingRef;
            }
            set
            {
                _bookingRef = value;
            }
        }

        public bool BreakfastMeals
        {
            get
            {
                return _breakfastMeals;
            }
            set
            {
                _breakfastMeals = value;
            }
        }

        public bool EveningMeals
        {
            get
            {
                return _eveningMeals;
            }
            set
            {
                _eveningMeals = value;
            }
        }

        public ObservableCollection<Guest> GuestList
        {
            get
            {
                return _guestList;
            }
            set
            {
                _guestList = value;
            }
        }

        public ObservableCollection<CarHire> CarHireList
        {
            get
            {
                return _carHireList;
            }
            set
            {
                _carHireList = value;
            }
        }

        public int ChaletCost
        {
            get
            {
                int guestAmount = _guestList.Count;
                int daysAmount = (_departureDate - _arrivalDate).Days;
                _chaletCost = (60 + (25 * guestAmount)) * daysAmount;

                return _chaletCost;
            }
        }

        public int EveningMealsCost
        {
            get
            {
                if (!_eveningMeals)
                {
                    _eveningMealsCost = 0;
                    return _eveningMealsCost;
                }

                int guestAmount = _guestList.Count;
                int daysAmount = (_departureDate - _arrivalDate).Days;
                _eveningMealsCost = (guestAmount * 10) * daysAmount;

                return _eveningMealsCost;
            }
        }

        public int BreakFastMealsCost
        {
            get
            {
                if (!_breakfastMeals)
                {
                    _breakfastMealsCost = 0;
                    return _breakfastMealsCost;
                }

                int guestAmount = _guestList.Count;
                int daysAmount = (_departureDate - _arrivalDate).Days;
                _breakfastMealsCost = (guestAmount * 10) * daysAmount;

                return _breakfastMealsCost;
            }
        }

        public int CarHireCost
        {
            get
            {
                if(_carHireList.Count == 0)
                {
                    _carHireCost = 0;
                    return _carHireCost;
                }

                foreach (CarHire hire in _carHireList)
                {
                    int hireDays = (hire.EndDate - hire.StartDate).Days;

                    _carHireCost = _carHireCost + (50 * hireDays);

                }

                return _carHireCost;
            }
        }

        public int TotalCost
        {
            get
            {
                _totalCost = ChaletCost + EveningMealsCost + BreakFastMealsCost + CarHireCost;

                return _totalCost;
            }
        }

        //Properties done

        public void AddGuest(Guest newGuest)
        {
            if(_guestList.Count >= 6)
            {
                throw new ArgumentException("Maximun number (6) of guests reached!");
            }

            _guestList.Add(newGuest);
        }

        public void AddCarHire(CarHire newCarHire)
        {
            _carHireList.Add(newCarHire);
        }
        
        //Methods done

    }
}
