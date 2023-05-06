using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using p = ETicaretAPI.Domain.Entities.Identity;

namespace ETicaretAPI.Application.Features.Commands.AppUser.LoginUser
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommandRequest, LoginUserCommandResponse>
    {
        readonly UserManager<p.AppUser> _userManager;
        readonly SignInManager<p.AppUser> _signInManage;
        readonly ITokenHandler _tokenHandler;

        public LoginUserCommandHandler(UserManager<p.AppUser> userManager, SignInManager<p.AppUser> signInManage, ITokenHandler tokenHandler)
        {
            _userManager = userManager;
            _signInManage = signInManage;
            _tokenHandler = tokenHandler;
        }

        public async Task<LoginUserCommandResponse> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
        {
            p.AppUser user = await _userManager.FindByNameAsync(request.UsernameOrEmail);
            if (user == null)
                user = await _userManager.FindByEmailAsync(request.UsernameOrEmail);
            if (user == null)
                throw new NotFoundUserExceptions();
            SignInResult result = await _signInManage.CheckPasswordSignInAsync(user, request.Password, false);
            if (result.Succeeded) //Authentication başarılı
            { //yetkileri belirlememiz gerekiyor
                Token token = _tokenHandler.CreateAccessToken(5);
                return new LoginUserSuccessCommandResponse() { Token= token };
            }
            throw new AuthenticationErrorException();


        }
    }
}
