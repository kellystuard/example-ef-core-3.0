using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using System.Data;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
		private readonly IMapper _mapper;
		private readonly IContext _context;

		/// <summary>
		/// Creates a new UsersController.
		/// </summary>
		/// <param name="logger">Represents a type used to perform logging.</param>
		/// <param name="mapper">Represents a types used to copy property values from one object type to another.</param>
		/// <param name="context">Represents a session with the database and can be used to query and save instances of your entities.</param>
		public UsersController(ILogger<UsersController> logger, IMapper mapper, IContext context)
		{
			_logger = logger;
			_mapper = mapper;
			_context = context;
		}

		/// <summary>
		/// Reads a paged list of users.
		/// </summary>
		/// <param name="page">Parameters passed to control paging of the results.</param>
		/// <param name="cancellationToken">Injected by MVC and signaled if the current request is canceled.</param>
		/// <param name="firstName">Filters users to those that starts with this first name.</param>
		/// <param name="lastName">Filters users to those that starts with this last name.</param>
		/// <param name="email">Filters users to those that starts with this email.</param>
		/// <returns>Page of users.</returns>
		[HttpGet, Transactional(IsolationLevel.ReadCommitted)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<Models.Page<Models.User>> ReadAll([FromQuery]Models.PageQuery page, CancellationToken cancellationToken,
			string? firstName = null, string? lastName = null, string? email = null)
		{
			page ??= new Models.PageQuery();
			page.OrderBy ??= $"{nameof(Models.User.LastName)},{nameof(Models.User.FirstName)}";

			var query = _context.Users.AsNoTracking();

			// sort
			query = query.OrderBy(page.OrderBy).ThenBy(u => u.Id);

			// filter
			if (firstName != null)
				query = query.Where(u => u.FirstName.StartsWith(firstName));
			if (lastName != null)
				query = query.Where(u => u.LastName.StartsWith(lastName));
			if (email != null)
				query = query.Where(u => u.Email.StartsWith(email));

			// page and map
			var users = page.Limit == 0 ?
				Enumerable.Empty<Models.User>() :
				await query
					.Skip(page.Offset)
					.Take(page.Limit)
					.ProjectTo<Models.User>(_mapper.ConfigurationProvider)
					.ToArrayAsync(cancellationToken);

			// count is done from query without page and map
			var totalCount = await query.CountAsync(cancellationToken);

			return new Models.Page<Models.User>(users, totalCount, page.Limit, page.Offset, page.OrderBy);
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

			return _mapper.Map<Models.User>(user);
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
		public async Task<ActionResult<Models.User>> Create(Models.UserEdit user, CancellationToken cancellationToken)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			var newUser = _mapper.Map<Data.User>(user);
			_context.Users.Add(newUser);

			await _context.SaveChangesAsync(cancellationToken);
			var savedUser = _mapper.Map<Models.User>(newUser);

			return CreatedAtAction(nameof(ReadSingle), new { id = newUser.Id }, savedUser);
		}

		/// <summary>
		/// Overwrites the existing user at the specified location.
		/// </summary>
		/// <param name="id">Resource identifier of the user.</param>
		/// <param name="user">Data to be used to update the existing user.</param>
		/// <param name="cancellationToken">Injected by MVC and signaled if the current request is canceled.</param>
		/// <returns>New or overwritten user. If newly-created user, location header will contain URL.</returns>
		[HttpPut("{id}"), Transactional(IsolationLevel.RepeatableRead)]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<Models.User>> Update(int id, Models.UserEdit user, CancellationToken cancellationToken)
		{
			if (user == null)
				throw new ArgumentNullException(nameof(user));

			var updateUser = await _context.Users.FindAsync(id);
			if (updateUser == null)
			{
				ModelState.AddModelError(string.Empty, "Unable to create users with PUT.");
				return ValidationProblem(ModelState);
			}
			_mapper.Map(user, updateUser);

			await _context.SaveChangesAsync(cancellationToken);
			var savedUser = _mapper.Map<Models.User>(updateUser);

			return savedUser;
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