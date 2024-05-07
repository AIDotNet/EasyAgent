using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using SqlSugar;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EasyAgent.Doamin.Common.DependencyInjection
{
    public static class InitExtensions
    {
        /// <summary>
        /// 使用codefirst创建数据库表
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static WebApplication CodeFirst(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
           
            }
            return app;
        }
    }
}
