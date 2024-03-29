﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Vidly.Models;
using Vidly.ViewModels;

namespace Vidly.Controllers {
	public class MoviesController : Controller {
		private ApplicationDbContext _context;

		public MoviesController() {
			_context = new ApplicationDbContext ();
		}

		protected override void Dispose(bool disposing) {
			_context.Dispose ();
		}

		public ActionResult Index() {
			return View ();
		}

		[Route ("movies/released/{year}/{month:regex(\\d{2}):range(1, 12)}")]
		public ActionResult ByReleaseDate(int year, int month) {
			return Content (year + "/" + month);
		}

		public ActionResult Details(int id) {
			var movie = _context.Movies.Include (m => m.Genre).SingleOrDefault (m => m.Id == id);
			if (movie == null) {
				return HttpNotFound ();
			}
			return View (movie);
		}

		public ActionResult NewMovie() {
			var genres = _context.Genres.ToList ();
			var viewModel = new MovieFormViewModel {
				Genres = genres
			};
			return View ("MovieForm", viewModel);
		}

		[HttpPost]
		public ActionResult UpdateMovie(Movie movie) {
			if (!ModelState.IsValid) {
				var viewModel = new MovieFormViewModel (movie) {
					Genres = _context.Genres.ToList ()
				};

				return View ("MovieForm", viewModel);
			}

			if (movie.Id == 0) {
				movie.DateAdded = DateTime.Now;
				_context.Movies.Add (movie);
			} else {
				var movieFromDB = _context.Movies.SingleOrDefault (c => c.Id == movie.Id);
				movieFromDB.Name = movie.Name;
				movieFromDB.ReleaseDate = movie.ReleaseDate;
				movieFromDB.GenreId = movie.GenreId;
				movieFromDB.NumberInStock = movie.NumberInStock;
			}
			_context.SaveChanges ();

			return RedirectToAction ("Index", "Movies");
		}

		public ActionResult EditMovie(int id) {
			var movie = _context.Movies.SingleOrDefault (m => m.Id == id);

			if (movie == null) {
				return HttpNotFound ();
			}

			var viewModel = new MovieFormViewModel (movie) {
				Genres = _context.Genres.ToList ()
			};
			return View ("MovieForm", viewModel);
		}
	}
}