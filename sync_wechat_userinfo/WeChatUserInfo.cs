using System;
using System.Collections.Generic;
using System.Text;
using FreeSql.DataAnnotations;

namespace sync_wechat_userinfo
{
    public class WeChatUserInfo
    {
        [Column(IsIdentity = true, IsPrimary = true)]
        public long id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createtime { get; set; } = DateTime.Now;

        /// <summary>
        /// 关注状态 
        /// </summary>
        public int subscribe { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// 例如：刘学满
        /// </summary>
        public string nickname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int sex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string language { get; set; }
        /// <summary>
        /// 例如：广州
        /// </summary>
        public string city { get; set; }
        /// <summary>
        /// 例如：广东
        /// </summary>
        public string province { get; set; }
        /// <summary>
        /// 例如：中国
        /// </summary>
        public string country { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string headimgurl { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int subscribe_time { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int groupid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public List<string> tagid_list { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string subscribe_scene { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int qr_scene { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string qr_scene_str { get; set; }

        /// <summary>
        /// 完整微信用户json数据
        /// </summary>
        public string jsonstring { get; set; }
    }
}
