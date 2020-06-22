using MailKit.Net.Smtp;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IpWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;        
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<AppConfig> _appConfigOptions;

        public Worker(ILogger<Worker> logger,
            IHttpClientFactory httpClientFactory,
            IOptions<AppConfig> appConfigOptions)
        {
            _logger = logger;            
            _httpClientFactory = httpClientFactory;
            _appConfigOptions = appConfigOptions;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var appConfig = _appConfigOptions.Value;
            var preIpAddr = string.Empty;
            while (!stoppingToken.IsCancellationRequested)
            {
                var wanIpAddr = string.Empty;
                try
                {
                    using (var httpClient = _httpClientFactory.CreateClient())
                    {
                        var response = await httpClient.GetAsync(appConfig.GetWanIpUrl);
                        wanIpAddr = await response.Content.ReadAsStringAsync();
                    }                    

                    if (!string.IsNullOrEmpty(preIpAddr) && 
                        !string.IsNullOrEmpty(wanIpAddr) && 
                        preIpAddr != wanIpAddr)
                    {
                        using (var smtpClient = new SmtpClient())
                        {
                            smtpClient.Connect(appConfig.EmailConfig.Smtp, appConfig.EmailConfig.Port, true);
                            smtpClient.Authenticate(appConfig.EmailConfig.Sender, appConfig.EmailConfig.SenderPwd);
                            var message = new MimeMessage();
                            message.From.Add(new MailboxAddress(string.Empty, appConfig.EmailConfig.Sender));
                            message.To.Add(new MailboxAddress(string.Empty, appConfig.EmailConfig.To));
                            message.Subject = "Ip���֪ͨ";
                            message.Body = new TextPart()
                            {
                                Text = $"����IP�����:{wanIpAddr}"
                            };
                            smtpClient.Send(message);
                            smtpClient.Disconnect(true);
                        }
                    }
                    preIpAddr = wanIpAddr;
                }
                catch (Exception e)
                {
                    _logger.LogInformation($"{DateTimeOffset.Now} �����쳣: {e.Message}");
                }
                _logger.LogInformation($"{DateTimeOffset.Now} ip: {wanIpAddr}");
                await Task.Delay(appConfig.MillisecondsDelay, stoppingToken);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now}: Worker started.");

            return base.StartAsync(cancellationToken);
        }


        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{DateTime.Now}:Worker stopped. ");

            return base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _logger.LogInformation($"{DateTime.Now}:Worker disposed.");

            base.Dispose();
        }
    }
}
