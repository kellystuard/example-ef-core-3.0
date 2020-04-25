using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Examples.EFCore.Complete.Controllers
{
	/// <summary>
	/// Handles all web-service operations related to <see cref="Models.User"/>s.
	/// </summary>
	/// <remarks>
	/// Conforms to the following REST specifications: https://restfulapi.net/http-methods/
	/// </remarks>
	[ApiController]
	[Route("[controller]")]
	public sealed class UsersController : ControllerBase
	{
		private readonly ILogger _logger;
		private readonly Context _context;

		/// <summary>
		/// Creates a new UsersController.
		/// </summary>
		/// <param name="logger">Represents a type used to perform logging.</param>
		/// <param name="context">Represents a session with the database and can be used to query and save instances of your entities.</param>
		public UsersController(ILogger<UsersController> logger, Context context)
		{
			_logger = logger;
			_context = context;
		}

		/// <summary>
		/// Reads a page of users, starting at <paramref name="offset"/> and returning a maximum count of <paramref name="limit"/>.
		/// </summary>
		/// <param name="cancellationToken">Injected by MVC and signaled if the current request is canceled.</param>
		/// <param name="limit">Maximum number of users to return.</param>
		/// <param name="offset">Zero-based offset of the first user to return.</param>
		/// <returns>Page of users.</returns>
		[HttpGet, Transactional(IsolationLevel.ReadCommitted)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<Models.Page<Models.User>> ReadAll(CancellationToken cancellationToken, [Range(0, int.MaxValue)] int limit = 10, [Range(0, int.MaxValue)] int offset = 0)
		{
			var query = _context.Users;
			var users = limit == 0 ?
				Enumerable.Empty<Models.User>() :
				await query
					.Skip(offset)
					.Take(limit)
					.ToArrayAsync(cancellationToken);
			var totalCount = await query.CountAsync(cancellationToken);

			return new Models.Page<Models.User>(users, totalCount, limit, offset);
		}

		/// <summary>
		/// Reads a single user.
		/// </summary>
		/// <param name="id">Resource identifier of the user.</param>
		/// <returns>Single user.</returns>
		[HttpGet("{id}"), Transactional(IsolationLevel.ReadCommitted)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<Models.User>> ReadSingle(int id)
		{
			var user = await _context.Users.FindAsync(id);
			if (user == null)
				return NotFound();

			return user;
		}

		/// <summary>
		/// Creates a new user and adds it to the list of users.
		/// </summary>
		/// <param name="user">Data to be used to create the new user.</param>
		/// <param name="cancellationToken">Injected by MVC and signaled if the current request is canceled.</param>
		/// <returns>Newly-created user. Location header will contain URL to the newly-created user.</returns>
		[HttpPost, Transactional(IsolationLevel.RepeatableRead)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<Models.User>> Create(Models.User user, CancellationToken cancellationToken)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			var newUser = new Models.User()
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
			};
			_context.Users.Add(newUser);

			await _context.SaveChangesAsync(cancellationToken);

			return CreatedAtAction(nameof(ReadSingle), new { id = newUser.Id }, newUser);
		}

		/// <summary>
		/// Creates a new user at the specified location or overwrites the existing user at the specified location.
		/// </summary>
		/// <param name="id">Resource identifier of the user.</param>
		/// <param name="user">Data to be used to create the new user.</param>
		/// <param name="cancellationToken">Injected by MVC and signaled if the current request is canceled.</param>
		/// <returns>New or overwritten user. If newly-created user, location header will contain URL.</returns>
		[HttpPut("{id}"), Transactional(IsolationLevel.RepeatableRead)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<Models.User>> CreateOrUpdate(int id, Models.User user, CancellationToken cancellationToken)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			var updateUser = await _context.Users.FindAsync(id);
			var newUser = (updateUser == null);
			_logger.LogInformation("User will be {Action}", newUser ? "created" : "updated");

			updateUser ??= _context.Users.Add(new Models.User()).Entity;

			updateUser.FirstName = user.FirstName;
			updateUser.LastName = user.LastName;
			updateUser.Visible = true;

			await _context.SaveChangesAsync(cancellationToken);

			if (newUser)
				return CreatedAtAction(nameof(ReadSingle), new { id = updateUser.Id }, updateUser);

			return updateUser;
		}

		/// <summary>
		/// Deletes the user at the specified location.
		/// </summary>
		/// <param name="id">Resource identifier of the user.</param>
		/// <param name="cancellationToken">Injected by MVC and signaled if the current request is canceled.</param>
		/// <returns>Empty result.</returns>
		[HttpDelete("{id}"), Transactional(IsolationLevel.RepeatableRead)]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
		{
			var user = await _context.Users.FindAsync(id);
			if (user == null)
				return NotFound();

			_context.Users.Remove(user);

			await _context.SaveChangesAsync(cancellationToken);

			return NoContent();
		}
	}
}
