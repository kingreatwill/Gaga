
using System;
using Gaga.Core.Configuration;
using ServiceStack.Caching;
using ServiceStack.Caching.Memcached;

namespace Gaga.Core.Caching
{
	/// <summary>
	/// <!--Enyim.Caching配置（省略了Log4Net框架）   For Memcached-->
	/// </summary>
	public class MemcachedClientCacheManager:ICacheManager
	{
		private ICacheClient _cacheClient;

		public MemcachedClientCacheManager(GagaConfig config)
		{
			//IMemcachedClientConfiguration memcachedClientConfiguration
			this._cacheClient = new MemcachedClientCache(config.MemcachedHosts);
		}

		public void Clear()
		{
			
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public T Get<T>(string key)
		{
			throw new NotImplementedException();
		}

		public bool IsSet(string key)
		{
			throw new NotImplementedException();
		}

		public void Remove(string key)
		{
			throw new NotImplementedException();
		}

		public void RemoveByPattern(string pattern)
		{
			throw new NotImplementedException();
		}

		public void Set(string key, object data, int cacheTime)
		{
			throw new NotImplementedException();
		}
	}
}
