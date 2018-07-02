using System;
using System.Collections.Generic;
using Banana;

namespace GR.Entity
{
    /// <summary>
    /// 纸张背景设置
    /// 作者：张炜
    /// 时间：2018/06/14 17:16:00
    /// </summary>
    [Serializable]
    public class CmCardBackgroundSet
    {
        #region 属性

        /// <summary>
        /// 创建人
        /// </summary>
        public string Createuser { get; set; }

        /// <summary>
        /// 每页分栏
        /// </summary>
        public string Subfeild { get; set; }

        /// <summary>
        /// 分栏数量
        /// </summary>
        public string Subfieldqty { get; set; }

        /// <summary>
        /// 左右距离
        /// </summary>
        public string Leftrightdistance { get; set; }

        /// <summary>
        /// 上下距离
        /// </summary>
        public string Updowndistance { get; set; }

        /// <summary>
        /// 左边距
        /// </summary>
        public string Leftedgedistance { get; set; }

        /// <summary>
        /// 上边距
        /// </summary>
        public string Upedgedistance { get; set; }

        /// <summary>
        /// 横向竖向 1竖向 2 横向
        /// </summary>
        public string Verticalorhorizontal { get; set; }

        /// <summary>
        /// 纸张类别
        /// </summary>
        public string Papertype { get; set; }

        /// <summary>
        /// 纸张高
        /// </summary>
        public string Paperheigh { get; set; }

        /// <summary>
        /// 纸张宽
        /// </summary>
        public string Paperweith { get; set; }

        /// <summary>
        /// 1，非自定义 2，自定义
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime Createdate { get; set; }

        #endregion
    }
}
