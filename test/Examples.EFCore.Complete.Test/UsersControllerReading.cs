using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Examples.EFCore.Complete.Test
{
	[Binding]
	public sealed class UsersControllerReadingSteps : Steps
	{
		private readonly ServiceProvider _serviceProvider;

		private Controllers.UsersController _usersController;
		private Models.Page<Models.User> _pageUser;

		private UsersControllerReadingSteps()
		{
			var services = new ServiceCollection();
			services.AddDbContext<Context>(options => options
				.UseInMemoryDatabase(nameof(UsersControllerReadingSteps))
			);
			services.AddScoped<IContext, Context>();
			services.AddLogging(options => options
				.SetMinimumLevel(LogLevel.None)
			);
			services.AddAutoMapper(typeof(Startup));
			services.AddTransient<Controllers.UsersController>();

			_serviceProvider = services.BuildServiceProvider();
		}

		[Given(@"I am an administrator")]
		public void GivenIAmAnAdministrator()
		{
			_usersController = _serviceProvider.GetRequiredService<Controllers.UsersController>();
		}

		[Given(@"the default set of users exist")]
		public async Task GivenTheDefaultSetOfUsersExist()
		{
			var context = _serviceProvider.GetRequiredService<Context>();

			await Program.PopulateDbIfEmpty(context, Program.Get100RandomUsers);
		}

		[When(@"I read users")]
		public async Task WhenIReadUsers()
		{
			_pageUser = await _usersController.ReadAll(null, CancellationToken.None);
		}

		[When(@"I read (.*) users?")]
		public async Task WhenIReadUser(int numberOf)
		{
			_pageUser = await _usersController.ReadAll(new Models.PageQuery
			{
				Limit = numberOf,
			}, CancellationToken.None);
		}

		[Then(@"the results should have (.*) users?")]
		public async Task ThenTheResultShouldHaveUsers(int numberOf)
		{
			var context = _serviceProvider.GetRequiredService<Context>();

			Assert.AreEqual(numberOf, _pageUser.Results.Count());
			Assert.AreEqual(await context.Users.CountAsync(), _pageUser.TotalCount);
		}

		[Then(@"the results should be ordered by '(.*)'")]
		public void ThenTheResultShouldBeOrderedBy(string orderBys)
		{
			var context = _serviceProvider.GetRequiredService<Context>();
			var expected = context.Users
				.OrderBy(_pageUser.OrderBy)
				.ThenBy(u => u.Id)
				.Skip(_pageUser.Offset)
				.Take(_pageUser.Limit)
				.Select(u => u.Id);

			StringAssert.AreEqualIgnoringCase(orderBys, _pageUser.OrderBy);
			CollectionAssert.AreEqual(expected, _pageUser.Results.Select(u => u.Id));
		}
	}
}