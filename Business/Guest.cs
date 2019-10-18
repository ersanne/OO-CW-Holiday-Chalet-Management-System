using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class Guest
    {
        private int _guestRef; //PrimaryKey for Databse, Required for database interaction
        private String _name;
        private String _passportNum;
        private int _age;

        public int GuestRef
        {
            get
            {
                return _guestRef;
            }
            set
            {
                _guestRef = value;
            }
        }

        public String Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Please enter a valid name.");

                }

                _name = value;
            }
        }

        public String PassportNum
        {
            get
            {
                return _passportNum;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Please enter a valid Passport Number");

                }

                if (value.Length > 10)
                {
                    throw new ArgumentException("Please enter a valid Passport Number (Max 10 chars)");
                }

                _passportNum = value;
            }
        }

        public int Age
        {
            get
            {
                return _age;
            }
            set
            {
                if (value >= 0 && value < 115)
                {
                    _age = value;
                }
                else
                {
                    throw new ArgumentException("Plese enter a valid age (1-115)");
                }
            }
        }
   


    }
}
