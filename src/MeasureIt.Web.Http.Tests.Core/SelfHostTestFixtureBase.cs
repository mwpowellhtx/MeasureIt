using System;
using System.Net.Http;

namespace MeasureIt.Web.Http
{
    using Microsoft.Owin.Hosting;
    using Xunit;

    public abstract class SelfHostTestFixtureBase<TStartup> : DisposableTestFixtureBase
        where TStartup : Startup
    {
        protected static string GetUrl()
        {
            // TODO: TBD: completely arbitrary port number(s)...
            var port = new Random().Next(9000, 10000 - 1);
            return string.Format("http://localhost:{0}/", port);
        }

        private readonly IDisposable _webApp;

        private readonly string _url;

        private string _baseApiUrl;

        /// <summary>
        /// Gets the BaseApiUrl.
        /// </summary>
        protected string BaseApiUrl
        {
            get { return _baseApiUrl; }
            private set
            {
                var s = value;
                Assert.NotNull(s);
                Assert.NotEqual(string.Empty, s);
                Assert.StartsWith("api/", s);
                _baseApiUrl = s;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="url">The self hosted web server Url.</param>
        /// <param name="baseApiUrl">The base API address.</param>
        protected SelfHostTestFixtureBase(string url, string baseApiUrl)
        {
            BaseApiUrl = baseApiUrl;
            _url = url;
            // TODO: TBD: may need/want the Options ctor...
            _webApp = WebApp.Start<TStartup>(url);
            Assert.NotNull(_webApp);
        }

        public delegate void ResponseHandler(HttpResponseMessage response);

        protected interface IHttpResponseHandler
        {
            void Handle(ResponseHandler handler);
        }

        protected class HttpResponseHandler : IHttpResponseHandler
        {
            private readonly HttpResponseMessage _response;

            internal HttpResponseHandler(HttpResponseMessage response)
            {
                _response = response;
            }

            public void Handle(ResponseHandler handler)
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
                // TODO: TBD: don't know how to get at the request, per se...
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
