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

        public LoginUserCommandHandler(UserManager<p.AppUser> userManager, SignInManager<p.AppUser> signInManage)
        {
            _userManager = userManager;
            _signInManage = signInManage;
        }

        public async Task<LoginUserCommandResponse> Handle(LoginUserCommandRequest request, CancellationToken cancellationToken)
        {
            p.AppUser user = await _userManager.FindByNameAsync(request.UsernameOrEmail);
            if (user == null)
                user = await _userManager.FindByEmailAsync(request.UsernameOrEmail);
            if (user == null)
                throw new NotFoundUserExceptions("Kullanıcı veya Şifre Hatalı....");
            SignInResult result = await _signInManage.CheckPasswordSignInAsync(user, request.Password, false);
            if (result.Succeeded) //Authentication başarılı
            { //yetkileri belirlememiz gerekiyor
            }
            return new();

        }
    }
}
