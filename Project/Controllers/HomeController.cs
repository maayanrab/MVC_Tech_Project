using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Models;
using Project.Dal;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.Ajax.Utilities;
using System.Diagnostics;
using System.Data.Entity;
using System.Text.RegularExpressions;
using System.Net.Sockets;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ShowFlights()
        {
            var entities = new FlightDal();

            return View(entities.Flights.ToList());

        }

        public List<Flight> updateFlights() {
            FlightDal flightdal = new FlightDal();

            // UPDATE POPULARITY
            Dictionary<string, float> popdict = Popularity();
            foreach (Flight f in flightdal.Flights.ToList())
            {
                f.popularity = popdict[f.destination_country];
            }
            flightdal.SaveChanges();


            List<Flight> flightLst = new List<Flight>();

            // FILTERING PAST FLIGHTS
            foreach (Flight f in flightdal.Flights.ToList())
            {
                if (f.date_time >= DateTime.Now && f.num_of_seats > 0)
                    flightLst.Add(f);
            }
            return flightLst;
        }

        public ActionResult ShowFlightsUser(string username)
        {
            
            List<Flight> flightLst = updateFlights();
            ViewBag.username = username;

            return View(flightLst);

        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult AddFlights(Flight flight)
        {
            return View();
        }

        public ActionResult RemoveFlights(int id = -1)
        {
            ViewBag.flight_num = id;
            return View();
        }

        public ActionResult EditFlights(int id = -1)
        {
            ViewBag.flight_num = id;
            return View();
        }

        public ActionResult Submit(User user)
        {
            String errormsg;

            if (ModelState.IsValid) {

                if (user.password != Request.Form["rpassword"])
                {
                    errormsg = "Error: passwords do not match";
                    ViewBag.Message = errormsg;

                    return View("Register");
                }

                UserDal dal = new UserDal();
                var exists = dal.Users.Find(user.username);

                if (exists == null)
                {
                    dal.Users.Add(user);
                    dal.SaveChanges();

                    return View("User", user);
                }
            }

            errormsg = "Error: username already in use";
            ViewBag.Message = errormsg;

            return View("Register");
            
        }

        public ActionResult AddFlight(Flight flight)
        {

            FlightDal dal = new FlightDal();
            try
            {
                String date = Request.Form["Date"];

                flight.date_time = DateTime.ParseExact(date, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);

                dal.Flights.Add(flight);
                dal.SaveChanges();

                return View("ShowFlights", dal.Flights.ToList());
            }
            catch
            {
                ViewBag.Message = "Error: Form is invalid, please make sure date is in format: d/M/yyyy HH:mm";
                return View("AddFlights");
            }

        }

        public ActionResult RemoveFlight(Flight flight)
        {

            FlightDal dal = new FlightDal();
            var cur_flight = dal.Flights.Find(flight.flight_num);

            if (cur_flight != null)
            {
                dal.Flights.Remove(cur_flight);
                dal.SaveChanges();
                return View("ShowFlights", dal.Flights.ToList());
            }

            String errormsg = "Flight was not found";
            ViewBag.Message = errormsg;

            return View("RemoveFlights");

        }

        public ActionResult EditFlight(Flight flight)
        {

            String errormsg;

            FlightDal dal = new FlightDal();
            var cur_flight = dal.Flights.Find(flight.flight_num);

            if (cur_flight == null)
            {
                errormsg = "Flight was not found";
                ViewBag.Message = errormsg;

                return View("EditFlights");
            }

            if (Request.Form["price"] != "")
                cur_flight.price = float.Parse(Request.Form["price"]);

            if (Request.Form["dest_country"] != "")
                cur_flight.destination_country = Request.Form["dest_country"];

            if (Request.Form["or_country"] != "")
                cur_flight.origin_country = Request.Form["or_country"];

            String date = Request.Form["Date"];
            if (date != "")
            {
                try
                {
                    /*cur_flight.date_time = DateTime.ParseExact(date, "d/M/yyyy HH:mm", CultureInfo.InvariantCulture);*/
                    cur_flight.date_time = DateTime.ParseExact(date, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                }
                catch
                {
                    ViewBag.Message = "Error: date format is invalid. Correct format is: d/M/yyyy HH:mm";
                    return View("EditFlights");
                }
            }

            if (Request.Form["seats_num"] != "")
            {
                try
                {
                    int new_seats = Int32.Parse(Request.Form["seats_num"]);
                    if (new_seats < 0)
                        throw new Exception("Negative seats number");
                    cur_flight.num_of_seats = new_seats;
                }
                catch
                {
                    ViewBag.Message = "Error: number of seats is invalid";
                    return View("EditFlights");
                }
            }

            dal.SaveChanges();

            return View("ShowFlights", dal.Flights.ToList());

        }

        public ActionResult UserLogin(User user)
        {

            UserDal dal = new UserDal();

            var admin = dal.Users.Any(x => x.username == user.username && x.password == user.password && x.admin == 1);
            if (admin)
                return View("Admin", user);

            var userexist = dal.Users.Any(x => x.username == user.username && x.password == user.password);
            if (userexist)
                return View("User", user);
            else
            {
                String errormsg = "username or password are incorrect";
                ViewBag.Message = errormsg;
            }

            return View("Login");

        }

        public ActionResult SearchFlight(Flight flight)
        {
            FlightDal dal = new FlightDal();
            String price = Request.Form["Price"];
            Debug.WriteLine(price);
            if (price != "")
                flight.price = float.Parse(price, CultureInfo.InvariantCulture.NumberFormat);
            String D_C = Request.Form["D_C"];
            if (D_C != "")
                flight.destination_country = D_C;
            String O_C = Request.Form["O_C"];
            if (O_C != "")
                flight.origin_country = O_C;
            String date = Request.Form["Date"];
            if (date != "")
                try
                {
                    /*flight.date_time = DateTime.ParseExact(date, "d/M/yyyy HH:mm", CultureInfo.InvariantCulture);*/
                    flight.date_time = DateTime.ParseExact(date, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture);
                }
                catch
                {
                    String errormsg = "Error: date format is invalid. Correct format is: d/M/yyyy HH:mm";
                    ViewBag.Message = errormsg;

                    return View("SearchFlights");
                }
            
                FlightDal entities = new FlightDal();

                List<Flight> flightLst = new List<Flight>();

                foreach (Flight f in entities.Flights.ToList())
                {

                    if (flight.price != 0 && f.price > flight.price)
                        continue;

                    if (flight.destination_country != null && f.destination_country != flight.destination_country)
                        continue;

                    if (flight.origin_country != null && f.origin_country != flight.origin_country)
                            continue;

                    if (date != "" && f.date_time != flight.date_time)
                        continue;


                    if (f.date_time >= DateTime.Now)
                        flightLst.Add(f);

                }
                
                return View("ShowFlightsUser", flightLst);
            
        }

        public ActionResult SearchFlights()
        {
            return View();
        }

        public ActionResult OrderAdminFlights(String sort)
        {
            return View("ShowFlights", OrderFlights(sort));
        }

        public ActionResult OrderUserFlights(String sort, String username)
        {
            ViewBag.username = username;
            return View("ShowFlightsUser", OrderFlights(sort));
        }

        public List<Flight> OrderFlights(String sort)
        {
            FlightDal entities = new FlightDal();

            ViewBag.PriceSortParm = sort == "price_inc" ? "price_desc" : "price_inc";
            ViewBag.D_C_SortParm = sort == "D_C_a" ? "D_C_d" : "D_C_a";
            ViewBag.O_C_SortParm = sort == "O_C_a" ? "O_C_d" : "O_C_a";
            ViewBag.DateSortParm = sort == "date_a" ? "date_d" : "date_a";
            ViewBag.PopSortParm = sort == "pop_d" ? "pop_a" : "pop_d";


            /*var ent = from f in entities.Flights*/
            var ent = from f in updateFlights()
                      select f;
            switch (sort)
            {
                case "price_inc":
                    ent = ent.OrderBy(f => f.price);
                    break;
                case "price_desc":
                    ent = ent.OrderByDescending(f => f.price);
                    break;
                case "D_C_a":
                    ent = ent.OrderBy(f => f.destination_country);
                    break;
                case "D_C_d":
                    ent = ent.OrderByDescending(f => f.destination_country);
                    break;
                case "O_C_a":
                    ent = ent.OrderBy(f => f.origin_country);
                    break;
                case "O_C_d":
                    ent = ent.OrderByDescending(f => f.origin_country);
                    break;
                case "date_a":
                    ent = ent.OrderBy(f => f.date_time);
                    break;
                case "date_d":
                    ent = ent.OrderByDescending(f => f.date_time);
                    break;
                case "pop_a":
                    ent = ent.OrderBy(f => f.popularity);
                    break;
                case "pop_d":
                    ent = ent.OrderByDescending(f => f.popularity);
                    break;
                default:
                    ent = ent.OrderBy(f => f.flight_num);
                    break;
            }
            return ent.ToList();
        }

        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        bool IsLettersOnly(string str)
        {
            bool res = false;
            foreach (char c in str)
            {
                if (c >= 'A' && c <= 'z')
                    res = true;
                if (c == ' ')
                    continue;
                if (c < 'A' || c > 'z')
                    return false;
            }

            return res;
        }

        public ActionResult BookFlights(string username, int id = -1)
        {
            if (TempData["Message"] != null)
            {
                ViewBag.Message = TempData["Message"].ToString();
                TempData["Message"] = null;
            }
            ViewBag.username = username;
            ViewBag.flight_num = id;

            FlightDal flightdal = new FlightDal();
            Flight f = flightdal.Flights.Where(i => i.flight_num == id).FirstOrDefault();

            ViewBag.price = f.price;

            return View("BookFlights");
        }

        public ActionResult BookFlight(Ticket ticket, string username)
        {

            if (ticket.num_of_tickets <= 0)
            {
                ViewBag.Message = "Error: number of tickets must be a positive integer!";
                TempData["Message"] = ViewBag.Message;
                return Redirect(String.Format("/Home/BookFlights/{0}/{1}", username, ticket.flight_num));
            }

            TicketDal ticketdal = new TicketDal();
            FlightDal flightdal = new FlightDal();
            CreditCardDal creditcarddal = new CreditCardDal();
            Flight flight;
            try
            {
                flight = flightdal.Flights.Find(ticket.flight_num);

                float totalPrice = flight.price * ticket.num_of_tickets;

                if (flight.num_of_seats - ticket.num_of_tickets >= 0)
                {

                    // Proccessing payment:  @@@ CHECK VALIDATION

                    string credit_num = Request.Form["credit_num"];
                    if (!IsDigitsOnly(credit_num)) {
                        ViewBag.Message = "Error: credit card must contain digits only!";
                        TempData["Message"] = ViewBag.Message;
                        return Redirect(String.Format("/Home/BookFlights/{0}/{1}", username, ticket.flight_num));
                    }

                    string cvc = Request.Form["cvc"];
                    if (!IsDigitsOnly(cvc))
                    {
                        ViewBag.Message = "Error: CVC must contain digits only!";
                        TempData["Message"] = ViewBag.Message;
                        return Redirect(String.Format("/Home/BookFlights/{0}/{1}", username, ticket.flight_num));
                    }

                    string full_name = Request.Form["full_name"];
                    if (!IsLettersOnly(full_name))
                    {
                        ViewBag.Message = "Error: Full name must contain letters only!";
                        TempData["Message"] = ViewBag.Message;
                        return Redirect(String.Format("/Home/BookFlights/{0}/{1}", username, ticket.flight_num));
                    }


                    string ID = Request.Form["ID"];
                    if (!IsDigitsOnly(ID))
                    {
                        ViewBag.Message = "Error: ID must contain digits only!";
                        TempData["Message"] = ViewBag.Message;
                        return Redirect(String.Format("/Home/BookFlights/{0}/{1}", username, ticket.flight_num));
                    }

                    CreditCard c = creditcarddal.CreditCards.Where(i => i.credit_num == credit_num).FirstOrDefault();
                    if (c == null)  // first time paying with this card
                    {
                        CreditCard creditcard = new CreditCard();
                        creditcard.credit_num = credit_num;  // ADD AES @@@ BONUS!!!
                        creditcard.cvc = cvc;
                        creditcard.full_name = full_name;
                        creditcard.payer_id = ID;
                        creditcard.balance -= totalPrice;
                        creditcarddal.CreditCards.Add(creditcard);
                        creditcarddal.SaveChanges();
                    }
                    else
                    {
                        c.balance -= totalPrice;
                        creditcarddal.SaveChanges();
                    }

                    // Removing tickets from database

                    Ticket t = ticketdal.Tickets.Where(i => i.username == username && i.flight_num == ticket.flight_num).FirstOrDefault();
                    if (t == null)  // first time buying a ticket for this flight
                    {
                        ticket.username = username;
                        ticketdal.Tickets.Add(ticket);
                        ticketdal.SaveChanges();
                    }
                    else  // ticket for the flight already exists
                    {
                        t.num_of_tickets += ticket.num_of_tickets;
                        ticketdal.SaveChanges();
                    }
                    
                    flight.num_of_seats -= ticket.num_of_tickets;
                    flightdal.SaveChanges();

                    // Payment successful
                    TempData["flight_num"] = flight.flight_num;
                    TempData["destination_country"] = flight.destination_country;
                    TempData["origin_country"] = flight.origin_country;
                    TempData["date_time"] = flight.date_time;
                    TempData["num_of_tickets"] = ticket.num_of_tickets;
                    TempData["price"] = ticket.num_of_tickets * flight.price;

                    return Redirect("/Home/PaymentSuccessful/");
                }
                else
                {
                    ViewBag.Message = "Error: not enough remaining tickets";
                    TempData["Message"] = ViewBag.Message;
                    return Redirect(String.Format("/Home/BookFlights/{0}/{1}", username, ticket.flight_num));
                }
            }
            catch
            {
                ViewBag.Message = "Error: Form is invalid";
                TempData["Message"] = ViewBag.Message;
                return Redirect(String.Format("/Home/BookFlights/{0}/{1}", username, ticket.flight_num));
            }

        }

        public ActionResult ShowTickets(string username)  // $$$
        {
            ViewBag.username = username;

            TicketDal entities = new TicketDal();

            List<Ticket> ticketLst = new List<Ticket>();

            FlightDal flightdal = new FlightDal();

            List<Flight> flights = new List<Flight>();

            foreach (Ticket t in entities.Tickets.ToList())
            {
                if (t.username == username)
                    ticketLst.Add(t);

                Flight temp = flightdal.Flights.Where(i => i.flight_num == t.flight_num).FirstOrDefault();

                flights.Add(temp);

            }

            ViewBag.flights = flights;

            return View(ticketLst);

        }

        public ActionResult PaymentSuccessful()
        {
            ViewBag.flight_num = TempData["flight_num"];
            TempData["flight_num"] = null;

            ViewBag.destination_country = TempData["destination_country"];
            TempData["destination_country"] = null;

            ViewBag.origin_country = TempData["origin_country"];
            TempData["origin_country"] = null;

            ViewBag.date_time = TempData["date_time"];
            TempData["date_time"] = null;

            ViewBag.num_of_tickets = TempData["num_of_tickets"];
            TempData["num_of_tickets"] = null;

            ViewBag.price = TempData["price"];
            TempData["price"] = null;

            return View();
        }

        public Dictionary<string, float> Popularity()
        {
            TicketDal ticketdal = new TicketDal();
            FlightDal flightdal = new FlightDal();
            Flight flight;
            Dictionary<string, float> popdict = new Dictionary<string, float>();
            Dictionary<string, float> temp = new Dictionary<string, float>();

            if (ViewBag.dict == null)
            {
                foreach (Flight f in flightdal.Flights.ToList())
                {
                    try
                    {
                        temp.Add(f.destination_country, 0);
                        popdict.Add(f.destination_country, 0);
                    }
                    catch { }
                    popdict[f.destination_country] += f.num_of_seats;
                }
            }

            foreach (Ticket t in ticketdal.Tickets.ToList())
            {
                flight = flightdal.Flights.Find(t.flight_num);
                popdict[flight.destination_country] += t.num_of_tickets;
                temp[flight.destination_country] += t.num_of_tickets;
            }

            foreach (string s in popdict.Keys)
                temp[s] /= popdict[s];

            return temp;
        }







    }
}