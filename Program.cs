using System;

namespace NationRegion
{
  class Program
  {
    static void Main(string[] args)
    {
      // for Encoding GB2312
      System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

      //http://www.mca.gov.cn/article/sj/xzqh/2018/201804-12/20180910291042.html
      //const string regionUrl = "http://www.stats.gov.cn/tjsj/tjbz/xzqhdm/201703/t20170310_1471429.html";
      //const string regionBaseUrl = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2016/index.html";

      const string regionBaseUrl = "http://www.stats.gov.cn/tjsj/tjbz/tjyqhdmhcxhfdm/2019/index.html";
      RegionHelper.GetAllRegions(regionBaseUrl);

      // 生成JSON数据
      // JsonRegionHelper.GenerateJsonArea();
      Console.WriteLine("All Done");
      Console.ReadLine();
    }
  }
}
