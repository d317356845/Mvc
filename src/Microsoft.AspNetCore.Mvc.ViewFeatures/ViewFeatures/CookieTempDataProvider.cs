// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace Microsoft.AspNetCore.Mvc.ViewFeatures
{
    public class CookieTempDataProvider : ITempDataProvider
    {
        public static readonly string CookieName = ".AspNetCore.Mvc.ViewFeatures.CookieTempDataProvider";
        private static readonly string Purpose = "Microsoft.AspNetCore.Mvc.ViewFeatures.CookieTempDataProviderToken.v1";
        private const byte TokenVersion = 0x01;
        private readonly IDataProtector _dataProtector;
        private TempDataSerializer _tempDataSerializer;
        private readonly ChunkingCookieManager _chunkingCookieManager;

        public CookieTempDataProvider(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtector = dataProtectionProvider.CreateProtector(Purpose);
            _tempDataSerializer = new TempDataSerializer();
            _chunkingCookieManager = new ChunkingCookieManager();
        }

        public IDictionary<string, object> LoadTempData(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            IDictionary<string, object> values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (context.Request.Cookies.ContainsKey(CookieName))
            {
                var base64EncodedValue = _chunkingCookieManager.GetRequestCookie(context, CookieName);
                if (!string.IsNullOrEmpty(base64EncodedValue))
                {
                    var protectedData = Convert.FromBase64String(base64EncodedValue);
                    var unprotectedData = _dataProtector.Unprotect(protectedData);
                    values = _tempDataSerializer.DeserializeTempData(unprotectedData);
                }
            }

            return values;
        }

        public void SaveTempData(HttpContext context, IDictionary<string, object> values)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var cookieOptions = new CookieOptions()
            {
                Path = context.Request.PathBase,
                HttpOnly = true,
                Secure = true
            };

            var hasValues = (values != null && values.Count > 0);
            if (hasValues)
            {
                var bytes = _tempDataSerializer.SerializeTempData(values);
                bytes = _dataProtector.Protect(bytes);
                var base64EncodedValue = Convert.ToBase64String(bytes);
                _chunkingCookieManager.AppendResponseCookie(context, CookieName, base64EncodedValue, cookieOptions);
            }
            else
            {
                _chunkingCookieManager.DeleteCookie(context, CookieName, cookieOptions);
            }
        }
    }
}
