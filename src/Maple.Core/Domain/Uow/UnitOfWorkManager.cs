using Maple.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain.Uow
{
    /// <summary>
    /// 工作单元管理器的默认实现。用于创建一个新的工作单元或获取当前处于激活状态的工作单元
    /// </summary>
    internal class UnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public IActiveUnitOfWork Current
        {
            get { return _currentUnitOfWorkProvider.Current; }
        }

        public UnitOfWorkManager(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
        }

        public IUnitOfWorkCompleteHandle Begin()
        {
            return Begin(getDefaultUnitOfWorkOptions());
        }

        /// <summary>
        /// 开始了一个新的工作单元
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public IUnitOfWorkCompleteHandle Begin(UnitOfWorkOptions options)
        {
            if (options == null)
                options = this.getDefaultUnitOfWorkOptions();

            var outerUow = _currentUnitOfWorkProvider.Current;
            if (outerUow != null)
                return new InnerUnitOfWorkCompleteHandle();

            var uow = EngineContext.Current.Resolve<IUnitOfWork>();

            uow.Completed += (sender, args) =>
            {
                _currentUnitOfWorkProvider.Current = null;
            };

            uow.Failed += (sender, args) =>
            {
                _currentUnitOfWorkProvider.Current = null;
            };

            uow.Disposed += (sender, args) =>
            {

            };

            uow.Begin(options);

            _currentUnitOfWorkProvider.Current = uow;

            return uow;
        }

        protected UnitOfWorkOptions getDefaultUnitOfWorkOptions()
        {
            return new UnitOfWorkOptions()
            {
                IsTransactional = true
            };
        }
    }
}
