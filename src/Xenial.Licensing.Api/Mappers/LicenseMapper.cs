using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;

using DevExpress.Xpo;

using Xenial.Licensing.Api.Controllers;
using Xenial.Licensing.Model;

namespace Xenial.Licensing.Api.Mappers
{

    /// <summary>
    /// Extension methods to map to/from entity/model for API resources.
    /// </summary>
    public static class LicenseMapper
    {
        internal static IMapper Mapper { get; }
            = new MapperConfiguration(cfg => cfg.AddProfile<LicenseMapperProfile>())
                .CreateMapper();

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static OutLicenseModel ToModel(this GrantedLicense entity)
            => entity == null ? null : Mapper.Map<OutLicenseModel>(entity);

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static GrantedLicense ToEntity(this OutLicenseModel model, Session session)
            => model == null ? null : Mapper.Map<GrantedLicense>(model, opt => opt.ConstructServicesUsing(t => session.GetClassInfo(t).CreateObject(session)));
    }
}
