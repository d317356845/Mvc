// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.AspNetCore.Mvc
{
    public class DesignTimeMvcServiceCollectionProvider
    {
        public static IServiceCollection PopulateServiceCollection(IServiceCollection services, string assemblyName)
        {
            var hostingEnvironment = new HostingEnvironment
            {
                ApplicationName = assemblyName
            };

            services.AddSingleton<IHostingEnvironment>(hostingEnvironment);
            services.AddMvc();

            return services;
        }

        private class HostingEnvironment : IHostingEnvironment
        {
            public string EnvironmentName { get; set; } = Hosting.EnvironmentName.Production;

            public string ApplicationName { get; set; }

            public string WebRootPath { get; set; }

            public IFileProvider WebRootFileProvider { get; set; }

            public string ContentRootPath { get; set; }

            public IFileProvider ContentRootFileProvider { get; set; }
        }
    }
}
