using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Examples.EFCore.Complete.Controllers
{
    // conforms to the following REST specifications: https://restfulapi.net/http-methods/
    [ApiController]
    //[Route("[controller]")]
    [Route("users")]
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
        public IActionResult ReadAll()
        {
            var users = _context.Users.Where(u => u.Visible).ToArray();

            return StatusCode(200, users);
        }

        [HttpGet("{id}")]
        public IActionResult ReadSingle(string id)
        {
            int realId;
            if (int.TryParse(id, out realId) == false)
                return StatusCode(400);
                //return this.ValidationProblem(ModelState);

            var users = _context.Users.ToArray();
            var user = users.Where(u => u.Visible && u.Id == realId).FirstOrDefault();
            if (user == null)
                return StatusCode(404);

            return StatusCode(200, user);
        }

        [HttpPost]
        public IActionResult Create(Models.User user)
        {
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

            Response.Headers.Add("Location",  new UriBuilder(Request.Scheme, Request.Host.Host, Request.Host.Port ?? (Request.IsHttps ? 443 : 80), Request.Path + newUser.Id).ToString());
            return StatusCode(201, newUser);

            //return CreatedAtAction("ReadSingle", new { id = user.Id });
        }

        [HttpPut("{id}")]
        public IActionResult CreateOrUpdate(string id, Models.User user)
        {
            int realId;
            if (int.TryParse(id, out realId) == false)
                return StatusCode(400);
                //return this.ValidationProblem(ModelState);

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

            return StatusCode(newUser ? 201 : 200, updateUser);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            int realId;
            if (int.TryParse(id, out realId) == false)
                return StatusCode(400);
                //return this.ValidationProblem(ModelState);

            var users = _context.Users.ToArray();
            var user = users.Where(u => u.Visible && u.Id == realId).FirstOrDefault();
            if (user == null)
                return StatusCode(404);

            _context.Users.Remove(user);

            _context.SaveChanges();

            return StatusCode(204);
        }

        [HttpPatch("{id}")]
        //[Consumes("application/merge-patch+json")]
        public IActionResult Patch(string id, Models.User user)
        {
            int realId;
            if (int.TryParse(id, out realId) == false)
                return StatusCode(400);
                //return this.ValidationProblem(ModelState);

            var users = _context.Users.ToArray();
            var updateUser = users.Where(u => u.Visible && u.Id == realId).FirstOrDefault();
            if (updateUser == null)
                return StatusCode(404);

            if (Request.Form.ContainsKey(nameof(user.FirstName)))
                updateUser.FirstName = user.FirstName;
            if (Request.Form.ContainsKey(nameof(user.FirstName)))
                updateUser.LastName = user.FirstName;

            _context.Users.Update(updateUser);
            _context.SaveChanges();

            return StatusCode(200, updateUser);
        }
    }
}