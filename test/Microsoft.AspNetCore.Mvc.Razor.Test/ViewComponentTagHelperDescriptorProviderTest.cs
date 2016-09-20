// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Razor.Host;
using Microsoft.AspNetCore.Razor.Compilation.TagHelpers;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.Razor
{
    public class ViewComponentTagHelperDescriptorProviderTest
    {
        [Fact]
        public void GetDescriptorsFromAssembly_ReturnsAllViewComponentsAsTagHelperDescriptors()
        {
            // Act
            var descriptors = ViewComponentTagHelperDescriptorProvider.GetDescriptorsFromAssembly("Microsoft.AspNetCore.Mvc.Razor.Test");

            // Assert
            Assert.Collection(descriptors,
                f => Assert.Equal(GetFirstTagHelperDescriptor(), f, TagHelperDescriptorComparer.Default),
                f => Assert.Equal(GetSecondTagHelperDescriptor(), f, TagHelperDescriptorComparer.Default));
        }

        [Fact]
        public void GetDescriptorsFromAssembly_NoViewComponents_ReturnsEmptyCollection()
        {
            // Act
            var descriptors = ViewComponentTagHelperDescriptorProvider.GetDescriptorsFromAssembly("Microsoft.AspNetCore.Mvc.Razor");

            // Assert
            Assert.Empty(descriptors);
        }

        [Fact]
        public void GetDescriptorsFromAssembly_ThrowsForNonExistentAssembly()
        {
            // Act & Assert
            var exception = Assert.Throws<FileNotFoundException>(
                () => ViewComponentTagHelperDescriptorProvider.GetDescriptorsFromAssembly("FakeAssembly"));
            Assert.Equal(
                "Could not load file or assembly 'FakeAssembly, Culture=neutral, PublicKeyToken=null'. The system cannot find the file specified.",
                exception.Message);
        }

        public TagHelperDescriptor GetFirstTagHelperDescriptor()
        {
            var descriptor = new TagHelperDescriptor
            {
                TagName = "vc:first",
                TypeName = "__Generated__FirstViewComponentTagHelper",
                AssemblyName = "Microsoft.AspNetCore.Mvc.Razor.Test",
                Attributes = new List<TagHelperAttributeDescriptor>
                    {
                        new TagHelperAttributeDescriptor
                        {
                            Name = "string-value",
                            PropertyName = "StringValue",
                            TypeName = typeof(string).FullName
                        }
                    },
                RequiredAttributes = new List<TagHelperRequiredAttributeDescriptor>
                    {
                        new TagHelperRequiredAttributeDescriptor
                        {
                            Name = "string-value"
                        }
                    }
            };

            descriptor.PropertyBag.Add(ViewComponentTagHelperDescriptorConventions.ViewComponentNameKey, "First");
            return descriptor;
        }

        public TagHelperDescriptor GetSecondTagHelperDescriptor()
        {
            var descriptor = new TagHelperDescriptor
            {
                TagName = "vc:second",
                TypeName = "__Generated__SecondViewComponentTagHelper",
                AssemblyName = "Microsoft.AspNetCore.Mvc.Razor.Test",
                Attributes = new List<TagHelperAttributeDescriptor>
                    {
                        new TagHelperAttributeDescriptor
                        {
                            Name = "string-value",
                            PropertyName = "StringValue",
                            TypeName = typeof(string).FullName
                        },
                        new TagHelperAttributeDescriptor
                        {
                            Name = "bool-value",
                            PropertyName = "BoolValue",
                            TypeName = typeof(string).FullName
                        }
                    },
                RequiredAttributes = new List<TagHelperRequiredAttributeDescriptor>
                    {
                        new TagHelperRequiredAttributeDescriptor
                        {
                            Name = "string-value"
                        },
                        new TagHelperRequiredAttributeDescriptor
                        {
                            Name = "bool-value"
                        }
                    }
            };

            descriptor.PropertyBag.Add(ViewComponentTagHelperDescriptorConventions.ViewComponentNameKey, "Second");
            return descriptor;
        }
    }

    public class FirstViewComponent : ViewComponent
    {
        public string Invoke(string stringValue)
        {
            return $"string: {stringValue}";
        }
    }

    public class SecondViewComponent : ViewComponent
    {
        public string Invoke(string stringValue, bool boolValue)
        {
            return $"string: {stringValue} bool: {boolValue}";
        }
    }
}
