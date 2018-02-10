using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XHX.DTO.SingleShopReport
{
    public class ShopChapterScoreDto12To15
    {
        /// <summary>
        /// 权重
        /// </summary>
        public string projectCode{ get; set; }

        /// <summary>
        /// 经销商代码
        /// </summary>
        public string ShopCode { get; set; }
        /// <summary>
        /// 经销商
        /// </summary>
        public decimal ScoreShop { get; set; }
        public string CharterName { get; set; }

        /// <summary>
        /// 指标点合格数量
        /// </summary>
        public decimal SubjectPassCount { get; set; }
        /// <summary>
        /// A:销售
        /// B:服务
        /// </summary>
        public string Type { get; set; }
    }
    public class ShopChapterScoreDto17To20
    {
        /// <summary>
        /// 权重
        /// </summary>
        public string projectCode { get; set; }

        /// <summary>
        /// 经销商代码
        /// </summary>
        public string ShopCode { get; set; }
        /// <summary>
        /// 经销商
        /// </summary>
        public decimal ScoreShop { get; set; }
        public string CharterName { get; set; }

        /// <summary>
        /// 指标点合格数量
        /// </summary>
        public decimal SubjectPassCount { get; set; }
        /// <summary>
        /// A:销售
        /// B:服务
        /// </summary>
        public string Type { get; set; }
    }
}
