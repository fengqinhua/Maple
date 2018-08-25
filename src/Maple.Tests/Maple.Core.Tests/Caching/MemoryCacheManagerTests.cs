using Maple.Core.Caching;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Maple.Core.Tests.Caching
{
    public class MemoryCacheManagerTests
    {
        [Fact]
        public void Can_set_and_get_object_from_cache()
        {
            var cacheManager = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()));
            cacheManager.Set("some_key_1", 3, int.MaxValue);
 
            Assert.True(cacheManager.Get<int>("some_key_1") == 3);
        }

        [Fact]
        public void Can_validate_whetherobject_is_cached()
        {
            var cacheManager = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()));
            cacheManager.Set("some_key_1", 3, int.MaxValue);
            cacheManager.Set("some_key_2", 4, int.MaxValue);

            Assert.True(cacheManager.IsSet("some_key_1") == true);
            Assert.True(cacheManager.IsSet("some_key_2")== true);
        }

        [Fact]
        public void Can_clear_cache()
        {
            var cacheManager = new MemoryCacheManager(new MemoryCache(new MemoryCacheOptions()));
            cacheManager.Set("some_key_1", 3, int.MaxValue);

            cacheManager.Clear();

            Assert.True(cacheManager.IsSet("some_key_1") == false);
        }
    }
}
