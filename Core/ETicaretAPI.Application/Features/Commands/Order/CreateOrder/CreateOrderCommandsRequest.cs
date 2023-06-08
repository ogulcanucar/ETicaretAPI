using MediatR;

namespace ETicaretAPI.Application.Features.Commands.Order.CreateOrder
{
    public class CreateOrderCommandsRequest:IRequest<CreateOrderCommandsResponse>
    {
        public string Description { get; set; }
        public string Address { get; set; }
    }
}