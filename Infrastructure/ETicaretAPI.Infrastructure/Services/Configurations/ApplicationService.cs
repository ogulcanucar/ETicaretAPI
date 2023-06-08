using ETicaretAPI.Application.Abstractions.Services.Configurations;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.DTOs.Configurations;
using ETicaretAPI.Application.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.ComponentModel.Design;
using System.Reflection;

namespace ETicaretAPI.Infrastructure.Services.Configurations
{
    public class ApplicationService : IApplicationService
    {
        public List<Menu> GetAuthorizeDefinitionEndpoints(Type type)
        {
            Assembly assembly = Assembly.GetAssembly(type);
            var controllers = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(ControllerBase)));
            List<Menu> menus = new();

            if (controllers == null)
                return menus;

            foreach (var controller in controllers)
            {
                var actions = controller.GetMethods().Where(m => m.IsDefined(typeof(AuthorizeDefinitionAttribute)));

                if (actions == null)
                    continue;

                foreach (var action in actions)
                {
                    var attributes = action.GetCustomAttributes(true);

                    if (attributes == null)
                        continue;

                    var authorizeDefinitionAttributes = attributes.FirstOrDefault(a => a.GetType() == typeof(AuthorizeDefinitionAttribute)) as AuthorizeDefinitionAttribute;

                    if (!menus.Any(m => m.Name == authorizeDefinitionAttributes.Menu))
                    {
                        var menu = new Menu { Name = authorizeDefinitionAttributes.Menu };
                        menus.Add(menu);
                    }

                    var existingMenu = menus.FirstOrDefault(m => m.Name == authorizeDefinitionAttributes.Menu);
                    var _action = new Application.DTOs.Configurations.Action
                    {
                        ActionType = Enum.GetName(typeof(ActionType), authorizeDefinitionAttributes.ActionType),
                        Definition = authorizeDefinitionAttributes.Definition,
                    };

                    var httpAttribute = attributes.FirstOrDefault(a => a.GetType().IsAssignableTo(typeof(HttpMethodAttribute))) as HttpOptionsAttribute;

                    if (httpAttribute != null)
                        _action.HttpType = httpAttribute.HttpMethods.First();
                    else
                        _action.HttpType = HttpMethods.Get;

                    _action.Code= $"{_action.HttpType}.{_action.ActionType}.{_action.Definition.Replace(" ", "")}";

                    existingMenu.Actions.Add(_action);
                }
            }

            return menus;
        }



    }
}
