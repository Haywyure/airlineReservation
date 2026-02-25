using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment6AirlineReservation
{
    internal class clsPassenger
    {
        /// <summary>
        /// holds the passenger's first name
        /// </summary>
        private string firstName;
        /// <summary>
        /// Gets or sets the passenger's first name.
        /// </summary>
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }
        /// <summary>
        /// holds the passenger's last name
        /// </summary>
        private string lastName;
        /// <summary>
        /// Gets or sets the passenger's last name.
        /// </summary>
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }
        /// <summary>
        /// holds the passenger's ID number, primary key in the database
        /// </summary>
        private int passengerID;
        /// <summary>
        /// Gets or sets the passenger ID of the passenger.
        /// </summary>
        public int PassengerID
        {
            get { return passengerID; }
            set { passengerID = value; }
        }

        /// <summary>
        /// holds the passenger's seat number
        /// </summary>
        private int seatNumber;
        /// <summary>
        /// Gets or sets the seat number of the passenger.
        /// </summary>
        public int SeatNumber
        {
            get { return seatNumber; }
            set { seatNumber = value; }
        }

        /// <summary>
        /// Default constructor, sets all values to empty
        /// </summary>
        public clsPassenger()
        {
            firstName = "";
            lastName = "";
            passengerID = 0;
            seatNumber = 0;
        }
    }
}
