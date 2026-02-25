using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment6AirlineReservation
{
    internal class clsFlight
    {
        /// <summary>
        /// holds the flight's ID, primary key in the database
        /// </summary>
        private int flightID;
        /// <summary>
        /// Gets or sets the flight ID of the flight.
        /// </summary>
        public int FlightID
        {
            get { return flightID; }
            set { flightID = value; }
        }

        /// <summary>
        /// holds the flights flight number
        /// </summary>
        private int flightNumber;
        /// <summary>
        /// Gets or sets the flight number of the flight.
        /// </summary>

        public int FlightNumber
        {
            get { return flightNumber; }
            set { flightNumber = value; }
        }
        /// <summary>
        /// Holds the flight's airframe
        /// </summary>
        private string airframe;
        /// <summary>
        /// Gets or sets the airframe of the flight.
        /// </summary>
        public string Airframe
        {
            get { return airframe; }
            set { airframe = value; }
        }

        /// <summary>
        /// Default constructor, sets all values to empty
        /// </summary>
        public clsFlight()
        {
            flightNumber = 0;
            airframe = "";
        }

        
    }
}
