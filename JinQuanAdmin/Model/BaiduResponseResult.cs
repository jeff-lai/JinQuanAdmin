using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinQuanAdmin.Model
{
    public enum BaiduResponseResult
    {
        [Description("无")]
        None=0,
        [Description("是")]
        Included=1,
        [Description("查询结果空，疑似代理异常")]
        ProxyException = 2,
        [Description("IP黑名单,已重查10次失败")]
        IpBlackIntercept=3,
        [Description("查询异常，代理服务器不稳定")]
        Exception =4,

    }
}
