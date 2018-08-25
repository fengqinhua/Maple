using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain.Uow
{

    public class UnitOfWorkFailedEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }

        public UnitOfWorkFailedEventArgs(Exception exception)
        {
            Exception = exception;
        }
    }
}
