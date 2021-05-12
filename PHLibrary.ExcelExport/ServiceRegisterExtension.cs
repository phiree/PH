using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace PHLibrary.ExcelExport
{
  public static  class ServiceRegisterExtension
    {
        public static IServiceCollection AddPHExcelExport(this IServiceCollection services)
        {
            services.AddScoped<IExcelCreator,ExcelCreatorEPPlus>();
            return services;
        }
    }
}
