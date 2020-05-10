﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Emprise.Admin.Models.Config;
using Emprise.Domain.Core.Interfaces;
using Emprise.Domain.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Emprise.Admin.Pages.Config
{
    public class IndexModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly AppConfig _appConfig;
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        private IDatabase _db;
        public IndexModel(IMapper mapper, IOptionsMonitor<AppConfig> appConfig, ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _mapper = mapper;
            _appConfig = appConfig.CurrentValue;
            _logger = logger;
            _configuration = configuration;

            var redis = ConnectionMultiplexer.Connect(_configuration.GetConnectionString("Redis"));
            _db = redis.GetDatabase();
        }

        public string ErrorMessage { get; set; }

        public List<ConfigDto> ConfigDtos { get; set; }

        [BindProperty]
        public string UrlReferer { get; set; }

        public Dictionary<string, string> Configs = new Dictionary<string, string>();


        public async Task OnGetAsync()
        {
            UrlReferer = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(UrlReferer))
            {
                UrlReferer = Url.Page("/Config/Index");
            }

            ConfigDtos = new List<ConfigDto>();

            var configurations = _db.HashGetAll("configurations");
            Configs = configurations.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());


            GetInfoPropertys(typeof(AppConfig));

        }

        public void GetInfoPropertys(Type type , string parentName="")
        {


            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo prop in props)
            {
             
                string name = prop.Name;
                string key = string.IsNullOrEmpty(parentName) ? $"{name}" : $"{parentName}:{name}";
               
                if (prop.PropertyType.IsValueType || prop.PropertyType.Name.StartsWith("String"))
                {
                    Configs.TryGetValue(key, out string value);
                    var attribute = prop.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;
                    ConfigDtos.Add(new ConfigDto
                    {
                        Key = key,
                        Name = attribute?.DisplayName ?? name,
                        Value = value, 
                        Type = prop.PropertyType
                    });
                }
                else
                {
                    GetInfoPropertys(prop.PropertyType, key);
                }

            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ErrorMessage = "";
            if (!ModelState.IsValid)
            {
                ErrorMessage = ModelState.Where(e => e.Value.Errors.Count > 0).Select(e => e.Value.Errors.First().ErrorMessage).First();
                return Page();
            }
            try
            {
                SetValueFrom(typeof(AppConfig));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "");
            }


            foreach (var config in Configs)
            {
                _db.HashSet("configurations", config.Key, config.Value);
            }

       

            return Redirect(UrlReferer);
        }


        public void SetValueFrom(Type type, string parentName = "")
        {
            PropertyInfo[] props = type.GetProperties();
            foreach (PropertyInfo prop in props)
            {
                string name = prop.Name;
                string key = string.IsNullOrEmpty(parentName) ? $"{name}" : $"{parentName}:{name}";
                if (prop.PropertyType.IsValueType || prop.PropertyType.Name.StartsWith("String"))
                {
                    var value = Request.Form[key];
                    Configs.TryAdd(key, value);
                }
                else
                {
                    SetValueFrom(prop.PropertyType,key);

                }

            }

        }
    }
}