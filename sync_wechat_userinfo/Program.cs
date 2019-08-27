using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Polly;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Transcoders;
using Enyim.Caching;

namespace sync_wechat_userinfo
{
    class Program
    {
        static void Main(string[] args) => Run();

        private static HttpClient client = null;

        private static MemcachedClient memcached = null;

        public static async void Run()
        {
            Console.WriteLine("========sync start========");
            try
            {
                Console.WriteLine("初始化HttpClientFactory");
                IServiceCollection services = new ServiceCollection();
                services.AddEnyimMemcached(options =>
                {
                    options.AddServer("127.0.0.1", 11212);
                    options.Protocol = MemcachedProtocol.Binary;
                });
                services.AddSingleton<ITranscoder, BinaryFormatterTranscoder>();
                services.AddLogging();

                services.AddHttpClient("wechat", options => { });

                IServiceProvider serviceProvider = services.BuildServiceProvider();
                memcached = serviceProvider.GetService<IMemcachedClient>() as MemcachedClient;
                memcached.Remove("VALUE");

                var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
                client = httpClientFactory.CreateClient();

                Console.WriteLine("开始获取需要同步的OpenID");
                SyncOpenids();
                Console.WriteLine("开始同步用户信息");
                SyncUserInfo();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异常：{ex.Message}");
            }
            Console.WriteLine("========sync end ========");
            Console.ReadKey();
        }

        static bool SyncOpenids()
        {
            string access_token = GetAccessToken();

            if (string.IsNullOrEmpty(access_token)) { throw new Exception("access_token未获取到"); }

            var lastopenid = db.fsql.Select<WeChatSyncQueue>()
                .OrderByDescending(s => s.id)
                .Limit(1).First();

            bool isread = false;
            string next_openid = lastopenid?.openid; ;
            int runIndex = 0;//总计运行数量
            do
            {
                runIndex++;
                //计时开始
                int countStart = System.Environment.TickCount;

                //获取所有关注用户的openid
                string openid_geturl = "https://api.weixin.qq.com/cgi-bin/user/get?access_token=" + access_token + (string.IsNullOrEmpty(next_openid) ? "" : "&next_openid=" + next_openid);
                var response = client.SendAsync(new HttpRequestMessage(HttpMethod.Get, openid_geturl)).Result;
                var content = response.Content.ReadAsStringAsync().Result;
                WeChatSyncResult result = Newtonsoft.Json.JsonConvert.DeserializeObject<WeChatSyncResult>(content);
                next_openid = result.next_openid;
                Console.WriteLine($"获取OpenID,第{runIndex}/{Convert.ToInt32(result.total / 10000)}次数");
                if (result.total == 0)
                {
                    Console.WriteLine("失败：{Newtonsoft.Json.JsonConvert.SerializeObject(result)}");
                }
                else
                {
                    if (result.data == null || result.data.openid.Count() == 0)
                    {
                        Console.WriteLine("没有需要读取的openid了");
                        isread = false;
                    }
                    else
                    {
                        //确定最后一页
                        if (result.count != 10000)
                            isread = false;
                        else isread = true;

                        List<WeChatSyncQueue> openids = new List<WeChatSyncQueue>();
                        int idx = 0;
                        int runidx = 0;
                        foreach (var openid in result.data.openid)
                        {
                            idx++;
                            runidx++;
                            openids.Add(new WeChatSyncQueue
                            {
                                openid = openid,
                                createtime = DateTime.Now,
                                issync = 0,
                                synctime = null
                            });

                            if (idx >= 1000 || runidx >= result.data.openid.Count())
                            {
                                var res_count = db.fsql.Insert<WeChatSyncQueue>().AppendData(openids).ExecuteAffrows();
                                Console.WriteLine($"批量新增同步队列，预计：{idx}，实际：{res_count}");
                                openids.Clear();
                                idx = 0;
                            }
                        }
                    }
                }
                int countEnd = System.Environment.TickCount - countStart;
                Console.WriteLine($"获取OpenID,第{runIndex + "/" + Convert.ToInt32(result.total / 10000)}次数， 耗时：{countEnd}");

            } while (isread);

            return true;
        }

