using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Examples.EFCore.DIY.Controllers
{
	// conforms to the following REST specifications: https://restfulapi.net/http-methods/
	[ApiController]
	[Route("users")]
	public sealed class UsersController : ControllerBase
	{
		private readonly ILogger _logger;
		private readonly Context _context;

		public UsersController(ILogger<UsersController> logger, Context context)
		{
			_logger = logger;
			_context = context;
		}

		[HttpGet]
		public IActionResult ReadAll()
		{
			_logger.LogInformation("Getting all users");
			var users = _context.Users.Where(u => u.Visible).ToArray();

			var results = new List<Models.User>();
			foreach (var user in users)
			{
				results.Add(new Models.User()
				{
					Id = user.Id,
					FirstName = user.FirstName,
					LastName = user.LastName,
					Email = user.Email,
					LoginCount = _context.UserLogins.Where(ul => ul.User.Id == user.Id).Count(),
				});
			}

			_logger.LogInformation("Returning all users");
			return StatusCode(200, results);
		}

		[HttpGet("{id}")]
		public IActionResult ReadSingle(string id)
		{
			_logger.LogInformation("Getting user {Id}", id);
			int realId;
			if (int.TryParse(id, out realId) == false)
				return StatusCode(400);

			var users = _context.Users.ToArray();
			var user = users.Where(u => u.Visible && u.Id == realId).FirstOrDefault();
			if (user == null)
				return StatusCode(404);

			var result = new Models.User()
			{
				Id = user.Id,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				LoginCount = _context.UserLogins.Where(ul => ul.User.Id == user.Id).Count(),
			};

			_logger.LogInformation("Returning user {Id}", id);
			return StatusCode(200, result);
		}

		[HttpPost]
		public IActionResult Create(Models.User user)
		{
			_logger.LogInformation("Creating user {Id}", user.Id);
			if (user.FirstName == null)
				return StatusCode(400, "firstName cannot be null");
			if (user.LastName == null)
				return StatusCode(400, "lastName cannot be null");
			if (user.Email == null)
				return StatusCode(400, "email cannot be null");

			var newUser = new Data.User()
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
			};
			_context.Users.Add(newUser);

			_context.SaveChanges();

			var savedUser = new Models.User()
			{
				FirstName = newUser.FirstName,
				LastName = newUser.LastName,
				Email = newUser.Email,
			};

			Response.Headers.Add("Location", new UriBuilder(Request.Scheme, Request.Host.Host, Request.Host.Port ?? (Request.IsHttps ? 443 : 80), Request.Path + newUser.Id).ToString());
			_logger.LogInformation("Created user {Id}", user.Id);
			return StatusCode(201, savedUser);
		}

		[HttpPut("{id}")]
		public IActionResult Update(string id, Models.User user)
		{
			_logger.LogInformation("Updating user {Id}", user.Id);
			int realId;
			if (int.TryParse(id, out realId) == false)
				return StatusCode(400);

			if (user.FirstName == null)
				return StatusCode(400, "firstName cannot be null");
			if (user.LastName == null)
				return StatusCode(400, "lastName cannot be null");
			if (user.Email == null)
				return StatusCode(400, "email cannot be null");

			var users = _context.Users.ToArray();
			var updateUser = users.Where(u => u.Visible && u.Id == realId).FirstOrDefault();
			if (updateUser == null)
				return StatusCode(400, "Unable to create users with PUT.");

			updateUser.FirstName = user.FirstName;
			updateUser.LastName = user.LastName;
			updateUser.Email = user.Email;

			_context.Users.Update(updateUser);
			_context.SaveChanges();

			var savedUser = new Models.User()
			{
				FirstName = updateUser.FirstName,
				LastName = updateUser.LastName,
				Email = updateUser.Email,
			};

			_logger.LogInformation("Updated user {Id}", user.Id);
			return StatusCode(200, savedUser);
		}

		[HttpDelete("{id}")]
		public IActionResult Delete(string id)
		{
			_logger.LogInformation("Deleting user {Id}", id);
			int realId;
			if (int.TryParse(id, out realId) == false)
				return StatusCode(400);

			var users = _context.Users.ToArray();
			var user = users.Where(u => u.Visible && u.Id == realId).FirstOrDefault();
			if (user == null)
				return StatusCode(404);

			_context.Users.Remove(user);

			_context.SaveChanges();

			_logger.LogInformation("Deleted user {Id}", id);
			return StatusCode(204);
		}
	}
}