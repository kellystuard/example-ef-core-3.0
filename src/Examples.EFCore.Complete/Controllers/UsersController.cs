using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Examples.EFCore.Complete.Controllers
{
    // conforms to the following REST specifications: https://restfulapi.net/http-methods/
    [ApiController]
    [Route("[controller]")]
    public sealed class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly Context _context;

        public UsersController(ILogger<UsersController> logger, Context context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<Models.Page<Models.User>> ReadAll(int limit = 10, int offset = 0)
        {
            var query = _context.Users;
            var users = await query.Skip(offset).Take(limit).ToArrayAsync();
            var totalCount = await query.CountAsync();

            return new Models.Page<Models.User>
            {
                Results = users,
                TotalCount = totalCount,
                Limit = limit,
                Offset = offset,
            };
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Models.User>> ReadSingle(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<Models.User>> Create(Models.User user)
        {
            var newUser = new Models.User()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
            };
            _context.Users.Add(newUser);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ReadSingle), new { id = newUser.Id }, newUser);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Models.User>> CreateOrUpdate(int id, Models.User user)
        {
            var updateUser = await _context.Users.FindAsync(id);
            var newUser = (updateUser == null);
            updateUser ??= _context.Users.IgnoreQueryFilters().Add(new Models.User()).Entity;

            updateUser.FirstName = user.FirstName;
            updateUser.LastName = user.LastName;

            await _context.SaveChangesAsync();

            if (newUser)
                return CreatedAtAction(nameof(ReadSingle), new { id = updateUser.Id }, updateUser);

            return updateUser;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}