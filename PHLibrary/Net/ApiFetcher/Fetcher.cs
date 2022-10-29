using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PHLibrary.Net.ApiFetcher
{
    public interface IFetcher<T>
    {
        /// <summary>
        /// fetch with paging
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="startPageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IList<T> Get(string url);
        IList<T> Post(string url, string postbody);

        IList<T> PagedGet(string urlWithOutPage, IPagingRequest pagingParam, Func<bool> pageEndpoint);
        IList<T> PagedPost(string url, IPagingRequest pagingParam, Func<bool> pageEndpoint);
    }
    public interface IPagingParam
    {
        int PageIndex { get; set; }
        int PageSize { get; set; }
    }
    /// <summary>
    /// fetch and deserialize object from url
    /// support break point 
    /// support paging
    /// 
    /// useage: var fecher=new Fetcher<Student>(baseUrl,"Post",
    /// 

    /// </summary>
    public class Fetcher<T> : IFetcher<T>
    {

        string url;
        HttpMethod method;
        bool needPaging;

        HttpClient httpClient;
        public Fetcher(IHttpClientFactory httpClientFactory)
        {

        }


        public IList<T> Get(string url)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<T>> PagedGet(string urlWithOutPage, IPagingRequest pagingParam, Func<bool> pageEndpoint)
        {

            string url = urlWithOutPage.Any(x => x.Equals("?"))
                ? $"{urlWithOutPage}&{pagingParam.BuildPagingQuery()}"
                : $"{urlWithOutPage}?{pagingParam.BuildPagingQuery()}";
            var response=await httpClient.GetAsync(url);
            Newtonsoft.Json.JsonConvert.DeserializeObject response.Content.ReadAsStringAsync()
        }

        public IList<T> PagedPost(string url, IPagingRequest pagingParam, Func<bool> pageEndpoint)
        {
            throw new NotImplementedException();
        }

        public IList<T> Post()
        {
            throw new Exception();
        }

        public IList<T> Post(string url, string postbody)
        {
            throw new NotImplementedException();
        }

        private HttpRequestMessage CreateRequest()
        {
            var httpRequestMessage = new HttpRequestMessage(method, url);


        }



    }
    public abstract class IPagingRequest
    {
        public string PageIndexParamName { get; set; }
        public string PageSizePramName { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string BuildPagingQuery()
        {
            return $"{PageIndexParamName}={PageIndex}&{PageSizePramName}={PageSize}";
        }
    }

}
