using Microsoft.EntityFrameworkCore;
using NationRegion.Data;
using NationRegion.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NationRegion
{
  public class JsonRegionHelper
  {
    /// <summary>
    /// 生成JSON区域数据
    /// 1~3级区域
    /// </summary>
    /// <param name="level"></param>
    public static void GenerateJsonArea(int level = 3)
    {
      if (level < 1 || level > 3)
      {
        Console.WriteLine("Error Level");
        return;
      }
      List<RegionJsonModel> areaList;
      using (var dbContext = new RegionDbContext())
      {
        var areaStatus = new List<int> { 1, 2, 3 };
        var query = dbContext.Region.AsNoTracking()
            .Where(a => areaStatus.Contains(a.Status) && a.IsDel == false)
            .Select(a => new RegionJsonModel
            {
              Code = a.Code,
              Name = a.Name,
              ParentCode = a.ParentCode,
              Level = a.Level
            }).OrderBy(a => a.ParentCode).ThenBy(a => a.Code);

        areaList = query.Where(a => a.Level == 1).ToList();
        if (level > 1)
        {
          foreach (var area in areaList)
          {
            area.Children = query.Where(a => a.ParentCode == area.Code).ToList();
            if (level == 2)
            {
              continue;
            }

            foreach (var areaChild in area.Children)
            {
              areaChild.Children = query.Where(a => a.ParentCode == areaChild.Code).ToList();
            }
          }
        }
      }

      var currentPath = Environment.CurrentDirectory;
      const string projectName = "NationRegion";
      var projectIndex = currentPath.IndexOf(projectName, StringComparison.OrdinalIgnoreCase);
      var jsonPath = Path.Combine(currentPath.Substring(0, projectIndex), projectName + "\\Data\\JsonRegion.json");

      var setting = new JsonSerializerSettings
      {
        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
      };
      var jsonAreaListStr = JsonConvert.SerializeObject(areaList, setting);

      File.WriteAllText(jsonPath, jsonAreaListStr, Encoding.UTF8);
    }
  }
}
