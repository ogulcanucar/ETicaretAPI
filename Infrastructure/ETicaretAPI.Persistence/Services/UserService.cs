using ETicaretAPI.Application.Abstractions.Services;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Helpers;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;


namespace ETicaretAPI.Persistence.Services;

public class UserService : IUserService
{
    readonly UserManager<AppUser> _userManager;
    readonly IEndpointReadRepository _endpointReadRepository;

    public int TotalUsersCount => _userManager.Users.Count();

    public UserService(UserManager<AppUser> userManager, IEndpointReadRepository endpointReadRepository)
    {
        _userManager = userManager;
        _endpointReadRepository = endpointReadRepository;
    }

    public async Task<CreateUserReponse> CreateAsync(CreateUser model)
    {
        IdentityResult result = await _userManager.CreateAsync(new()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = model.Username,
            Email = model.Email,
            NameSurname = model.NameSurname,
        }, model.Password);

        CreateUserReponse response = new() { Succeeded = result.Succeeded };

        if (result.Succeeded)
            response.Message = "Kullanıcı başarıyla oluşturulmuştur.";
        else
            foreach (var error in result.Errors)
                response.Message += $"{error.Code} - {error.Description}\n";

        return response;
    }

    public async Task UpdateRefreshTokenAsync(string refreshToken, AppUser user, DateTime accessTokenDate, int addOnAccessTokenDate)
    {
        if (user != null)
        {
            user.RefreshToken = refreshToken;
            user.RefreshTokenEndDate = accessTokenDate.AddSeconds(addOnAccessTokenDate);
            await _userManager.UpdateAsync(user);
        }
        else
            throw new NotFoundUserExceptions();
    }
    public async Task UpdatePasswordAsync(string userId, string resetToken, string newPassword)
    {
        AppUser appUser = await _userManager.FindByIdAsync(userId);
        if (appUser != null)
        {
            resetToken = resetToken.UrlDecode();
            IdentityResult result = await _userManager.ResetPasswordAsync(appUser, resetToken, newPassword);
            if (result.Succeeded) await _userManager.UpdateSecurityStampAsync(appUser);
            else
                throw new PasswordChangeFailedException();
        }
    }

    public async Task<List<ListUser>> GetAllUserAsync(int page, int size)
    {
        var users = await _userManager.Users
               .Skip(page * size)
               .Take(size)
               .ToListAsync();
        return users.Select(user => new ListUser
        {
            Id = user.Id,
            Email = user.Email,
            NameSurname = user.NameSurname,
            UserName = user.UserName,
            TwoFactorEnabled = user.TwoFactorEnabled,

        }).ToList();
    }

    public async Task AssignRoleToUserAsync(string userId, string[] roles)
    {
        AppUser user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles);
            await _userManager.AddToRolesAsync(user, roles);
        }
    }

    public async Task<string[]> GetRolesToUserAsync(string userIdOrName)
    {
        AppUser user = await _userManager.FindByIdAsync(userIdOrName);
        if (user == null)
            user = await _userManager.FindByNameAsync(userIdOrName);
        if (user != null)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            return userRoles.ToArray();
        }
        return new string[] { };
    }

    public async Task<bool> HasRolePermissionToEndpointAsync(string name, string code)
    {
        var userRoles = await GetRolesToUserAsync(name);
        if (!userRoles.Any())
            return false;

        Endpoint? endPoint = await _endpointReadRepository.Table
               .Include(e => e.Roles)
               .FirstOrDefaultAsync(e => e.Code == code);
        if (endPoint == null)
            return false;

        var hasRole = false;
        var endpointRoles = endPoint.Roles.Select(r => r.Name);
        foreach (var userRole in userRoles)
        {
            foreach (var endpointRole in endpointRoles)
            {
                if (userRole == endpointRole)
                    return true;
            }
        }
        return false;
    }
}
