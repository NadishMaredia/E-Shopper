﻿using AutoMapper;
using EShop.Services.ShoppingCartAPI.Models;
using EShop.Services.ShoppingCartAPI.Models.Dto;

namespace EShop.Services.ShoppingCartAPI
{
	public class MappingConfig
	{
		public static MapperConfiguration RegisterMaps()
		{
			var mappingConfig = new MapperConfiguration(config =>
			{
                config.CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
                config.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
            });
			return mappingConfig;
		}
	}
}
