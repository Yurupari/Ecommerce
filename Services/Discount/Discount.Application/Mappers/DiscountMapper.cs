﻿using AutoMapper;

namespace Discount.Application.Mappers
{
    public class DiscountMapper
    {
        private static readonly Lazy<IMapper> Lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.ShouldMapProperty = p => p.GetMethod.IsPublic || p.GetMethod.IsAssembly;
                cfg.AddProfile<DiscountProfile>();
            });

            return config.CreateMapper();
        });

        public static IMapper Mapper => Lazy.Value;
    }
}
