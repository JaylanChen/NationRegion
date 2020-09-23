using System;

namespace NationRegion.Data
{
    /// <summary>
    /// 区域基类
    /// </summary>
    public class Region
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 父级编号
        /// </summary>
        public string ParentCode { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 区域名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// 是否以获取子区域
        /// </summary>
        public bool IsGetChild { get; set; }

        /// <summary>
        /// 子级地区URL
        /// </summary>
        public string ChildNodeUrl { get; set; }

        /// <summary>
        /// 状态
        /// 1 正常可用
        /// 2 修改名称，直辖市 的二级区域从 市辖区 改为 父级名称
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDel { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
