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
			CreateMap<Models.User, Data.User>()
				.ReverseMap();
			CreateMap<Models.UserEdit, Data.User>();
		}
	}
}