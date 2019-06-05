using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace verint_service.Controllers.HealthCheck.Models
{
    public class HealthCheckModel
    {
        public string AppVersion { get; set; }

        public string Name { get; set; }
    }
}