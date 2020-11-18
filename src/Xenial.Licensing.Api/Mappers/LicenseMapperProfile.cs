
using AutoMapper;

using Xenial.Licensing.Api.Controllers;
using Xenial.Licensing.Model;

namespace Xenial.Licensing.Api.Mappers
{
    public class LicenseMapperProfile : Profile
    {
        public LicenseMapperProfile()
            => CreateMap<GrantedLicense, OutLicenseModel>()
                .ForMember(m => m.License, o =>
                {
                    o.Condition(i => i.GeneratedLicense != null);
                    o.MapFrom(i => i.GeneratedLicense);
                })
                .ForMember(m => m.PublicKey, o =>
                {
                    o.Condition(i => i.Key != null);
                    o.MapFrom(i => i.Key.PublicKey);
                })
                .ReverseMap()
                .ConstructUsingServiceLocator();
    }
}
