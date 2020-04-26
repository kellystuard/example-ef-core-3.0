using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace Examples.EFCore.Complete
{
	public class AutoMapping : Profile
	{
		public AutoMapping()
		{
			CreateMap<Models.User, Data.User>()
				.ReverseMap();
			CreateMap<Models.UserEdit, Data.User>();
		}
	}
}
