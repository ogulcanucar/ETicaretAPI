﻿using MediatR;

namespace ETicaretAPI.Application.Features.Queries.Role.GetRolesById
{
    public class GetRoleByIdQueryRequest:IRequest<GetRoleByIdQueryResponse>
    {
        public string Id { get; set; }
    }
}