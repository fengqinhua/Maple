using Maple.Core.Data.DataProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain.Uow
{
    /// <summary>
    /// UnitOfWork扩展方法
    /// </summary>
    public static class MapleUnitOfWorkExtensions
    {
        public static IDataProvider GetDataProvider(this IActiveUnitOfWork unitOfWork)  
        {
            return GetDataProvider(unitOfWork, string.Empty);
        }

        public static IDataProvider GetDataProvider(this IActiveUnitOfWork unitOfWork,string dataSettingName) 
        {
            Check.NotNull(unitOfWork, nameof(unitOfWork));

            if (!(unitOfWork is MapleUnitOfWork))
                throw new ArgumentException("unitOfWork is not type of " + typeof(MapleUnitOfWork).FullName, nameof(MapleUnitOfWork));

            return (unitOfWork as MapleUnitOfWork).GetOrCreateDataProvider(dataSettingName);
        }
    }
}
