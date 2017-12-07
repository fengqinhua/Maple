
namespace Maple.Core.Domain
{
    public class AggregateRoot : AggregateRoot<long>, IAggregateRoot
    {

    }

    public class AggregateRoot<TPrimaryKey> : Entity<TPrimaryKey>
    {
        public AggregateRoot()
        {
        }
    }
}