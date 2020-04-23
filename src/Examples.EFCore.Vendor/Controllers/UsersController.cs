using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Z.EntityFramework.Plus;

namespace Examples.EFCore.Vendor.Controllers
{
    // conforms to the following REST specifications: https://restfulapi.net/http-methods/
    [ApiController]
    [Route("[controller]")]
    public sealed class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IContext _context;

        public UsersController(ILogger<UsersController> logger, IContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<Models.Page<Models.User>> ReadAll(CancellationToken cancellationToken, int limit = 10, int offset = 0)
        {
            var query = _context.Users;
            var users = query
                .Skip(offset)
                .Take(limit)
                .Future();
                //.ToArrayAsync(cancellationToken);
            var totalCount = query.DeferredCount().FutureValue(); //CountAsync(cancellationToken);

            return new Models.Page<Models.User>
            {
                Results = users,
                TotalCount = totalCount,
                Limit = limit,
                Offset = offset,
            };
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Models.User>> ReadSingle(CancellationToken cancellationToken, int id)
        {
            var user = await _context.Users.FindAsync(id, cancellationToken);
            if (user == null)
                return NotFound();

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<Models.User>> Create(CancellationToken cancellationToken, Models.User user)
        {
            var newUser = new Models.User()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
            _context.Users.Add(newUser);

            await _context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction(nameof(ReadSingle), new { id = newUser.Id }, newUser);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Models.User>> CreateOrUpdate(CancellationToken cancellationToken, int id, Models.User user)
        {
            var updateUser = await _context.Users.FindAsync(id, cancellationToken);
            var newUser = (updateUser == null);
            updateUser ??= _context.Users.Add(new Models.User()).Entity;

            updateUser.FirstName = user.FirstName;
            updateUser.LastName = user.LastName;
            updateUser.Visible = true;

            await _context.SaveChangesAsync(cancellationToken);

            if (newUser)
                return CreatedAtAction(nameof(ReadSingle), new { id = updateUser.Id }, updateUser);

            return updateUser;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(CancellationToken cancellationToken, int id)
        {
            var user = await _context.Users.FindAsync(id, cancellationToken);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);

            await _context.SaveChangesAsync(cancellationToken);

            return NoContent();
        }
    }
}