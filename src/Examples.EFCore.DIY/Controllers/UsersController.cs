using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

			_logger.LogInformation("Returning all users");
			return StatusCode(200, users);
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

			_logger.LogInformation("Returning user {Id}", id);
			return StatusCode(200, user);
		}

		[HttpPost]
		public IActionResult Create(Models.User user)
		{
			_logger.LogInformation("Creating user {Id}", user.Id);
			if (user.FirstName == null)
				return StatusCode(400, "firstName cannot be null");
			if (user.LastName == null)
				return StatusCode(400, "lastName cannot be null");

			var newUser = new Models.User()
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				Visible = true,
			};
			_context.Users.Add(newUser);

			_context.SaveChanges();

			Response.Headers.Add("Location", new UriBuilder(Request.Scheme, Request.Host.Host, Request.Host.Port ?? (Request.IsHttps ? 443 : 80), Request.Path + newUser.Id).ToString());
			_logger.LogInformation("Created user {Id}", user.Id);
			return StatusCode(201, newUser);
		}

		[HttpPut("{id}")]
		public IActionResult CreateOrUpdate(string id, Models.User user)
		{
			_logger.LogInformation("Creating or updating user {Id}", user.Id);
			int realId;
			if (int.TryParse(id, out realId) == false)
				return StatusCode(400);

			if (user.FirstName == null)
				return StatusCode(400, "firstName cannot be null");
			if (user.LastName == null)
				return StatusCode(400, "lastName cannot be null");

			var users = _context.Users.ToArray();
			var updateUser = users.Where(u => u.Visible && u.Id == realId).FirstOrDefault();
			bool newUser = (updateUser == null);

			if (updateUser == null)
				updateUser = new Models.User();

			updateUser.FirstName = user.FirstName;
			updateUser.LastName = user.LastName;

			if (newUser)
				_context.Users.Add(updateUser);
			else
				_context.Users.Update(updateUser);

			_context.SaveChanges();

			_logger.LogInformation((newUser ? "Created" : "Updated") + " user {Id}", user.Id);
			return StatusCode(newUser ? 201 : 200, updateUser);
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