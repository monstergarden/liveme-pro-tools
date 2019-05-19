using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;

namespace LMPT.Core.Services.Http
{
    internal interface IRequestBuilder
    {
        HttpRequestMessage Build();
        IRequestBuilder WithQueryParameter(params (string, string)[] parameter);
    }

    internal interface IPostRequestBuilder : IRequestBuilder
    {
        HttpRequestBuilder WithFormData(object data);
    }


    internal class HttpRequestBuilder : IPostRequestBuilder
    {
        private readonly HttpMethod _method;
        private HttpContent _content;
        private Dictionary<string, string> _contentHeaders;
        private readonly string _inputUrl;
        private string _url;

        private HttpRequestBuilder(HttpMethod method, string url)
        {
            _method = method;
            _url = url;
            _inputUrl = url;
        }

        public IRequestBuilder WithQueryParameter(params (string, string)[] parameter)
        {
            var builder = new UriBuilder(_inputUrl);
            var query = HttpUtility.ParseQueryString(builder.Query);
            foreach (var (key, value) in parameter) query[key] = value;
            builder.Query = query.ToString();
            _url = builder.ToString();
            return this;
        }


        public HttpRequestBuilder WithFormData(object data)
        {
            var formData = new MultipartFormDataContent();

            foreach (var info in data.GetType().GetProperties())
            {
                var name = info.Name;
                var val = info.GetValue(data);
                formData.Add(new StringContent(val.ToString()), name);
            }

            _content = formData;
            return this;
        }

        public HttpRequestMessage Build()
        {
            var msg = new HttpRequestMessage(_method, _url);


            if (_content == null) return msg;

            if (_contentHeaders != null)
            {
                _content.Headers.Clear();
                foreach (var pair in _contentHeaders) _content.Headers.Add(pair.Key, pair.Value);
            }

            msg.Content = _content;
            return msg;
        }


        public static IRequestBuilder Get(string url)
        {
            return new HttpRequestBuilder(HttpMethod.Get, url);
        }

        public static HttpRequestBuilder Post(string url)
        {
            return new HttpRequestBuilder(HttpMethod.Post, url);
        }

        public HttpRequestBuilder WithBody(string data)
        {
            _content = new StringContent(data);
            return this;
        }

        public HttpRequestBuilder ReplaceContentHeaders(Dictionary<string, string> headers)
        {
            _contentHeaders = headers;
            return this;
        }
    }
}