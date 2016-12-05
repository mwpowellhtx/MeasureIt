using System;
using System.Net.Http;

namespace MeasureIt
{
    using Microsoft.Owin.Hosting;
    using Xunit;

    public abstract class SelfHostTestFixtureBase<TStartup> : DisposableTestFixtureBase
        where TStartup : Startup
    {
        private readonly IDisposable _webApp;

        private readonly string _url;

        protected SelfHostTestFixtureBase(string url)
        {
            _url = url;
            // TODO: TBD: may need/want the Options ctor...
            _webApp = WebApp.Start<TStartup>(url);
            Assert.NotNull(_webApp);
        }

        protected interface IHttpResponseHandler
        {
            void Handle(Action<HttpResponseMessage> handler);
        }

        protected class HttpResponseHandler : IHttpResponseHandler
        {
            private readonly HttpResponseMessage _response;

            internal HttpResponseHandler(HttpResponseMessage response)
            {
                _response = response;
            }

            public void Handle(Action<HttpResponseMessage> handler)
            {
                Assert.NotNull(handler);
                handler(_response);
            }
        }

        protected IHttpResponseHandler MakeRequest(Func<HttpClient, HttpResponseMessage> action)
        {
            Assert.NotNull(action);

            using (var client = new HttpClient {BaseAddress = new Uri(_url)})
            {
                return new HttpResponseHandler(action(client));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!IsDisposed && disposing)
            {
                _webApp.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
