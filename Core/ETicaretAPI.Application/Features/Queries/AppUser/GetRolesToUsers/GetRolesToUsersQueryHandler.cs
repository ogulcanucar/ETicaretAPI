using ETicaretAPI.Application.Abstractions.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.AppUser.GetRolesToUsers
{
    public class GetRolesToUsersQueryHandler : IRequestHandler<GetRolesToUsersQueryRequest, GetRolesToUsersQueryResponse>
    {
        readonly IUserService _userService;

        public GetRolesToUsersQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<GetRolesToUsersQueryResponse> Handle(GetRolesToUsersQueryRequest request, CancellationToken cancellationToken)
        {
            var userRoles = await _userService.GetRolesToUserAsync(request.UserId);
            return new()
            {
                UserRoles = userRoles
            };
        }
    }
}
