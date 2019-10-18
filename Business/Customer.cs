using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class Customer
    {
        private String _name;
        private String _address;
        private int _customerNum;

        public String Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Please enter a Name!");
                }                             

                _name = value;
            }          

        }

        public String Address
        {
            get
            {
                return _address;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Please enter an Address!");
                }

                _address = value;
            }
        }
        
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

    }
}
