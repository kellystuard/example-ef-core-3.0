using System.Linq;
using AutoMapper;

namespace Examples.EFCore.Complete
{
	/// <summary>
	/// Provides to AutoMapper a named configuration for maps.
	/// </summary>
	public class AutoMapping : Profile
	{
		/// <summary>
		/// Creates an instance of <see cref="AutoMapping"/>.
		/// </summary>
		public AutoMapping()
		{
			CreateMap<Models.User, Data.User>();
			CreateMap<Models.UserEdit, Data.User>();
			CreateMap<Data.User, Models.User>()
				.ForMember(m => m.LoginCount, opt => opt.MapFrom(d => d.Logins.Count));
		}
	}
}