﻿using ETicaretAPI.Application.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Queries.Product.GetAllProduct
{
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQueryRequest, GetAllProductQueryResponse>
    {
        readonly IProductReadRepository _productReadRepository;
        ILogger<GetAllProductQueryHandler> _logger;
        public GetAllProductQueryHandler(IProductReadRepository productReadRepository, ILogger<GetAllProductQueryHandler> logger = null)
        {
            _productReadRepository = productReadRepository;
            _logger = logger;
        }
        public async Task<GetAllProductQueryResponse> Handle(GetAllProductQueryRequest request, CancellationToken cancellationToken)
        {
        
            var totalProductCount = _productReadRepository.GetAll(false).Count();
            var products = _productReadRepository.GetAll(false)
                .OrderBy(p => p.CreatedDate)
                .Skip(request.Page * request.Size)
                .Take(request.Size)
                .Include(p=>p.ProductImageFiles)
                .Select(p => new
             {
                 p.Id,
                 p.Name,
                 p.Stock,
                 p.Price,
                 p.CreatedDate,
                 p.UpdatedDate,
                 p.ProductImageFiles
             })
                    .ToList();


            return new()
            {
                Products = products,
                TotalProductCount = totalProductCount
            };
        }
    }
}