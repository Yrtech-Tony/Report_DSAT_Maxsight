using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XHX.DTO
{
    public class LossResultDto
    {
        public string LossCode{get;set;}
        public string LossName{get;set;}
        public string LossName1 { get; set; }
        public string LossName2 { get; set; }
        public string LossName3 { get; set; }
        public string InUserID{ get; set; }
        public DateTime InDateTime { get; set; }
        public char StatusType { get; set; }
        public string ProjectCode { get; set; }
        public string SubjectCode { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string PicName { get; set; }
        public int? SeqNO { get; set; }
        public string LossType { get; set; }
    }
}
