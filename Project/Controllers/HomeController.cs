using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project.Models;
using Project.Dal;
using System.Globalization;

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

        public ActionResult RemoveFlights()
        {
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
                dal.Users.Add(user);
                dal.SaveChanges();

                return View("User", user);
            }

            return View();
            
        }

        public ActionResult AddFlight(Flight flight)
        {

            /*UserDal userdal = new UserDal();
            var admin = userdal.Users.Find("admin");*/

            FlightDal dal = new FlightDal();
            /*flight.flight_num = 1912;*/
            String temp = flight.date_time.ToString();  // EDIT BELOW
            flight.date_time = DateTime.ParseExact("12/20/2024 20:48", "M/d/yyyy HH:mm", CultureInfo.InvariantCulture);
            dal.Flights.Add(flight);
            dal.SaveChanges();

            /*return View("admin", admin);*/
            return View("ReturnShowFlights");

        }

        public ActionResult RemoveFlight(Flight flight)
        {

            /*UserDal userdal = new UserDal();
            var admin = userdal.Users.Find("admin");*/

            FlightDal dal = new FlightDal();
            var cur_flight = dal.Flights.Find(flight.flight_num);

            if (cur_flight != null)
            {
                dal.Flights.Remove(cur_flight);
                dal.SaveChanges();
            }

            /*return View("admin", admin);*/
            return View("ReturnShowFlights");

        }

        public ActionResult EditFlight(Flight flight)
        {

            /*UserDal userdal = new UserDal();
            var admin = userdal.Users.Find("admin");*/

            FlightDal dal = new FlightDal();
            var cur_flight = dal.Flights.Find(flight.flight_num);  // ADD EDIT

            cur_flight.flight_num = flight.flight_num;
            cur_flight.price = flight.price;
            cur_flight.destination_country = flight.destination_country;
            cur_flight.origin_country = flight.origin_country;
            /*cur_flight.date_time = flight.date_time;*/
            flight.date_time = DateTime.ParseExact("12/20/2024 20:48", "M/d/yyyy HH:mm", CultureInfo.InvariantCulture);
            cur_flight.num_of_seats = flight.num_of_seats;

            dal.SaveChanges();

            /*return View("admin", admin);*/
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

            return View("Login");

        }


    }
}