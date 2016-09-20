// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Razor.Compilation.TagHelpers;

namespace Microsoft.AspNetCore.Mvc.Razor
{
    public class ViewComponentTagHelperDescriptorProvider
    {
        public static IEnumerable<TagHelperDescriptor> GetDescriptorsFromAssembly(string assemblyName)
        {
            var assembly = Assembly.Load(new AssemblyName(assemblyName));

            var partManager = new ApplicationPartManager();
            partManager.ApplicationParts.Add(new AssemblyPart(assembly));
            partManager.FeatureProviders.Add(new ViewComponentFeatureProvider());

            var viewComponentDescriptorProvider = new DefaultViewComponentDescriptorProvider(partManager);
            var viewComponentTagHelperDescriptorFactory = new ViewComponentTagHelperDescriptorFactory(
                viewComponentDescriptorProvider);

            var descriptors = viewComponentTagHelperDescriptorFactory.CreateDescriptors(assemblyName);

            return descriptors;
        }
    }
}