        static object objlock = new object();
        public static bool SyncUserInfo()
        {
            for (int i = 0; i < Convert.ToInt32(db.config["setting:ThreadPoolCount"]); i++)
            {
                //使用多线程读取
                WaitCallback waitCallback = t =>
                {
                    Console.WriteLine("任务：{0}，线程：{1}，任务开始", t, Thread.CurrentThread.ManagedThreadId);
                    bool iscontinue = true;

                    while (iscontinue)
                    {
                        List<WeChatSyncQueue> models = new List<WeChatSyncQueue>();
                        lock (objlock)
                        {
                            models = db.fsql.Select<WeChatSyncQueue>().Where(w => w.issync == 0).Limit(100).ToList();
                            string opids = "'" + string.Join("','", models.Select(s => s.openid).ToArray()) + "'";
                            //models.ForEach(p => p.issync = 1);
                            db.fsql.Update<WeChatSyncQueue>().SetSource(models).Set(a => a.synctime, DateTime.Now).Set(a => a.issync, 2).ExecuteAffrows();// ("update wechatopenidlist set issync=1,synctime=now() where openid in (" + opids + ")");

                            if (models.Count() == 0) iscontinue = false;
                        }
                        foreach (var model in models)
                        {
                            if (model != null)
                            {
                                model.issync = 1;
                                model.synctime = DateTime.Now;
                                db.fsql.Update<WeChatSyncQueue>().SetSource(model).ExecuteAffrows();
                                try
                                {
                                    var userinfo = GetUserInfo(model.openid);
                                    if (userinfo != null)
                                    {
                                        db.fsql.Insert<WeChatUserInfo>(userinfo).ExecuteAffrows();
                                        Console.WriteLine("任务：{0}，线程：{1}，执行中-成功，openid：{2}", t, Thread.CurrentThread.ManagedThreadId, model.openid);
                                    }
                                    else
                                    {
                                        db.fsql.Insert<WeChatUserInfo>(new WeChatUserInfo
                                        {
                                            jsonstring = "同步异常",
                                            openid = model.openid
                                        }).ExecuteAffrows();
                                        Console.WriteLine("任务：{0}，线程：{1}，执行中-失败，openid：{2}", t, Thread.CurrentThread.ManagedThreadId, model.openid);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    db.fsql.Insert<WeChatUserInfo>(new WeChatUserInfo
                                    {
                                        jsonstring = "同步异常",
                                        openid = model.openid
                                    }).ExecuteAffrows();
                                    Console.WriteLine("任务：{0}，线程：{1}，执行中-异常，openid：{2}", t, Thread.CurrentThread.ManagedThreadId, model.openid);
                                }
                            }
                            else
                            {
                                iscontinue = false;
                                Console.WriteLine("任务：{0}，线程：{1}，任务结束", t, Thread.CurrentThread.ManagedThreadId);
                            }
                        }
                    }

                    Console.WriteLine("当前参数是{0},当前线程是{1}", t, Thread.CurrentThread.ManagedThreadId);
                };

                ThreadPool.QueueUserWorkItem(waitCallback, i);
            }
            return true;
        }

        public static string GetAccessToken()
        {
            try
            {
                string key = "GetWxCatInfo_AccessToken";
                //判断缓存是否存在
                string access_token = memcached.Get<string>(key);
                if (string.IsNullOrEmpty(access_token))
                {
                    string appid = db.config["wechat:appid"];
                    string secret = db.config["wechat:secret"];
                    //获取最新的token
                    string url = $"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={appid}&secret={secret}";
                    var response = client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)).Result;
                    var content = response.Content.ReadAsStringAsync().Result;
                    OAuthAccessTokenJsonResult result = Newtonsoft.Json.JsonConvert.DeserializeObject<OAuthAccessTokenJsonResult>(content);
                    if (result.errcode != 0)
                    {
                        Console.WriteLine("失败：" + Newtonsoft.Json.JsonConvert.SerializeObject(result));
                        return "";
                    }
                    access_token = result.access_token;
                    memcached.Add(key, result.access_token, 120);
                }
                return access_token;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "";
        }

        /// <summary>
        /// 通过OpenID获取微信用户信息
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        public static WeChatUserInfo GetUserInfo(string openId)
        {
            try
            {
                string accessToken = GetAccessToken();
                var url = $"https://api.weixin.qq.com/cgi-bin/user/info?access_token={accessToken}&openid={openId}&lang=zh_CN";
                var response = client.SendAsync(new HttpRequestMessage(HttpMethod.Get, url)).Result;
                var content = response.Content.ReadAsStringAsync().Result;
                return Newtonsoft.Json.JsonConvert.DeserializeObject<WeChatUserInfo>(content);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
