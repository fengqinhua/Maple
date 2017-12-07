using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Core.Domain
{
    public interface IAggregateRoot : IAggregateRoot<long>, IEntity
    {

    }

    public interface IAggregateRoot<TPrimaryKey> : IEntity<TPrimaryKey>
    {

    }

}
