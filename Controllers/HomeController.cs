using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;


namespace ECommerce.Controllers
{
    public class HomeController : Controller
    {

        private ECommerceContext _context;

        public HomeController(ECommerceContext context)
        {
            _context = context;
        }

        // GET: /Home/
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            ViewBag.errors = new List<string>();
            ViewBag.values = new List<string> { "", "", "", "", "", "" };
            return View();
        }

        [HttpPost]
        [Route("Register")]
        public IActionResult Create(RegisterUser model)
        {
            ViewBag.errors = new List<string>();
            ViewBag.values = new List<string> { "", "", "", "", "", "" };
            User MyUser = _context.User.Where(u => u.EmailAddress == model.EmailAddress)
                    .SingleOrDefault();
            if (MyUser == null)
            {
                if (ModelState.IsValid)
                {
                    User NewUser = new User()
                    {
                        Name = model.Name,
                        EmailAddress = model.EmailAddress,
                        Password = model.Password,
                        Description = model.Description
                    };
                    _context.Add(NewUser);
                    _context.SaveChanges();
                    // Set my ID for later.
                    MyUser = _context.User.Where(u => u.EmailAddress == model.EmailAddress)
                        .SingleOrDefault();
                    HttpContext.Session.SetInt32("myID", MyUser.UserId);
                    return RedirectToAction("Users");
                }
                else
                {
                    ViewBag.errors = ModelState.Values;
                    ViewBag.values = new List<string> { model.Name, "", model.EmailAddress, model.Password, model.ConfirmPassword, model.Description };
                }
            }
            else
            {
                ViewBag.errors = ModelState.Values;
                ViewBag.values = new List<string> { model.Name, "This Email Address is already in use. Please pick a different email address.", model.EmailAddress, model.Password, model.ConfirmPassword, model.Description };
            }
            return View("Index");
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginUser model)
        {
            ViewBag.errors = new List<string>();
            ViewBag.values = new List<string> { "", "", "", "", "", "" };
            User MyUser = _context.User.Where(u => u.EmailAddress == model.EmailAddress)
                    .Where(u => u.Password == model.Password)
                    .SingleOrDefault();
            // If the user is found, that matched email and password.
            if (MyUser != null)
            {
                if (ModelState.IsValid)
                {
                    // Set my ID for later.
                    HttpContext.Session.SetInt32("myID", MyUser.UserId);
                    // Move forward.
                    return RedirectToAction("Users");
                }
                else
                {
                    ViewBag.errors = ModelState.Values;
                    ViewBag.values = new List<string> { "", "", model.EmailAddress, model.Password, "", "" };
                }
            }
            else
            {
                ViewBag.errors = ModelState.Values;
                ViewBag.values = new List<string> { "", "The email address / password you entered do not match what we have in our database.", model.EmailAddress, model.Password, "", "" };
            }
            return View("Index");
        }



        [HttpGet]
        [Route("Users")]
        public IActionResult Users()
        {
            ViewBag.errors = new List<string>();
            ViewBag.values = new List<string> { "", "", "", "", "", "" };
            if (HttpContext.Session.GetInt32("myID") == null)
            {
                ViewBag.values = new List<string> { "", "You must login to view this page.", "", "", "", "" };
                return View("Index");
            }
            // ADD: need to make it only users who are not you and that you are not connected/invited with.
            List<User> AllUsers = _context.User.OrderByDescending(o => o.CreatedAt).ToList();
            ViewBag.allusers = AllUsers;
            return View();
        }

