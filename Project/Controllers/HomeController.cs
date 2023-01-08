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

        public ActionResult ShowFlightsUser(string username)
        {
            FlightDal entities = new FlightDal();

            List<Flight> flightLst = new List<Flight>();

            foreach (Flight f in entities.Flights.ToList())
            {
                if (f.date_time >= DateTime.Now)
                    flightLst.Add(f);
            }
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

            if (ModelState.IsValid) {

                UserDal dal = new UserDal();
                var exists = dal.Users.Find(user.username);

                if (exists == null)
                {
                    dal.Users.Add(user);
                    dal.SaveChanges();

                    return View("User", user);
                }
            }

            String errormsg = "Error: username already in use";
            ViewBag.Message = errormsg;

            return View("Register");
            
        }

        public ActionResult AddFlight(Flight flight)
        {

            FlightDal dal = new FlightDal();
            try
            {
                String date = Request.Form["Date"];
                flight.date_time = DateTime.ParseExact(date, "d/M/yyyy HH:mm", CultureInfo.InvariantCulture);
                dal.Flights.Add(flight);
                dal.SaveChanges();

                return View("ShowFlights", dal.Flights.ToList());
            }
            catch
            {
                ViewBag.Message = "Error:\nForm is invalid,\nplease make sure date is in format: d/M/yyyy HH:mm";
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
                    cur_flight.date_time = DateTime.ParseExact(date, "d/M/yyyy HH:mm", CultureInfo.InvariantCulture);
                }
                catch
                {
                    ViewBag.Message = "Error: date format is invalid.\nCorrect format is: d/M/yyyy HH:mm";
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
                    flight.date_time = DateTime.ParseExact(date, "d/M/yyyy HH:mm", CultureInfo.InvariantCulture);
                }
                catch
                {
                    ViewBag.Message = "Error: date format is invalid.\nCorrect format is: d/M/yyyy HH:mm";
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
            // ViewBag.PopSortParm = String.IsNullOrEmpty(sort) ? "popularity" : "";


            var ent = from f in entities.Flights
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
                //case "popularity":
                //    ent = ent.OrderByDescending(f => f.EnrollmentDate);
                //    break;
                case "date_a":
                    ent = ent.OrderBy(f => f.date_time);
                    break;
                case "date_d":
                    ent = ent.OrderByDescending(f => f.date_time);
                    break;
                default:
                    ent = ent.OrderBy(f => f.flight_num);
                    break;
            }
            return ent.ToList();
        }

        public ActionResult BookFlights(string username, int id = -1)
        {
            ViewBag.username = username;
            ViewBag.flight_num = id;
            return View("BookFlights");
        }

        public ActionResult BookFlight(Ticket ticket, string username)
        {

            TicketDal ticketdal = new TicketDal();
            FlightDal flightdal = new FlightDal();
            Flight flight;
            try
            {
                flight = flightdal.Flights.Find(ticket.flight_num);
                if (flight.num_of_seats - ticket.num_of_tickets > 0)
                {
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

                    return Redirect(String.Format("/Home/ShowTickets/{0}", username));
                }
                else
                {
                    ViewBag.Message = "Error:\n not enough remaining tickets";
                    return Redirect(String.Format("/Home/BookFlights/{0}/{1}", username, ticket.flight_num));
                }
            }
            catch
            {
                ViewBag.Message = "Error:\nForm is invalid";
                return Redirect(String.Format("/Home/BookFlights/{0}/{1}", username, ticket.flight_num));
                /*return View("BookFlights");*/
            }

        }

        public ActionResult ShowTickets(string username)
        {
            ViewBag.username = username;

            TicketDal entities = new TicketDal();

            List<Ticket> ticketLst = new List<Ticket>();

            foreach (Ticket t in entities.Tickets.ToList())
            {
                if (t.username == username)
                    ticketLst.Add(t);

            }

            return View(ticketLst);

        }









    }
}