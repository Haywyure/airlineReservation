using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
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

namespace Assignment6AirlineReservation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        wndAddPassenger wndAddPass;

        /// <summary>
        /// Holds the class that handles all seat reservations and database interactions
        /// </summary>
        private FlightManager seats;

        /// <summary>
        /// Holds the last selected seat
        /// </summary>
        private Label lastSeat;

        /// <summary>
        /// Holds the state of whether the last seat is taken or not
        /// </summary>
        private bool isLastSeatTaken;

        

        /// <summary>
        /// Determines if a seat is being changed
        /// </summary>
        private bool isSeatChanging;

        /// <summary>
        /// Flag determining if a newly added passenger 
        /// has been assigned a seat
        /// 
        /// true = assigned/no new passenger
        /// false = needs seat assigning
        /// </summary>
        private bool isNewPassengerAssigned;
        

        /// <summary>
        /// Constructor for the main window
        /// </summary>
        public MainWindow()
        {
            try
            {
                InitializeComponent();

                // set all variables to defaults
                seats = new FlightManager();
                lastSeat = null;
                isLastSeatTaken = false;
                isSeatChanging = false;
                isNewPassengerAssigned = true;

                // automatically get the available flights from the database
                fillFlightComboBox();

                // hide both flight layouts
                Canvas767.Visibility = Visibility.Hidden;
                CanvasA380.Visibility = Visibility.Hidden;
            }
            
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }



        /// <summary>
        /// Fills the passengerSelector combo box with the passengers for the selected flight
        /// </summary>
        private void fillPassengerComboBox()
        {
            try
            {
                // disable delete and change seat buttons
                cmdChangeSeat.IsEnabled = false;
                cmdDeletePassenger.IsEnabled = false;

                // get the selected flight from the flightSelector combo box
                int selectedFlight = 0;
                // get the current selected flight index
                if (cbChooseFlight.SelectedIndex >= 0)
                {
                    // selectedFlight the selected flight ID
                    selectedFlight = cbChooseFlight.SelectedIndex;
                }


                // match the index to the proper flightID

                // add all flight id's from flights into an array
                string[] flightIDs = seats.getFlightIDs();

                string flightID = flightIDs[selectedFlight];

                // list to hold the passengers
                List<clsPassenger> passengers = seats.getPassengers(flightID);

                // format the passengers for display in the combo box
                List<string> formattedPassengers = new List<string>();
                foreach (clsPassenger passenger in passengers)
                {
                    // format the passenger name and seat number
                    formattedPassengers.Add($"{passenger.FirstName} {passenger.LastName}");
                }

                // bind the passengers to the passengerSelector combo box
                cbChoosePassenger.ItemsSource = formattedPassengers;
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }


        /// <summary>
        /// Fills the flightSelector combo box with the available flights from the database 
        /// </summary>
        private void fillFlightComboBox()
        {
            try
            {
                // list to hold the flights
                List<clsFlight> flights = seats.getFlights();

                List<string> formattedFlights = new List<string>();
                // format the flights for display in the combo box
                foreach (clsFlight flight in flights)
                {
                    // format the flight number and airframe type
                    formattedFlights.Add($"Flight {flight.FlightNumber} ---  {flight.Airframe}");
                }

                // bind the flights to the flightSelector combo box
                cbChooseFlight.Items.Clear();
                cbChooseFlight.ItemsSource = formattedFlights;


            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        private void cbChooseFlight_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                cbChoosePassenger.ItemsSource = new string[1];
                cbChoosePassenger.IsEnabled = false; // disable the passenger combo box until a flight is selected
                resetLastSeat();

                Canvas selectedCanvas = null;
                // check if the flightSelector combo box is set to the first item
                // show the first plane layout and stats if so
                if (cbChooseFlight.SelectedIndex == 0)
                {
                    // set the airframe type to the first plane
                    Flight_Title.Content = seats.getFlights()[0].Airframe;
                    // show the first seat layout diagram
                    Canvas767.Visibility = Visibility.Visible;
                    CanvasA380.Visibility = Visibility.Hidden; // hide the second seat layout

                    selectedCanvas = Canvas767;

                }
                // check if the flightSelector combo box is set to the second item
                // show the second plane layout and stats if so
                else if (cbChooseFlight.SelectedIndex == 1)
                {
                    // set the airframe type to the second plane
                    Flight_Title2.Content = seats.getFlights()[1].Airframe;
                    // show the second seat layout diagram
                    Canvas767.Visibility = Visibility.Hidden;
                    CanvasA380.Visibility = Visibility.Visible; // hide the first seat layout

                    selectedCanvas = CanvasA380;
                }

                // enable the side buttons
                gPassengerCommands.IsEnabled = true;

                // fill the passenger combo box with the passenger list
                fillPassengerComboBox();
                cbChoosePassenger.IsEnabled = true; // reenable the passenger combo box

                // color in seats based on availability
                colorSeats(selectedCanvas);
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                     MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Colors the provided canvas's seat labels
        /// red = taken, blue = available
        /// </summary>
        /// <param name="canvas"></param> the canvas to color
        private void colorSeats(Canvas canvas)
        {
            // color each seat based on availability
            try
            {
                Canvas seatCanvas = null;

                // check if the first/second canvas is in use
                // use the seat canvas of the corresponding diagram
                if (canvas.Equals(Canvas767))
                {
                    seatCanvas = c767_Seats;
                }
                else
                {
                    seatCanvas = cA380_Seats;
                }
                // get the passenger list for the selected flight in the flight combo box
                List<clsPassenger> passengers = seats.getPassengers(seats.getFlightIDs()[cbChooseFlight.SelectedIndex]);



                // set all seat colors to blue
                foreach (UIElement seat in seatCanvas.Children)
                {
                    // ensure only labels are colored
                    if (seat.GetType() == typeof(Label))
                    {
                        Label seatLabel = (Label)seat;

                         // color the seat label red if matched
                         seatLabel.Background = Brushes.Blue;
                            
                    }
                }


                // color each taken seat label
                foreach (UIElement seat in seatCanvas.Children)
                {
                    // ensure only labels are colored
                    if (seat.GetType() == typeof(Label))
                    {
                        // compare the seat number to the passenger list, color all matches red
                        Label seatLabel = (Label)seat;

                        // compare seat numbers until a match is found or all passengers are checked
                        foreach (clsPassenger passenger in passengers)
                        {
                            if (passenger.SeatNumber == (Int32.Parse(seatLabel.Content.ToString())))
                            {
                                // color the seat label red if matched
                                seatLabel.Background = Brushes.Red;
                                break;
                            }
                        }

                    }
                }

                // color the currently selected seat from cbChoosePassenger
                if (cbChoosePassenger.Items.Count > 0)
                {
                    // get the selected passenger index
                    int passengerIndex = cbChoosePassenger.SelectedIndex;
                    if (passengerIndex < 0)
                        return;
                    // get the selected flight ID
                    string flightID = seats.getFlightIDs()[cbChooseFlight.SelectedIndex];
                    // get the current passenger
                    clsPassenger currPassenger = seats.getPassengers(flightID)[passengerIndex];
                    // get the seat label for the current passenger
                    Label seatLabel = null;
                    // check if the first/second canvas is in use
                    if (canvas.Equals(Canvas767))
                    {
                        // get the seat label from the first canvas
                        seatLabel = (Label)c767_Seats.Children[currPassenger.SeatNumber - 1];
                    }
                    else
                    {
                        // get the seat label from the second canvas
                        seatLabel = (Label)cA380_Seats.Children[currPassenger.SeatNumber - 1];
                    }
                    // color the seat label green
                    seatLabel.Background = Brushes.Lime;
                }
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                     MethodInfo.GetCurrentMethod().Name, ex.Message);
            }

        }

        /// <summary>
        /// Opens the add passenger window to add a new passenger to the flight database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdAddPassenger_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // set isNewPassengerAssigned to false to ensure they get a seat
                isNewPassengerAssigned = false;

                // open the AdddPassenger window
                wndAddPass = new wndAddPassenger();
                wndAddPass.ShowDialog();

                // if a passenger was added, lock all controls
                // until a seat is assigned to them
                if (wndAddPass.DialogResult == true)
                {
                    
                    // re-fill the passenger combo box 
                    //fillPassengerComboBox();

                    // set the passenger combo box to the last passenger added
                    cbChoosePassenger.SelectedIndex = cbChoosePassenger.Items.Count - 1;
                    

                    // enable the change seat control
                    cmdChangeSeat.IsEnabled = true;

                    // disable other controls
                    cbChooseFlight.IsEnabled = false;
                    cmdAddPassenger.IsEnabled = false;
                    cmdDeletePassenger.IsEnabled = false;
                    cbChoosePassenger.IsEnabled = false;


                    // reset the last seat pointer
                    resetLastSeat();
                }
                else
                    // set isNewPassengerAssigned to false to ensure they get a seat
                    isNewPassengerAssigned = true;

            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Adds the given first and last name as a new passenger to the current flight's database,
        /// Does not allow any other controls to be used until a seat is assigned to the passenger
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        public void addNewPassenger(string firstName, string lastName)
        {
            try
            {
                // add the passenger to the database
                seats.addPassenger(firstName, lastName, seats.getFlightIDs()[cbChooseFlight.SelectedIndex]);

                // re-fill the passenger combo box with the new passenger
                fillPassengerComboBox();


                // set the passenger combo box to the last passenger added
                cbChoosePassenger.SelectedIndex = cbChoosePassenger.Items.Count - 1;
                // enable the change seat control
                cmdChangeSeat.IsEnabled = true;
                // disable other controls
                cbChooseFlight.IsEnabled = false;
                cmdAddPassenger.IsEnabled = false;
                cmdDeletePassenger.IsEnabled = false;
                cbChoosePassenger.IsEnabled = false; 
            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }


        /// <summary>
        /// Handles errors by displaying a message box with the error information
        /// </summary>
        /// <param name="sClass"></param>
        /// <param name="sMethod"></param>
        /// <param name="sMessage"></param>
        private void HandleError(string sClass, string sMethod, string sMessage)
        {
            try
            {
                MessageBox.Show(sClass + "." + sMethod + " -> " + sMessage);
            }
            catch (System.Exception ex)
            {
                System.IO.File.AppendAllText(@"C:\Error.txt", Environment.NewLine + "HandleError Exception: " + ex.Message);
            }
        }

        /// <summary>
        /// Deletes the selected passenger from the flight database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdDeletePassenger_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // get the selected flightID and passenger
                int passengerIndex = cbChoosePassenger.SelectedIndex;
                string flightID = seats.getFlightIDs()[cbChooseFlight.SelectedIndex];
                clsPassenger currPassenger = seats.getPassengers(flightID)[passengerIndex];

                // remove from the database
                seats.removePassenger(currPassenger.PassengerID, flightID);

                // set the passenger's seat label color to blue
                Label seatLabel = null;

                // hold the current seat diagram canvas
                Canvas currentDiagram = null;

                // check if the first/second canvas is in use
                if (Canvas767.Visibility == Visibility.Visible)
                {
                    // get the seat label from the first canvas
                    seatLabel = (Label)c767_Seats.Children[currPassenger.SeatNumber - 1];
                    currentDiagram = Canvas767;
                }
                else
                {
                    // get the seat label from the second canvas
                    seatLabel = (Label)cA380_Seats.Children[currPassenger.SeatNumber - 1];
                    currentDiagram = CanvasA380;
                }

                seatLabel.Background = Brushes.Blue;
                resetLastSeat();

                // remove the lasSeat pointer
                lastSeat = null;
                isLastSeatTaken = false;

                // refill the passenger combo box
                fillPassengerComboBox();

                // recolor the current canvas diagram
                colorSeats(currentDiagram);
            }
            catch (System.Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Sets the selected passenger's seat label to green
        /// and any prior selected seat back to blue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbChoosePassenger_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // enable delete and change seat buttons
                cmdChangeSeat.IsEnabled = true;
                cmdDeletePassenger.IsEnabled = true;

                // restore the last seat to the proper color
                if (lastSeat != null)
                {
                    if (isLastSeatTaken)
                        lastSeat.Background = Brushes.Red;
                    else
                        lastSeat.Background = Brushes.Blue;
                }

                // get the selected flightID and passenger
                int passengerIndex = cbChoosePassenger.SelectedIndex;
                if (passengerIndex < 0)
                    return;


                string flightID = seats.getFlightIDs()[cbChooseFlight.SelectedIndex];
                clsPassenger currPassenger = seats.getPassengers(flightID)[passengerIndex];

                // set the passenger's seat label color to blue
                Label seatLabel = null;

                // handle adding a new passenger, as they have an unassigned seat
                if (!isNewPassengerAssigned)
                {
                    
                    return;
                }
                // check if the first/second canvas is in use
                else if (Canvas767.Visibility == Visibility.Visible)
                {
                    // get the seat label from the first canvas
                    seatLabel = (Label)c767_Seats.Children[currPassenger.SeatNumber - 1];
                }
                else
                {
                    // get the seat label from the second canvas
                    seatLabel = (Label)cA380_Seats.Children[currPassenger.SeatNumber - 1];
                }

                // check if the seat is taken or not
                isLastSeatTaken = (seatLabel.Background == Brushes.Red);

                seatLabel.Background = Brushes.Lime;

                lastSeat = seatLabel;


                // set the passenger seat label
                lblPassengersSeatNumber.Content = currPassenger.SeatNumber;

            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Restores the last selected seat to it's original color
        /// </summary>
        private void resetLastSeat()
        {
            try
            {
                // restore the last seat to the proper color
                if (lastSeat != null)
                {
                    if (isLastSeatTaken)
                        lastSeat.Background = Brushes.Red;
                    else
                        lastSeat.Background = Brushes.Blue;
                }
                lastSeat = null;
                isLastSeatTaken = false;
            }

            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }

        /// <summary>
        /// Changes the selected passenger's seat to a selected seat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdChangeSeat_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                // set isSeatChanging flag
                isSeatChanging = true;

                // lock all controls but change seat
                cmdAddPassenger.IsEnabled = false;
                cmdDeletePassenger.IsEnabled = false;
                cbChooseFlight.IsEnabled = false; 
                cbChoosePassenger.IsEnabled = false; 


            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }

        }

        /// <summary>
        /// Highlights a seat when clicked on
        /// Allows only one seat to be selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdSelectSeat(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // return if the isSeatChanging flag is not set
                if (!isSeatChanging)
                {
                    return;
                }

                // get the clicked seat label
                Label activeSeat = (Label)sender;
                

                // check if the clicked seat is already taken
                List<clsPassenger> passengers = seats.getPassengers(seats.getFlightIDs()[cbChooseFlight.SelectedIndex]);
                foreach(clsPassenger passenger in passengers)
                {
                    // prevent user from selecting it
                    if (passenger.SeatNumber == Int32.Parse(activeSeat.Content.ToString()))
                    {
                        return;
                    }
                }


                // abort action if ther is no active seat
                if (activeSeat == null)
                    return;

                // get the selected flightID and passengerID
                int passengerIndex = cbChoosePassenger.SelectedIndex;
                // force the choice to be the new passenger if one has been added
                if (!isNewPassengerAssigned)
                {
                    passengerIndex = cbChoosePassenger.Items.Count - 1; // set to the last passenger added
                }

                if (passengerIndex < 0)
                    return;
                string flightID = seats.getFlightIDs()[cbChooseFlight.SelectedIndex];
                clsPassenger currPassenger = seats.getPassengers(flightID)[passengerIndex];

                // assign the new seat number to the passenger
                string newSeatNumber = activeSeat.Content.ToString();

                // color the chosen seat red, update the database
                activeSeat.Background = Brushes.Red;
                seats.updateSeat(currPassenger.PassengerID, flightID, newSeatNumber);
                lastSeat.Background = Brushes.Blue;

                // set the current cbChoosePassenger index to the current passenger
                cbChoosePassenger.SelectedIndex = passengerIndex;


                // compare the passenger being assigned to a potentially newly added passenger
                if (passengerIndex == cbChoosePassenger.Items.Count - 1)
                {
                    // set the new passenger to assigned
                    isNewPassengerAssigned = true;
                    // reenable the controls now that they have a seat
                    cbChooseFlight.IsEnabled = true;
                    cbChoosePassenger.IsEnabled = true;
                    cbChoosePassenger.IsEnabled = true;
                    cmdAddPassenger.IsEnabled = true;
                    cmdDeletePassenger.IsEnabled = true;

                }


                // determine which flight diagram canvas is in use, recolor seats with it
                Canvas currentDiagram = null;
                if (Canvas767.Visibility.Equals(Visibility.Visible))
                {
                    currentDiagram = Canvas767;
                }
                else
                {
                    currentDiagram = CanvasA380;
                }

                colorSeats(currentDiagram);

                // unset isSeatChanging
                isSeatChanging = false;


                // unlock all controls but change seat
                cmdAddPassenger.IsEnabled = true;
                cmdDeletePassenger.IsEnabled = true;



                // color the currently selected seat from cbChoosePassenger
                if (cbChoosePassenger.Items.Count > 0)
                {
                    // get the selected passenger index
                     passengerIndex = cbChoosePassenger.SelectedIndex;
                    if (passengerIndex < 0)
                        return;
                    // get the selected flight ID
                    flightID = seats.getFlightIDs()[cbChooseFlight.SelectedIndex];
                    // get the current passenger
                    currPassenger = seats.getPassengers(flightID)[passengerIndex];
                    // get the seat label for the current passenger
                    Label seatLabel = null;
                    // check if the first/second canvas is in use
                    if (currentDiagram.Equals(Canvas767))
                    {
                        // get the seat label from the first canvas
                        seatLabel = (Label)c767_Seats.Children[currPassenger.SeatNumber - 1];
                    }
                    else
                    {
                        // get the seat label from the second canvas
                        seatLabel = (Label)cA380_Seats.Children[currPassenger.SeatNumber - 1];
                    }
                    // color the seat label green
                    seatLabel.Background = Brushes.Lime;
                    lastSeat = seatLabel;
                }

            }
            catch (Exception ex)
            {
                HandleError(MethodInfo.GetCurrentMethod().DeclaringType.Name,
                    MethodInfo.GetCurrentMethod().Name, ex.Message);
            }
        }
    }
}
