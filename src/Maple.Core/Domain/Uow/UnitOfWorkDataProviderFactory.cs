using Maple.Core.Data.DataProviders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain.Uow
{
    /// <summary>
    /// 用于工作单元的IDataProviderFactory
    /// </summary>
    /// <typeparam name="TDataProvider"></typeparam>
    public class UnitOfWorkDataProviderFactory : IDataProviderFactory 
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public UnitOfWorkDataProviderFactory(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
        }

        public IDataProvider GetDataProvider()
        {
            return GetDataProvider(string.Empty);
        }

        public IDataProvider GetDataProvider(string dataSettingName)
        {
            return _currentUnitOfWorkProvider.Current.GetDataProvider(dataSettingName);
        }
    }
}
