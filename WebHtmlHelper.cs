using SufeiUtil;
using System;
using System.Net;
using System.Threading;

namespace NationRegion
{
    public static class WebHtmlHelper
    {
        private static readonly HttpItem _httpItem = new HttpItem
        {
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.102 Safari/537.36 Edg/85.0.564.51",
            Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8"
        };
        private static readonly HttpHelper _httpHelper = new HttpHelper();

        public static string GetHtmlContent(string url)
        {
            _httpItem.URL = url;
            var result = _httpHelper.GetHtml(_httpItem);

            var times = 5;
            while (result.StatusCode != HttpStatusCode.OK)
            {
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm ") + result.StatusCode + "/" + result.StatusDescription + " 获取过于频繁，休息 " + times + "秒后重新开始获取");
                Thread.Sleep(times * 1000);
                result = _httpHelper.GetHtml(_httpItem);
                times++;
            }

            return result.Html;
        }
    }
}
