using Enterprise.Application.Features.Products.DTOs;
using Enterprise.Domain.Entities;
using Enterprise.Domain.ValueObjects;
using Mapster;

namespace Enterprise.Application.Mappings;

/// <summary>
/// Mapster configuration for mapping domain entities to DTOs.
/// </summary>
public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<SkuCode, string>().MapWith(src => src.Value);
        config.NewConfig<Money, decimal>().MapWith(src => src.Amount);

        config.NewConfig<Product, ProductDto>()
            .Map(dest => dest.Sku, src => src.Sku.Value)
            .Map(dest => dest.Price, src => src.Price.Amount)
            .Map(dest => dest.Currency, src => src.Price.Currency);
    }
}
