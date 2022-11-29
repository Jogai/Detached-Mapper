﻿using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Detached.Mappers.EntityFramework.Configuration
{
    public class EFMapperDbContextOptionsExtension : IDbContextOptionsExtension
    {
        readonly EFMapperConfigurationBuilder _configBuilder;

        public EFMapperDbContextOptionsExtension(EFMapperConfigurationBuilder configBuilder)
        {
            Info = new EFMapperDbContextOptionsExtensionInfo(this);
            _configBuilder = configBuilder;
        }

        internal EFMapperServiceProvider Profiles { get; set; }

        public DbContextOptionsExtensionInfo Info { get; }

        public void ApplyServices(IServiceCollection services)
        {
            services.AddSingleton(new EFMapperServiceProvider(_configBuilder));
        }

        public void Validate(IDbContextOptions options)
        {
        }
    }
}