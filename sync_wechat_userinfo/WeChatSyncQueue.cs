using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace sync_wechat_userinfo
{
    [Serializable]
    public class WeChatSyncQueue
    {
        /// <summary>
        /// 队列编号
        /// </summary>
        [Column(IsIdentity = true, IsPrimary = true)]
        public long id { get; set; }

        /// <summary>
        /// 微信Openid
        /// </summary>
        public string openid { get; set; }

        /// <summary>
        /// 同步状态（1是0否）
        /// </summary>
        public int issync { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createtime { get; set; }

        /// <summary>
        /// 同步时间
        /// </summary>
        public DateTime? synctime { get; set; }
    }

    public class WeChatSyncResult
    {
        public int total { get; set; }

        public int count { get; set; }

        public WeChatSyncData data { get; set; }

        public string next_openid { get; set; }

    }

    public class WeChatSyncData
    {
        public List<string> openid { get; set; }
    }


}