        [HttpGet]
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("/Users/{thisID}")]
        public IActionResult ViewUser(int thisID)
        {
            ViewBag.errors = new List<string>();
            ViewBag.values = new List<string> { "", "", "", "", "", "" };
            if (HttpContext.Session.GetInt32("myID") == null)
            {
                ViewBag.values = new List<string> { "", "You must login to view this page.", "", "", "", "" };
                return View("Index");
            }
            User MyUser = _context.User.Where(u => u.UserId == thisID)
                    .SingleOrDefault();
            ViewBag.thisuser = MyUser;
            return View();
        }

        [HttpGet]
        [Route("/Users/{thisID}/SendInvite")]
        public IActionResult SendInvite(int thisID)
        {
            ViewBag.errors = new List<string>();
            ViewBag.values = new List<string> { "", "", "", "", "", "" };
            if (HttpContext.Session.GetInt32("myID") == null)
            {
                ViewBag.values = new List<string> { "", "You must login to view this page.", "", "", "", "" };
                return View("Index");
            }

            User InvitingUser = _context.User.Where(u => u.UserId == (int)(HttpContext.Session.GetInt32("myID")))
                    .SingleOrDefault();
            Invitations NewInvitation = new Invitations()
            {
                UserId = InvitingUser.UserId,
                Name = InvitingUser.Name
            };

            User RecievingUser = _context.User.Where(u => u.UserId == thisID)
                    .Include(i => i.Invitations)
                    .SingleOrDefault();

            if (RecievingUser.Invitations.Contains(NewInvitation))
            {
                ViewBag.values = new List<string> { "", "You've already sent an invitation to " + RecievingUser.Name + "! Wait for them to respond.", "", "", "", "" };
            }
            else
            {
                RecievingUser.Invitations.Add(NewInvitation);
                _context.SaveChanges();
                ViewBag.values = new List<string> { "", "You have sent an invitation to " + RecievingUser.Name + "!", "", "", "", "" };
            }
            // ADD: need to make it only users who are not you and that you are not connected/invited with.
            List<User> AllUsers = _context.User.OrderByDescending(o => o.CreatedAt).ToList();
            ViewBag.allusers = AllUsers;
            return View("Users");
        }

        [HttpGet]
        [Route("/Users/{thisID}/Ignore")]
        public IActionResult Ignore(int thisID)
        {
            ViewBag.errors = new List<string>();
            ViewBag.values = new List<string> { "", "", "", "", "", "" };
            if (HttpContext.Session.GetInt32("myID") == null)
            {
                ViewBag.values = new List<string> { "", "You must login to view this page.", "", "", "", "" };
                return View("Index");
            }

            User MyUser = _context.User.Where(u => u.UserId == (int)(HttpContext.Session.GetInt32("myID")))
                    .Include(i => i.Invitations)
                    .Include(c => c.Connections)
                    .SingleOrDefault();
            MyUser.Invitations = MyUser.Invitations.OrderByDescending(o => o.CreatedAt).ToList();
            Invitations temp = new Invitations();
            foreach (Invitations I in MyUser.Invitations)
            {
                if (I.UserId == thisID)
                {
                    ViewBag.values = new List<string> { "", "You ignored an invitation from "+I.Name+".", "", "", "", "" };
                    temp = I;
                }
            }
            _context.Invitations.Remove(temp);
            _context.SaveChanges();
            // ADD: need to make it only users who are not you and that you are not connected/invited with.
            MyUser.Connections = MyUser.Connections.OrderByDescending(o => o.CreatedAt).ToList();
            ViewBag.myuser = MyUser;
            return RedirectToAction("MyProfile");
        }

        [HttpGet]
        [Route("/Users/{thisID}/AcceptInvite")]
        public IActionResult AcceptInvite(int thisID)
        {
            ViewBag.errors = new List<string>();
            ViewBag.values = new List<string> { "", "", "", "", "", "" };
            if (HttpContext.Session.GetInt32("myID") == null)
            {
                ViewBag.values = new List<string> { "", "You must login to view this page.", "", "", "", "" };
                return View("Index");
            }

            User MyUser = _context.User.Where(u => u.UserId == (int)(HttpContext.Session.GetInt32("myID")))
                    .Include(i => i.Invitations)
                    .Include(c => c.Connections)
                    .SingleOrDefault();
            MyUser.Invitations = MyUser.Invitations.OrderByDescending(o => o.CreatedAt).ToList();
            foreach (Invitations I in MyUser.Invitations)
            {
                if (I.UserId == thisID)
                {
                    ViewBag.values = new List<string> { "", "You added "+I.Name+" to your network!", "", "", "", "" };
                    Connections NewConnection = new Connections()
                    {
                        UserId = I.UserId,
                        Name = I.Name
                    };
                    MyUser.Invitations.Remove(I);
                    MyUser.Connections.Add(NewConnection);
                    _context.SaveChanges();
                }
            }
            _context.SaveChanges();
            // ADD: need to make it only users who are not you and that you are not connected/invited with.
            MyUser.Connections = MyUser.Connections.OrderByDescending(o => o.CreatedAt).ToList();
            ViewBag.myuser = MyUser;
            return RedirectToAction("MyProfile");
        }

        [HttpGet]
        [Route("/Professional_Profile")]
        public IActionResult MyProfile()
        {
            ViewBag.errors = new List<string>();
            ViewBag.values = new List<string> { "", "", "", "", "", "" };
            if (HttpContext.Session.GetInt32("myID") == null)
            {
                ViewBag.values = new List<string> { "", "You must login to view this page.", "", "", "", "" };
                return View("Index");
            }
            User MyUser = _context.User.Where(u => u.UserId == (int)(HttpContext.Session.GetInt32("myID")))
                    .Include(i => i.Invitations)
                    .Include(c => c.Connections)
                    .SingleOrDefault();
            MyUser.Invitations = MyUser.Invitations.OrderByDescending(o => o.CreatedAt).ToList();
            MyUser.Connections = MyUser.Connections.OrderByDescending(o => o.CreatedAt).ToList();
            ViewBag.myuser = MyUser;
            return View();
        }

        // [HttpGet]
        // [Route("Customers/Delete/{myID}")]
        // public IActionResult DeleteCustomer(int myID)
        // {
        //     ViewBag.errors = new List<string>();
        //     ViewBag.values = new List<string> { "", "" };
        //     Customer MyCustomer = _context.Customer.SingleOrDefault(c => c.CustomerId == myID);
        //     if (MyCustomer != null)
        //     {
        //         ViewBag.values = new List<string> { "", "" + MyCustomer.CustomerName + " was deleted." };
        //         _context.Customer.Remove(MyCustomer);
        //         _context.SaveChanges();
        //     }
        //     List<Customer> AllCustomers = _context.Customer.OrderByDescending(o => o.CreatedAt).ToList();
        //     ViewBag.allcustomers = AllCustomers;
        //     return View("Customers");
        // }

    }
}
