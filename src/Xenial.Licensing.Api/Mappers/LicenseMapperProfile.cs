
using AutoMapper;

using Xenial.Licensing.Api.Controllers;
using Xenial.Licensing.Model;

namespace Xenial.Licensing.Api.Mappers
{
    public class LicenseMapperProfile : Profile
    {
        public LicenseMapperProfile()
            => CreateMap<License, OutLicenseModel>()
                .ReverseMap()
                .ConstructUsingServiceLocator();
    }
}
