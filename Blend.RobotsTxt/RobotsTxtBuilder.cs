﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Cms.Core.Configuration.Models;
using Umbraco.Extensions;

namespace Blend.RobotsTxt
{
    public interface IRobotsBuilder
    {
        string BuildRobots();
    }

    public class RobotsTxtBuilder : IRobotsBuilder
    {
        private readonly List<RobotsTxtOption> _config;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RobotsTxtBuilder(IOptions<List<RobotsTxtOption>> config, IWebHostEnvironment webHostEnvironment)
        {
            _config = config.Value;
            
            _webHostEnvironment = webHostEnvironment;
        }

        public string BuildRobots()
        {
            var stringBuilder = new StringBuilder();


            if (_config.Any())
            {
                // Load Robots.txt with what is in the appsettings.
                foreach (var robot in _config)
                {
                    // User Agent
                    var userAgent = robot.UserAgent;
                    if (userAgent.IsNullOrWhiteSpace())
                    {
                        userAgent = "*";
                    }
                    stringBuilder.AppendLine($"User-agent: {userAgent}");

                    // Allow
                    if (robot.Disallow.IsCollectionEmpty() && robot.Allow.IsCollectionEmpty())
                    {
                        stringBuilder.AppendLine($"Allow: /");
                    }
                    if (!robot.Allow.IsCollectionEmpty())
                    {
                        foreach (var item in robot.Allow)
                        {
                            if (!item.IsNullOrWhiteSpace())
                                stringBuilder.AppendLine($"Allow: {item}");
                        }
                    }

                    // Disallow
                    if (!robot.Disallow.IsCollectionEmpty())
                    {
                        foreach (var item in robot.Disallow)
                        {
                            if (!item.IsNullOrWhiteSpace())
                                stringBuilder.AppendLine($"Disallow: {item}");
                        }
                    }

                    // Sitemap
                    if (!robot.Sitemap.IsNullOrWhiteSpace())
                    {
                        stringBuilder.AppendLine($"Sitemap: {robot.Sitemap}");
                    }
                    stringBuilder.AppendLine("");
                }
            }
            else if (!_webHostEnvironment.EnvironmentName.IsNullOrWhiteSpace())
            {
                // Load Robots.txt by environment.
                switch (_webHostEnvironment.EnvironmentName)
                {
                    case "Production":
                        stringBuilder.AppendLine("User-agent: *");
                        stringBuilder.AppendLine("Allow: /");
                        break;
                    default:
                        stringBuilder.AppendLine("User-agent: *");
                        stringBuilder.AppendLine("Disallow: /");
                        break;
                }
                
            }
            else
            {
                // Last Resort and display production robots.txt.
                stringBuilder.AppendLine("User-agent: *");
                stringBuilder.AppendLine("Allow: /");
            }
            var robotsTxt = stringBuilder.ToString();
            return robotsTxt;
        }
    }
}
