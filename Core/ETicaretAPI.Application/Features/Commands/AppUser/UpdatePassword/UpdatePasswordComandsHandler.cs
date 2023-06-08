using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.UpdatePassword
{
    public class UpdatePasswordComandsHandler : IRequestHandler<UpdatePasswordComandsRequest, UpdatePasswordComandsResponse>
    {
        readonly IUserService _userService;

        public UpdatePasswordComandsHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UpdatePasswordComandsResponse> Handle(UpdatePasswordComandsRequest request, CancellationToken cancellationToken)
        {
            if (!request.Password.Equals(request.PasswordConfirm))
                throw new PasswordChangeFailedException("Lütfen birebir şifreyi doğrulayınız.");
            await _userService.UpdatePasswordAsync(request.UserId, request.ResetToken, request.Password);
            return new();
        }
    }
}
