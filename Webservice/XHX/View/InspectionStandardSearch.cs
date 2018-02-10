using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XHX.Common;
using XHX.DTO;

namespace XHX.View
{
    public partial class InspectionStandardSearch : BaseForm
    {
        localhost.Service service = new localhost.Service();
        XtraGridDataHandler<AnswerDtlDto> dataHandler = null;

        public InspectionStandardSearch()
        {
            InitializeComponent();

            //初始化Grid样式
            CommonHandler.SetRowNumberIndicator(grvReCheck);
            InitData();

            dataHandler = new XtraGridDataHandler<AnswerDtlDto>(grvReCheck);
        }

        #region Private Method

        private void InitData()
        {
            //初始化查询条件
            BindComBox.BindProject(cboProject);
            btnShopCode.Text = String.Empty;
            txtShopName.Text = String.Empty;
            grcReCheck.DataSource = new List<AnswerDtlDto>();
        }

        private void SearchReCheck()
        {
            List<AnswerDtlDto> reCheckList = new List<AnswerDtlDto>();

            DataSet ds = service.SearchInspectionStandardResult(CommonHandler.GetComboBoxSelectedValue(cboProject).ToString(),
                                                    btnShopCode.Text.Trim());
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AnswerDtlDto recheckDto = new AnswerDtlDto();
                    recheckDto.SubjectCode = Convert.ToString(ds.Tables[0].Rows[i]["SubjectCode"]);
                    recheckDto.ShopCode = Convert.ToString(ds.Tables[0].Rows[i]["ShopCode"]);
                    recheckDto.ShopName = Convert.ToString(ds.Tables[0].Rows[i]["ShopName"]);
                    //recheckDto.SeqNO = Convert.ToString(ds.Tables[0].Rows[i]["SeqNO"]);
                    recheckDto.InspectionStandardName = Convert.ToString(ds.Tables[0].Rows[i]["InspectionStandardName"]);
                    //recheckDto.CheckOptionName = Convert.ToString(ds.Tables[0].Rows[i]["CheckOptionName"]);
                    reCheckList.Add(recheckDto);
                }
            }
            grcReCheck.DataSource = reCheckList;
        }

        #endregion

        public override void InitButtonClick()
        {

            InitData();
        }
        public override void SearchButtonClick()
        {
            SearchReCheck();
        }
        public override void ExcelDownButtonClick()
        {
            if(grvReCheck.DataSource!=null)
                CommonHandler.ExcelExportByExporter(grvReCheck);
        }
        public override List<ButtonType> CreateButton()
        {
            List<ButtonType> list = new List<ButtonType>();
            list.Add(ButtonType.InitButton);
            list.Add(ButtonType.SearchButton);
            list.Add(ButtonType.ExcelDownButton);
            return list;
        }

        private void btnShopCode_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            Shop_Popup pop = new Shop_Popup("", "", false);
            pop.ShowDialog();
            ShopDto dto = pop.Shopdto;
            if (dto != null)
            {
                btnShopCode.Text = dto.ShopCode;
                txtShopName.Text = dto.ShopName;
            }
            else {
                btnShopCode.Text = "";
                txtShopName.Text = "";
            }
        }
    }
}
