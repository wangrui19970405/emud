﻿using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Dm.Model.V20151123;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Emprise.Infra.Providers
{
    public class AliyunMail : IMail
    {
        private readonly ILogger<AliyunMail> _logger;
        private readonly AppConfig _appConfig;
        public AliyunMail(IOptionsMonitor<AppConfig> appConfig, ILogger<AliyunMail> logger)
        {
            _appConfig = appConfig.CurrentValue;
            _logger = logger;
        }


        public async Task Send(MailModel message)
        {
            await Task.Run(() => {
                IClientProfile profile = DefaultProfile.GetProfile(_appConfig.Aliyun.RegionId, _appConfig.Aliyun.AccessKeyId, _appConfig.Aliyun.AccessKeySecret);
                IAcsClient client = new DefaultAcsClient(profile);
                SingleSendMailRequest request = new SingleSendMailRequest();
                try
                {
                    request.AccountName = _appConfig.Email.AccountName;
                    request.FromAlias = _appConfig.Email.FromAlias;
                    request.AddressType = 1;
                    request.ReplyToAddress = true;
                    request.ToAddress = message.Address;
                    request.Subject = message.Subject;
                    request.HtmlBody = "1111111111111";
                    SingleSendMailResponse httpResponse = client.GetAcsResponse(request);
                }
                catch (ServerException ex)
                {
                    _logger.LogError(ex, "ServerException");
                    throw ex;
                }
                catch (ClientException ex)
                {
                    _logger.LogError(ex, "ClientException");
                    throw ex;
                }
            });
        }


    }
}
