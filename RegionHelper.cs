using HtmlAgilityPack;
using NationRegion.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NationRegion
{
  //详细地区
  public static class RegionHelper
  {
    /// <summary>
    /// 获取行政区域
    /// </summary>
    /// <param name="regionBaseUrl"></param>
    public static void GetAllRegions(string regionBaseUrl)
    {
      GetProvince(regionBaseUrl);
    }

    /// <summary>
    /// 省市
    /// </summary>
    private static void GetProvince(string regionBaseUrl)
    {
      Console.WriteLine("省级获取开始");
      //HtmlWeb htmlWeb = new HtmlWeb();
      //htmlWeb.OverrideEncoding = Encoding.GetEncoding("GB2312");
      //var doc = htmlWeb.Load(regionBaseUrl);

      var htmlContent = WebHtmlHelper.GetHtmlContent(regionBaseUrl);
      var htmlDoc = new HtmlDocument();
      htmlDoc.LoadHtml(htmlContent);

      var provinceTrs = htmlDoc.DocumentNode.SelectNodes("//tr[@class='provincetr']");
      var regionList = new List<Region>();
      foreach (var provinceTr in provinceTrs)
      {
        var provinceTdNodes = provinceTr.SelectNodes("td");
        foreach (var provinceTd in provinceTdNodes)
        {
          var aNodes = provinceTd.SelectNodes("a");
          if (aNodes == null || aNodes.Count == 0)
          {
            continue;
          }
          var provinceNode = aNodes.First();
          var href = provinceNode.Attributes["href"].Value;
          var code = GetCodeByHref(href);
          if (string.IsNullOrEmpty(code))
          {
            continue;
          }
          var name = provinceNode.InnerText;

          var region = new Region
          {
            Name = name.Trim(),
            Code = code,
            ParentCode = string.Empty,
            Level = 1,
            ChildNodeUrl = GetHrefFullUrl(regionBaseUrl, href),
            Status = 1 // 使用
          };
          regionList.Add(region);
        }
      }
      //710000     台湾省
      //810000     香港特别行政区
      //820000     澳门特别行政区
      regionList.Add(new Region
      {
        Name = "台湾省",
        Code = "71",
        ParentCode = string.Empty,
        Level = 1,
        Status = 1 // 使用
      });
      regionList.Add(new Region
      {
        Name = "香港特别行政区",
        Code = "81",
        ParentCode = string.Empty,
        Level = 1,
        Status = 1 // 使用
      });
      regionList.Add(new Region
      {
        Name = "澳门特别行政区",
        Code = "82",
        ParentCode = string.Empty,
        Level = 1,
        Status = 1 // 使用
      });
      var i = 0;
      using (var dbContext = new RegionDbContext())
      {
        regionList = regionList.OrderBy(r => r.Code).ToList();
        foreach (var region in regionList)
        {
          if (dbContext.Region.Any(a => a.Code == region.Code))
          {
            Console.WriteLine($"Code:{region.Code},Name:{region.Name} 已存在");
            continue;
          }
          dbContext.Region.Add(region);
          i++;
        }
        dbContext.SaveChanges();
      }
      Console.WriteLine($"省级获取完毕:共 {regionList.Count}条,添加 {i}条");
      GetCitys(regionList);
    }


    /// <summary>
    /// 市
    /// </summary>
    /// <param name="parentregionList"></param>
    private static void GetCitys(List<Region> parentregionList)
    {
      if (!parentregionList.Any())
      {
        return;
      }
      Console.WriteLine("市级获取开始：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
      var currentregionList = GetRegions(parentregionList, "citytr");
      Console.WriteLine("市级获取结束：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
      GetCountys(currentregionList);
    }

    /// <summary>
    /// 区县
    /// </summary>
    /// <param name="parentregionList"></param>
    private static void GetCountys(List<Region> parentregionList)
    {
      if (!parentregionList.Any())
      {
        return;
      }
      Console.WriteLine("区县级获取开始：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
      var currentregionList = GetRegions(parentregionList, "countytr");

      Console.WriteLine("区县级获取结束：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

      GetTowns(currentregionList);
    }

    /// <summary>
    /// 乡镇/街道
    /// </summary>
    /// <param name="parentregionList"></param>
    private static void GetTowns(List<Region> parentregionList)
    {
      if (!parentregionList.Any())
      {
        return;
      }
      Console.WriteLine("乡镇/街道级获取开始：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
      var currentregionList = GetRegions(parentregionList, "towntr");
      Console.WriteLine("乡镇/街道级获取结束：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
    }


    /// <summary>
    /// 获取子级区域
    /// </summary>
    /// <param name="parentregionList"></param>
    /// <param name="className"></param>
    /// <returns></returns>
    private static List<Region> GetRegions(List<Region> parentregionList, string className)
    {
      if (!parentregionList.Any())
      {
        return new List<Region>();
      }
      var random = new Random();
      var regionList = new List<Region>();
      var addedCount = 0;
      foreach (var parentRegion in parentregionList)
      {
        if (string.IsNullOrEmpty(parentRegion.ChildNodeUrl))
        {
          continue;
        }

        var htmlContent = WebHtmlHelper.GetHtmlContent(parentRegion.ChildNodeUrl);
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(htmlContent);

        var regionTrs = htmlDoc.DocumentNode.SelectNodes("//tr[@class='" + className + "']");
        var tempregionList = new List<Region>();
        if (regionTrs == null)
        {
          var currentRegion = new Region
          {
            ParentCode = parentRegion.Code,
            Level = parentRegion.Level + 1,
            Name = parentRegion.Name,
            Code = parentRegion.Code + "00",
            ChildNodeUrl = parentRegion.ChildNodeUrl,
            Status = 3 //没有该级区域，手动维护的
          };
          if (tempregionList.Any(a => a.Code == currentRegion.Code))
          {
            continue;
          }
          tempregionList.Add(currentRegion);

        }
        else
        {
          foreach (var regionTr in regionTrs)
          {
            var regionTds = regionTr.SelectNodes("td");
            var regions = regionTds[1].SelectNodes("a");
            var currentRegion = new Region
            {
              ParentCode = parentRegion.Code,
              Level = parentRegion.Level + 1,
              Status = 1
            };
            var name = regionTds[1].InnerText.Trim();
            if (regions == null)
            {
              var aCode = regionTds[0].InnerText.TrimEnd('0');
              if (aCode.Length % 2 != 0)
              {
                aCode += "0";
              }
              currentRegion.Name = name;
              currentRegion.Code = aCode;
              currentRegion.Status = 0;//不使用
              tempregionList.Add(currentRegion);
              continue;
            }

            var region = regions[0];
            name = region.InnerText.Trim();
            if (name == "市辖区")
            {
              name = parentRegion.Name;
              currentRegion.Status = 2;//改名了，直辖市 的二级区域 从市辖区 改为父级名称
            }
            var href = region.Attributes["href"].Value;
            var code = GetCodeByHref(href);

            currentRegion.Name = name;
            currentRegion.Code = code;
            currentRegion.ChildNodeUrl = GetHrefFullUrl(parentRegion.ChildNodeUrl, href);
            if (tempregionList.Any(a => a.Code == currentRegion.Code))
            {
              continue;
            }
            tempregionList.Add(currentRegion);
          }
        }

        using (var dbContext = new RegionDbContext())
        {
          tempregionList = tempregionList.OrderBy(r => r.Code).ToList();
          foreach (var region in tempregionList)
          {
            if (dbContext.Region.Any(a => a.Code == region.Code))
            {
              Console.WriteLine($"Code:{region.Code},Name:{region.Name} 已存在");
              continue;
            }
            dbContext.Region.Add(region);
            addedCount++;
          }

          parentRegion.IsGetChild = true;
          dbContext.Region.Update(parentRegion);
          dbContext.SaveChanges();
        }
        regionList.AddRange(tempregionList);
        var sleepTime = random.Next(500, 800) + parentRegion.Level * 60;
        Thread.Sleep(sleepTime);
      }
      Console.WriteLine($"共 {regionList.Count}条,添加 {addedCount}条");
      return regionList;
    }


    /// <summary>
    /// 获取完整URL路径
    /// </summary>
    /// <param name="currentUrl"></param>
    /// <param name="hrefUrl"></param>
    /// <returns></returns>
    private static string GetHrefFullUrl(string currentUrl, string hrefUrl)
    {
      var last = currentUrl.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
      var baseUrl = currentUrl.Substring(0, last + 1);
      return baseUrl + hrefUrl;
    }

    /// <summary>
    /// 根据URL获取区域编码
    /// </summary>
    /// <param name="hrefUrl"></param>
    /// <returns></returns>
    private static string GetCodeByHref(string hrefUrl)
    {
      if (string.IsNullOrEmpty(hrefUrl))
      {
        return null;
      }
      var slashIndex = hrefUrl.LastIndexOf("/", StringComparison.OrdinalIgnoreCase);
      if (slashIndex > 0)
      {
        hrefUrl = hrefUrl.Substring(slashIndex + 1);
      }
      var dotIndex = hrefUrl.IndexOf(".", StringComparison.OrdinalIgnoreCase);
      if (dotIndex < 0)
      {
        return hrefUrl;
      }
      var code = hrefUrl.Substring(0, dotIndex);
      return code;
    }

  }
}
