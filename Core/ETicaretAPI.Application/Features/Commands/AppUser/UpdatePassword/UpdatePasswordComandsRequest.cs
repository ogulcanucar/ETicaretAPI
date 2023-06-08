using MediatR;

namespace ETicaretAPI.Application.Features.Commands.AppUser.UpdatePassword
{
    public class UpdatePasswordComandsRequest:IRequest<UpdatePasswordComandsResponse>
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
        public string ResetToken { get; set; }
    }
}