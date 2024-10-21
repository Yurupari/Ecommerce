﻿using Basket.Application.Commands;
using Basket.Application.Mappers;
using Basket.Application.Responses;
using Basket.Core.Entities;
using Basket.Core.Repositories;
using MediatR;

namespace Basket.Application.Handlers
{
    public class CreateShoppingCartCommandHandler : IRequestHandler<CreateShoppingCartCommand, ShoppingCartResponse>
    {
        private readonly IBasketRepository _basketRepository;

        public CreateShoppingCartCommandHandler(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        public async Task<ShoppingCartResponse> Handle(CreateShoppingCartCommand request, CancellationToken cancellationToken)
        {
            // TODO: Will be integrating to Discount service
            var shoppingCartEntity = BasketMapper.Mapper.Map<ShoppingCart>(request);
            var shoppingCart = await _basketRepository.UpdateBasket(shoppingCartEntity);

            return BasketMapper.Mapper.Map<ShoppingCartResponse>(shoppingCart);
        }
    }
}
