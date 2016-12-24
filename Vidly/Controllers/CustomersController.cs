using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vidly.Models;
using Vidly.ViewModels;

namespace Vidly.Controllers {
	public class CustomersController : Controller {
		private ApplicationDbContext _context;

		public CustomersController() {
			_context = new ApplicationDbContext ();
		}

		protected override void Dispose(bool disposing) {
			_context.Dispose ();
		}

		public ActionResult Index() {
			return View ();
		}

		public ActionResult Details(int id) {
			var customer = _context.Customers.Include(c => c.MembershipType).SingleOrDefault (c => c.Id == id);
			if (customer == null) {
				return HttpNotFound ();
			}
			return View (customer);
		}

		public ActionResult NewCustomer() {
			var membershipTypes = _context.MembershipTypes.ToList ();
			var viewModel = new CustomerFormViewModel {
				Customer = new Customer (),
				MembershipTypes = membershipTypes
			};
			return View ("CustomerForm", viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult UpdateCustomer(Customer customer) {
			if (!ModelState.IsValid) {
				var viewModel = new CustomerFormViewModel {
					Customer = customer,
					MembershipTypes = _context.MembershipTypes.ToList ()
				};

				return View ("CustomerForm", viewModel);
			}

			if (customer.Id == 0) {
				_context.Customers.Add (customer);
			} else {
				var customerFromDB = _context.Customers.SingleOrDefault (c => c.Id == customer.Id);

				customerFromDB.Name = customer.Name;
				customerFromDB.DOB = customer.DOB;
				customerFromDB.IsSubscribedToNewsletter = customer.IsSubscribedToNewsletter;
				customerFromDB.MembershipTypeId = customer.MembershipTypeId;
			}
			_context.SaveChanges ();

			return RedirectToAction ("Index", "Customers");
		}

		public ActionResult EditCustomer(int id) {
			var customer = _context.Customers.SingleOrDefault (m => m.Id == id);

			if (customer == null) {
				return HttpNotFound ();
			}

			var viewModel = new CustomerFormViewModel {
				Customer = customer,
				MembershipTypes = _context.MembershipTypes.ToList ()
			};
			return View ("CustomerForm", viewModel);
		}
	}
}