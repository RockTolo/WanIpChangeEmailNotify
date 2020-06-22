using System;
using System.Collections.Generic;
using System.Text;

namespace IpWorkerService
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// worker延迟毫秒数
        /// </summary>
        public int MillisecondsDelay { get; set; }

        /// <summary>
        /// 获取外网的IP接口地址
        /// </summary>
        public string GetWanIpUrl { get; set; }

        /// <summary>
        /// 邮箱配置
        /// </summary>
        public EmailConfig EmailConfig { get; set; }
    }

    /// <summary>
    /// 邮箱配置
    /// </summary>
    public class EmailConfig
    { 
        /// <summary>
        /// smtp
        /// </summary>
        public string Smtp { get; set; }

        /// <summary>
        /// smtp 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 发件人邮箱地址
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// 发件人密码
        /// </summary>
        public string SenderPwd { get; set; }

        /// <summary>
        /// 接收人邮箱地址
        /// </summary>
        public string To { get; set; }
    }
}
