using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;

namespace Assignment6AirlineReservation
{
    internal class FlightManager
    {


        /// <summary>
        /// holds the database of flights and passengers
        /// </summary>
        private clsDataAccess database;

        /// <summary>
        /// default constructor
        /// </summary>
        public FlightManager()
        {
            database = new clsDataAccess(); // initialize the database of flights and passengers
        }

        /// <summary>
        /// Gets the list of flights from the database and returns them to the caller
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<clsFlight> getFlights()
        {
            try
            {
                // create a dataset to hold the flight information
                DataSet flightInfo;
                int iRetVal = 0;

                // create a generic list to hold the flights
                List<clsFlight> flightList = new List<clsFlight>();

                // get all of the flights from the dataset
                flightInfo = database.ExecuteSQLStatement("SELECT Flight_ID, Flight_Number, Aircraft_Type FROM FLIGHT", ref iRetVal);

                // add the flight names into the flightSelector combo box
                for (int i = 0; i < flightInfo.Tables[0].Rows.Count; i++)
                {

                    // add the flight name to the array
                    clsFlight currFlight = new clsFlight(); // create a new flight object

                    // add the flight's attributes
                    currFlight.FlightID = Convert.ToInt32(flightInfo.Tables[0].Rows[i][0].ToString());
                    currFlight.FlightNumber = Convert.ToInt32(flightInfo.Tables[0].Rows[i][1].ToString());
                    currFlight.Airframe = flightInfo.Tables[0].Rows[i][2].ToString();

                    // add currFlight to the flightList
                    flightList.Add(currFlight); // cast currFlight to type T and add it to the flightList

                }
                return flightList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Returns an array of all flight IDs from the database
        /// </summary>
        /// <returns></returns>
        public string[] getFlightIDs()
        {
            try
            {
                // create a dataset to hold the flight information
                DataSet flightInfo;
                int iRetVal = 0;
                // get all of the flights from the dataset
                flightInfo = database.ExecuteSQLStatement("SELECT Flight_ID FROM FLIGHT", ref iRetVal);
                // create an array to hold the flight IDs
                string[] flightIDs = new string[iRetVal];
                // add the flight IDs into the array
                for (int i = 0; i < iRetVal; i++)
                {
                    flightIDs[i] = flightInfo.Tables[0].Rows[i][0].ToString();
                }
                return flightIDs;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        /// <summary>
        /// Gets the list of passengers from the database and returns them to the caller
        /// </summary>
        /// <param name="flightNumber"><</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public List<clsPassenger> getPassengers(string flightNumber)
        {
            try
            {
                if (flightNumber == null)
                    return new List<clsPassenger>(0);

                // create a dataset to hold the flight information
                DataSet passengerInfo;
                int iRetVal = 0;
                List<clsPassenger> passengers = new List<clsPassenger>();

                // construct the SQL statement to get the given flight's passenger list
                StringBuilder sSQL = new StringBuilder();
                sSQL.Append("SELECT PASSENGER.Passenger_ID, First_Name, Last_Name, Seat_Number " +
              "FROM FLIGHT_PASSENGER_LINK, FLIGHT, PASSENGER " +
          "WHERE FLIGHT.FLIGHT_ID = FLIGHT_PASSENGER_LINK.FLIGHT_ID AND " +
          "FLIGHT_PASSENGER_LINK.PASSENGER_ID = PASSENGER.PASSENGER_ID AND " +
          "FLIGHT.FLIGHT_ID = ");
                sSQL.Append(flightNumber.ToString());

                // get all of the flights from the dataset
                passengerInfo = database.ExecuteSQLStatement(sSQL.ToString(), ref iRetVal);

                // add the flight names into the flightSelector combo box
                for (int i = 0; i < passengerInfo.Tables[0].Rows.Count; i++)
                {
                    clsPassenger currPassenger = new clsPassenger(); // create a new passenger object
                    // add the passenger's attributes
                    currPassenger.PassengerID = Convert.ToInt32(passengerInfo.Tables[0].Rows[i][0].ToString());
                    currPassenger.FirstName = passengerInfo.Tables[0].Rows[i][1].ToString();
                    currPassenger.LastName = passengerInfo.Tables[0].Rows[i][2].ToString();
                    currPassenger.SeatNumber = Convert.ToInt32(passengerInfo.Tables[0].Rows[i][3].ToString());


                    // add the passenger to the array
                    passengers.Add(currPassenger);
                }
                return passengers;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Removes the passenger from the flight's passenger list
        /// </summary>
        /// <param name="passengerID"></param>
        /// <param name="flightNumber"></param>
        public void removePassenger(int passengerID, string flightNumber)
        {
            try
            {
                // construct the SQL statement to remove the passenger from the flight's passenger list
                StringBuilder sSQL = new StringBuilder();
                sSQL.Append("DELETE FROM FLIGHT_PASSENGER_LINK WHERE PASSENGER_ID = ");
                sSQL.Append(passengerID.ToString());
                sSQL.Append(" AND FLIGHT_ID = ");
                sSQL.Append(flightNumber);
                // execute the SQL statement
                database.ExecuteNonQuery(sSQL.ToString());

                // delete the passenger from the passenger table
                sSQL = new StringBuilder();
                sSQL.Append("DELETE FROM PASSENGER WHERE PASSENGER_ID = ");
                sSQL.Append(passengerID.ToString());

                // execute SQL
                database.ExecuteNonQuery(sSQL.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Updates the given passenger's seat number on the given flight
        /// </summary>
        /// <param name="passengerID"></param>
        /// <param name="flightNumber"></param>
        /// <param name="seatNumber"></param>
        /// <exception cref="Exception"></exception>
        public void updateSeat(int passengerID, string flightNumber, string seatNumber)
        {
            try
            {
                // construct SQL statement to update the passenger's seat number
                StringBuilder sSQL = new StringBuilder();
                sSQL.Append("UPDATE FLIGHT_PASSENGER_LINK SET SEAT_NUMBER = ");
                sSQL.Append(seatNumber.ToString());
                sSQL.Append(" WHERE PASSENGER_ID = ");
                sSQL.Append(passengerID.ToString());
                sSQL.Append(" AND FLIGHT_ID = ");
                sSQL.Append(flightNumber);
                // execute SQL statement
                int iRetVal = 0;
                database.ExecuteNonQuery(sSQL.ToString());

               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Adds a passenger to the database's passenger table
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <exception cref="Exception"></exception>
        public void addPassenger(string firstName, string lastName, string flightID)
        {
            try
            {
                StringBuilder sSQL = new StringBuilder();
                // construct the SQL statement to add the passenger to the passenger table
                sSQL.Append("INSERT INTO PASSENGER (First_Name, Last_Name) VALUES ('");
                sSQL.Append(firstName);
                sSQL.Append("', '");
                sSQL.Append(lastName);
                sSQL.Append("')");
                // execute the SQL statement
                database.ExecuteNonQuery(sSQL.ToString());

                // get the passenger ID of the newly added passenger
                sSQL = new StringBuilder();
                sSQL.Append("SELECT MAX(PASSENGER_ID) FROM PASSENGER");

                int iRetVal = 0;
                DataSet passengerInfo = database.ExecuteSQLStatement(sSQL.ToString(), ref iRetVal);

                if (iRetVal == 0)
                {
                    throw new Exception("No passenger was added to the database.");
                }

                // get the passenger ID
                int passengerID = Convert.ToInt32(passengerInfo.Tables[0].Rows[0][0].ToString());

                // construct the SQL statement to add the passenger to the flight's passenger list
                sSQL = new StringBuilder();
                sSQL.Append("INSERT INTO FLIGHT_PASSENGER_LINK (FLIGHT_ID, PASSENGER_ID, SEAT_NUMBER) VALUES (");
                sSQL.Append(flightID);
                sSQL.Append(", ");
                sSQL.Append(passengerID.ToString());
                sSQL.Append(", 0)"); // seat number is set to 0 by default
                database.ExecuteNonQuery(sSQL.ToString());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}