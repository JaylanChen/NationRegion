using System.Collections.Generic;

namespace NationRegion.Model
{
    public class RegionJsonModel
    {
        /// <summary>
        /// 编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 父级编码
        /// </summary>
        public string ParentCode { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 子区域
        /// </summary>
        public List<RegionJsonModel> Children { get; set; } = new List<RegionJsonModel>();
    }
}
