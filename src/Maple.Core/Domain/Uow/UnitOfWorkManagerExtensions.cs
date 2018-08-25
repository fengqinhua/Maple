using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain.Uow
{
    public static class UnitOfWorkManagerExtensions
    {
        public static T ExecuteWithUOW<T>(this IUnitOfWorkManager unitOfWorkManager, Func<T> func, UnitOfWorkOptions options = null)
        {
            if (unitOfWorkManager.Current != null)
            {
                //如果当前已经在工作单元中，则直接执行被拦截类的方法
                return func();
            }
            else
            {
                if (options == null)
                    options = new UnitOfWorkOptions() { IsTransactional = true };

                using (var uow = unitOfWorkManager.Begin(options))
                {
                    var result = func();
                    uow.Complete();
                    return result;
                }
            }
        }

    }
}
