namespace sync_wechat_userinfo
{
    /// <summary>
    /// 微信公众平台接口通用返回信息
    /// </summary>
    public class WeChatJsonResult
    {
        public int errcode { get; set; }
        public string errmsg { get; set; }

        public string ChineseMsg()
        {
            string msg = "请求成功";
            switch (errcode)
            {
                case -1:
                    msg = "系统繁忙";
                    break;
                case 40001:
                    msg = "获取access_token时AppSecret错误，或者access_token无效";
                    break;
                case 40002:
                    msg = "不合法的凭证类型 ";
                    break;
                case 40003:
                    msg = "不合法的OpenID ";
                    break;
                case 40004:
                    msg = "不合法的媒体文件类型";
                    break;
                case 40005:
                    msg = "不合法的文件类型";
                    break;
                case 40006:
                    msg = "不合法的文件大小";
                    break;
                case 40007:
                    msg = "不合法的媒体文件id";
                    break;
                case 40008:
                    msg = "不合法的消息类型";
                    break;
                case 40009:
                    msg = "不合法的图片文件大小";
                    break;
                case 40010:
                    msg = "不合法的语音文件大小";
                    break;
                case 40011:
                    msg = "不合法的视频文件大小";
                    break;
                case 40012:
                    msg = "不合法的缩略图文件大小";
                    break;
                case 40013:
                    msg = "不合法的APPID";
                    break;
                case 40014:
                    msg = "不合法的access_token";
                    break;
                case 40015:
                    msg = "不合法的菜单类型";
                    break;
                case 40016:
                    msg = "不合法的按钮个数";
                    break;
                case 40017:
                    msg = "不合法的按钮个数";
                    break;
                case 40018:
                    msg = "不合法的按钮名字长度";
                    break;
                case 40019:
                    msg = "不合法的按钮KEY长度";
                    break;
                case 40020:
                    msg = "不合法的按钮URL长度";
                    break;
                case 40021:
                    msg = "不合法的菜单版本号";
                    break;
                case 40022:
                    msg = "不合法的子菜单级数";
                    break;
                case 40023:
                    msg = "不合法的子菜单按钮个数";
                    break;
                case 40024:
                    msg = "不合法的子菜单按钮类型";
                    break;
                case 40025:
                    msg = "不合法的子菜单按钮名字长度";
                    break;
                case 40026:
                    msg = "不合法的子菜单按钮KEY长度";
                    break;
                case 40027:
                    msg = "不合法的子菜单按钮URL长度";
                    break;
                case 40028:
                    msg = "不合法的自定义菜单使用用户";
                    break;
                case 40029:
                    msg = "不合法的oauth_code";
                    break;
                case 40030:
                    msg = "不合法的refresh_token";
                    break;
                case 40031:
                    msg = "不合法的openid列表";
                    break;
                case 40032:
                    msg = "不合法的openid列表长度";
                    break;
                case 40033:
                    msg = @"不合法的请求字符，不能包含\uxxxx格式的字符";
                    break;
                case 40035:
                    msg = "不合法的参数";
                    break;
                case 40038:
                    msg = "不合法的请求格式";
                    break;
                case 40039:
                    msg = "不合法的URL长度";
                    break;
                case 40050:
                    msg = "不合法的分组id";
                    break;
                case 40051:
                    msg = "分组名字不合法";
                    break;
                case 41001:
                    msg = "缺少access_token参数";
                    break;
                case 41002:
                    msg = "缺少appid参数";
                    break;
                case 41003:
                    msg = "缺少refresh_token参数";
                    break;
                case 41004:
                    msg = "缺少secret参数";
                    break;
                case 41005:
                    msg = "缺少多媒体文件数据";
                    break;
                case 41006:
                    msg = "缺少media_id参数";
                    break;
                case 41007:
                    msg = "缺少子菜单数据";
                    break;
                case 41008:
                    msg = "缺少oauth code";
                    break;
                case 41009:
                    msg = "缺少openid";
                    break;
                case 42001:
                    msg = "access_token超时";
                    break;
                case 42002:
                    msg = "refresh_token超时";
                    break;
                case 42003:
                    msg = "oauth_code超时";
                    break;
                case 43001:
                    msg = "需要GET请求";
                    break;
                case 43002:
                    msg = "需要POST请求";
                    break;
                case 43003:
                    msg = "需要HTTPS请求";
                    break;
                case 43004:
                    msg = "需要接收者关注";
                    break;
                case 43005:
                    msg = "需要好友关系";
                    break;
                case 44001:
                    msg = "多媒体文件为空";
                    break;
                case 44002:
                    msg = "POST的数据包为空";
                    break;
                case 44003:
                    msg = "图文消息内容为空";
                    break;
                case 44004:
                    msg = "文本消息内容为空";
                    break;
                case 45001:
                    msg = "多媒体文件大小超过限制";
                    break;
                case 45002:
                    msg = "消息内容超过限制";
                    break;
                case 45003:
                    msg = "标题字段超过限制";
                    break;
                case 45004:
                    msg = "描述字段超过限制";
                    break;
                case 45005:
                    msg = "链接字段超过限制";
                    break;
                case 45006:
                    msg = "图片链接字段超过限制";
                    break;
                case 45007:
                    msg = "语音播放时间超过限制";
                    break;
                case 45008:
                    msg = "图文消息超过限制";
                    break;
                case 45009:
                    msg = "接口调用超过限制";
                    break;
                case 45010:
                    msg = "创建菜单个数超过限制";
                    break;
                case 45015:
                    msg = "回复时间超过限制";
                    break;
                case 45016:
                    msg = "系统分组，不允许修改";
                    break;
                case 45017:
                    msg = "分组名字过长";
                    break;
                case 45018:
                    msg = "分组数量超过上限";
                    break;
                case 46001:
                    msg = "不存在媒体数据";
                    break;
                case 46002:
                    msg = "不存在的菜单版本";
                    break;
                case 46003:
                    msg = "不存在的菜单数据";
                    break;
                case 46004:
                    msg = "不存在的用户";
                    break;
                case 47001:
                    msg = "解析JSON/XML内容错误";
                    break;
                case 48001:
                    msg = "api功能未授权";
                    break;
                case 50001:
                    msg = "用户未授权该api";
                    break;
            }
            return msg;
        }
    }
    /// <summary>
    /// 获取OAuth AccessToken的结果
    /// 如果错误，返回结果{"errcode":40029,"errmsg":"invalid code"}
    /// </summary>
    public class OAuthAccessTokenJsonResult : WeChatJsonResult
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string openid { get; set; }
        public string scope { get; set; }
    }


}