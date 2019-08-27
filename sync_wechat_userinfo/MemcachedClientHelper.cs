using Enyim.Caching;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Transcoders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sync_wechat_userinfo
{
    public class MemcachedClientHelper
    {
        public const string TestObjectKey = "Hello_World";

        protected virtual MemcachedClient GetClient(IServiceCollection services, MemcachedProtocol protocol = MemcachedProtocol.Binary, bool useBinaryFormatterTranscoder = false)
        {
            services.AddEnyimMemcached(options =>
            {
                options.AddServer("memcached", 11211);
                options.Protocol = protocol;
                //if (useBinaryFormatterTranscoder)
                //{
                //    options.Transcoder = "BinaryFormatterTranscoder";
                //}
            });
            if (useBinaryFormatterTranscoder)
            {
                services.AddSingleton<ITranscoder, BinaryFormatterTranscoder>();
            }

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            var client = serviceProvider.GetService<IMemcachedClient>() as MemcachedClient;
            client.Remove("VALUE");
            return client;
        }

    }
}
