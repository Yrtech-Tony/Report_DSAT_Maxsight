using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XHX.DTO.SingleShopReport
{
    public class ShopReportDto
    {
        public string ProjectCode { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string AreaName { get; set; }
        public string Province { get; set; }
        public string City { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<AllScoreDto26> AllScoreDtoList26 { get; set; }
        public List<AllScoreDto31> AllScoreDtoList31 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<AllScoreSumDto> AllScoreSumDtoList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<AllShopScoreDto11> AllShopScoreDtoList11 { get; set; }
        public List<AllShopScoreDto16> AllShopScoreDtoList16 { get; set; }

        /// <summary>
        /// 服务现场审核得分率 List
        /// </summary>
        public List<ChaptersScoreDto27To30> ChaptersScoreDtoList27To30{ get; set; }
        public List<ChaptersScoreDto32To35> ChaptersScoreDtoList32To35 { get; set; }

        public List<ShopChapterScoreDto12To15> ShopChapterScoreDtoList12To15 { get; set; }

        public List<ShopChapterScoreDto17To20> ShopChapterScoreDtoList17To20 { get; set; }

        ///// <summary>
        ///// 经销商得分明细 List
        ///// </summary>
        //public List<LinkScoreDto> LinkScoreDtoList { get; set; }

        /// <summary>
        /// 指标得分详情 List
        /// </summary>
        public List<SubjectsScoreDto> SubjectsScoreDtoList { get; set; }
       
    }
}
