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

        public ActionResult ReturnShowFlights()
        {
            return View();
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

        [Route("Home/RemoveFlights/{flight_num}")]
        public ActionResult RemoveFlights(string flight_num="")
        {
            ViewBag.flight_num = flight_num;
            return View();
        }

        public ActionResult EditFlights()
        {
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

                return View("ReturnShowFlights");
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
                return View("ReturnShowFlights");
            }

            String errormsg = "Flight was not found";
            ViewBag.Message = errormsg;

            return View("RemoveFlights");

        }

        public ActionResult EditFlight(Flight flight)
        {

            String errormsg;

            FlightDal dal = new FlightDal();
            var cur_flight = dal.Flights.Find(flight.flight_num);  // ADD EDIT

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

            return View("ReturnShowFlights");

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

                    flightLst.Add(f);

                }
                
                return View("ShowFlights", flightLst);
                /*return View("ShowFlights", entities.Flights.ToList());*/
                /*return View("ReturnShowFlights");*/
            

            /*return View("SearchFlights");*/
        }
        public ActionResult SearchFlights()
        {
            return View();
        }

        public ActionResult ShowFlight(String sort)
        {
            FlightDal entities = new FlightDal();

            ViewBag.PriceSortParm = sort == "price_inc" ? "price_desc" : "price_inc";
            ViewBag.NameSortParm = String.IsNullOrEmpty(sort) ? "country" : "";
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
                //case "popularity":
                //    ent = ent.OrderByDescending(f => f.EnrollmentDate);
                //    break;

                case "country":
                    ent = ent.OrderBy(f => f.origin_country);
                    break;



            }
            return View("ShowFlights", ent.ToList());
        }

    }
}