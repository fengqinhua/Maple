using Maple.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Maple.Core.Tests.Infrastructure
{
    public class TypeFinderTests
    {
        [Fact]
        public void TypeFinder_Benchmark_Findings()
        {
            var finder = new AppDomainTypeFinder();

            var type = finder.FindClassesOfType<ISomeInterface>();

            Assert.Equal(type.Count(), 1);
            Assert.True(typeof(ISomeInterface).IsAssignableFrom(type.FirstOrDefault()));
        }

        public interface ISomeInterface
        {
        }

        public class SomeClass : ISomeInterface
        {
        }
    }
}
