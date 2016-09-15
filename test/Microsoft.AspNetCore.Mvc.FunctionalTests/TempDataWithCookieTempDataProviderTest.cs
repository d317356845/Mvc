// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Testing.xunit;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.FunctionalTests
{
    public class TempDataWithCookieTempDataProviderTest : IClassFixture<MvcTestFixture<BasicWebSite.StartupWithCookieTempDataProvider>>
    {
        private readonly TempDataCommon _tempDataCommon;

        public TempDataWithCookieTempDataProviderTest(MvcTestFixture<BasicWebSite.StartupWithCookieTempDataProvider> fixture)
        {
            Client = fixture.Client;
            _tempDataCommon = new TempDataCommon(Client);
        }

        public HttpClient Client { get; }

        [Fact]
        public Task PersistsJustForNextRequest()
        {
            return _tempDataCommon.PersistsJustForNextRequest();
        }

        [Fact]
        public Task ViewRendersTempData()
        {
            return _tempDataCommon.ViewRendersTempData();
        }

        [ConditionalFact]
        // Mono issue - https://github.com/aspnet/External/issues/21
        [FrameworkSkipCondition(RuntimeFrameworks.Mono)]
        public Task Redirect_RetainsTempData_EvenIfAccessed()
        {
            return _tempDataCommon.Redirect_RetainsTempData_EvenIfAccessed();
        }

        [Fact]
        public Task Peek_RetainsTempData()
        {
            return _tempDataCommon.Peek_RetainsTempData();
        }

        [ConditionalFact]
        // Mono issue - https://github.com/aspnet/External/issues/21
        [FrameworkSkipCondition(RuntimeFrameworks.Mono)]
        public Task ValidTypes_RoundTripProperly()
        {
            return _tempDataCommon.ValidTypes_RoundTripProperly();
        }

        [Fact]
        public Task SetInActionResultExecution_AvailableForNextRequest()
        {
            return _tempDataCommon.SetInActionResultExecution_AvailableForNextRequest();
        }

        [Theory]
        [InlineData(ChunkingCookieManager.DefaultChunkSize)]
        [InlineData(ChunkingCookieManager.DefaultChunkSize * 1.5)]
        [InlineData(ChunkingCookieManager.DefaultChunkSize * 2)]
        [InlineData(ChunkingCookieManager.DefaultChunkSize * 3)]
        public async Task RoundTripLargeData_WorksWithChunkingCookies(int size)
        {
            // Arrange
            var character = 'a';
            var expected = new string(character, size);

            // Act 1
            var response = await Client.GetAsync($"/TempData/SetLargeValueInTempData?size={size}&character={character}");

            // Assert 1
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Act 2
            response = await Client.SendAsync(_tempDataCommon.GetRequest("TempData/GetLargeValueFromTempData", response));

            // Assert 2
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadAsStringAsync();
            Assert.Equal(expected, body);

            // Act 3
            response = await Client.SendAsync(_tempDataCommon.GetRequest("TempData/GetLargeValueFromTempData", response));

            // Assert 3
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}