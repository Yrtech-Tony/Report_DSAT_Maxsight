using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XHX.Common;
using XHX.DTO;
using DevExpress.XtraEditors.Repository;
using Microsoft.Office.Interop.Excel;
//using XHX.WebService;

namespace XHX.View
{
    public partial class Shop : BaseForm
    {
        XtraGridDataHandler<ShopDto> dataHandler = null;
        localhost.Service webService = new localhost.Service();
        List<AreaDto> _allAreaDtoList = new List<AreaDto>();
        List<AreaDto> _saleBigAreaDtoList = new List<AreaDto>();
        List<AreaDto> _saleSmallAreaDtoList = new List<AreaDto>();
        List<AreaDto> _afterBigAreaDtoList = new List<AreaDto>();
        List<AreaDto> _afterSmallAreaDtoList = new List<AreaDto>();
        MSExcelUtil msExcelUtil = new MSExcelUtil();
        public Shop()
        {
            InitializeComponent();
            OnLoadView();
        }

        public void OnLoadView()
        {
            SearchAllArea();
            //绑定查询条件中Cbo
            CommonHandler.SetComboBoxEditItems(cboSaleBigArea, _saleBigAreaDtoList, "AreaName", "AreaCode");//销售大区
            CommonHandler.SetComboBoxEditItems(cboAfterBigArea, _afterBigAreaDtoList, "AreaName", "AreaCode");//售后大区
            //绑定Grid中Cbo
            CommonHandler.BindComboBoxItems<AreaDto>(cboSaleBigAreaInGrid, _saleBigAreaDtoList, "AreaName", "AreaCode");//销售大区
            CommonHandler.BindComboBoxItems<AreaDto>(cboAfterBigAreaInGrid, _afterBigAreaDtoList, "AreaName", "AreaCode");//售后大区

            dataHandler = new XtraGridDataHandler<ShopDto>(grvShop);
        }
        public void InitializeView()
        {
            txtShopName.Text = "";
            grcShop.DataSource = null;
            SearchAllArea();
        }

        public override List<ButtonType> CreateButton()
        {
            List<ButtonType> list = new List<ButtonType>();
            list.Add(ButtonType.InitButton);
            list.Add(ButtonType.SearchButton);
            list.Add(ButtonType.AddRowButton);

            list.Add(ButtonType.SaveButton);

            return list;
        }
        public override void InitButtonClick()
        {
            base.InitButtonClick();
            InitializeView();
        }
        public override void SearchButtonClick()
        {
            SearchShop();
            if (base.UserInfoDto.RoleType != "C")
            {
                this.CSParentForm.EnabelButton(ButtonType.AddRowButton, true);
                this.CSParentForm.EnabelButton(ButtonType.SaveButton, true);
            }
            else
            {
                this.CSParentForm.EnabelButton(ButtonType.AddRowButton, false);
                this.CSParentForm.EnabelButton(ButtonType.SaveButton, false);
            }
        }
        public override void AddRowButtonClick()
        {
            //if (CommonHandler.GetComboBoxSelectedValue(cboSaleBigArea) == null || string.IsNullOrEmpty(CommonHandler.GetComboBoxSelectedValue(cboSaleBigArea).ToString()))
            //{
            //    CommonHandler.ShowMessage(MessageType.Information, "请先选择一个大区");
            //    return;
            //}
            ShopDto shop = new ShopDto();
            shop.SaleBig = "";
            shop.AfterBig = "";
            shop.UseChk = true;
            dataHandler.AddRow(shop);
        }
        public override void SaveButtonClick()
        {
            grvShop.CloseEditor();
            grvShop.UpdateCurrentRow();

            if (base.UserInfoDto.RoleType != "S")
            {
                CommonHandler.ShowMessage(MessageType.Information, "没有权限");
            }
            foreach (ShopDto shop in grcShop.DataSource as List<ShopDto>)
            {
                if (string.IsNullOrEmpty(shop.ShopCode))
                {
                    CommonHandler.ShowMessage(MessageType.Information, "经销商代码不能为空");
                    grvShop.FocusedColumn = gcShopCode;
                    grvShop.FocusedRowHandle = (grcShop.DataSource as List<ShopDto>).IndexOf(shop);
                    return;
                }
                //if (string.IsNullOrEmpty(shop.SaleSmall) && (shop.StatusType == 'I' || shop.StatusType == 'U'))
                //{
                //    CommonHandler.ShowMessage(MessageType.Information, "经销商所属销售小区不能为空");
                //    grvShop.FocusedColumn = gcSaleSmallArea;
                //    grvShop.FocusedRowHandle = (grcShop.DataSource as List<ShopDto>).IndexOf(shop);
                //    return;
                //}
                //if (string.IsNullOrEmpty(shop.AfterSmall) && (shop.StatusType == 'I' || shop.StatusType == 'U'))
                //{
                //    CommonHandler.ShowMessage(MessageType.Information, "经销商所属售后小区不能为空");
                //    grvShop.FocusedColumn = gcAfterSmall;
                //    grvShop.FocusedRowHandle = (grcShop.DataSource as List<ShopDto>).IndexOf(shop);
                //    return;
                //}

                foreach (ShopDto s in dataHandler.DataList)
                {
                    if (s != shop)
                    {
                        if (s.ShopCode == shop.ShopCode)
                        {
                            CommonHandler.ShowMessage(MessageType.Information, "经销商代码不能重复");
                            grvShop.FocusedColumn = gcShopCode;
                            grvShop.FocusedRowHandle = (grcShop.DataSource as List<ShopDto>).IndexOf(s);
                            return;
                        }
                    }
                }
            }
            if (CommonHandler.ShowMessage(MessageType.Confirm, "确定要保存吗？") == DialogResult.Yes)
            {
                List<ShopDto> shopList = dataHandler.DataList;
                foreach (ShopDto shop in shopList)
                {
                    if (shop.StatusType == 'I' || shop.StatusType == 'U')
                    {
                        webService.SaveShop(shop.SaleSmall, shop.AfterSmall, shop.ShopCode, shop.ShopName.Trim(), shop.UseChk, shop.UserName, shop.Province, shop.City);
                    }

                }
            }
            SearchShop();
            CommonHandler.ShowMessage(MessageType.Information, "保存完毕");
        }

        private void SearchShop()
        {
            List<ShopDto> shopList = new List<ShopDto>();
            DataSet ds = webService.SearchShop("", txtShopName.Text);
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    ShopDto shop = new ShopDto();
                    shop.ShopCode = Convert.ToString(ds.Tables[0].Rows[i]["ShopCode"]);
                    shop.ShopName = Convert.ToString(ds.Tables[0].Rows[i]["ShopName"]);
                    shop.SaleBig = Convert.ToString(ds.Tables[0].Rows[i]["SellBigAreaCode"]);
                    shop.SaleSmall = Convert.ToString(ds.Tables[0].Rows[i]["SellSmallAreaCode"]);
                    shop.AfterBig = Convert.ToString(ds.Tables[0].Rows[i]["AfterBigAreaCode"]);
                    shop.AfterSmall = Convert.ToString(ds.Tables[0].Rows[i]["AfterSmallAreaCode"]);
                    shop.Province = Convert.ToString(ds.Tables[0].Rows[i]["Province"]);
                    shop.City = Convert.ToString(ds.Tables[0].Rows[i]["City"]);
                    //shop.UseChk = Convert.ToBoolean(ds.Tables[0].Rows[i]["UseChk"]);
                    shopList.Add(shop);
                }
            }

            grcShop.DataSource = shopList;
        }
        private void SearchAllArea()
        {
            _allAreaDtoList.Clear();
            DataSet ds = webService.SearchArea("");
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    AreaDto area = new AreaDto();
                    area.AreaCode = Convert.ToString(ds.Tables[0].Rows[i]["AreaCode"]);
                    area.AreaName = Convert.ToString(ds.Tables[0].Rows[i]["AreaName"]);
                    area.UpperAreaCode = Convert.ToString(ds.Tables[0].Rows[i]["UpperAreaCode"]);
                    area.AreaTypeCode = Convert.ToString(ds.Tables[0].Rows[i]["AreaTypeCode"]);
                    _allAreaDtoList.Add(area);
                }
            }

            //设置全局变量
            _saleBigAreaDtoList = (from x in _allAreaDtoList where x.AreaTypeCode == "01" select x).ToList<AreaDto>();
            _saleSmallAreaDtoList = (from x in _allAreaDtoList where x.AreaTypeCode == "02" select x).ToList<AreaDto>();
            _afterBigAreaDtoList = (from x in _allAreaDtoList where x.AreaTypeCode == "03" select x).ToList<AreaDto>();
            _afterSmallAreaDtoList = (from x in _allAreaDtoList where x.AreaTypeCode == "04" select x).ToList<AreaDto>();

            //添加‘全部’选项
            _saleBigAreaDtoList.Insert(0, new AreaDto("", "全部", "", "01"));
            _afterBigAreaDtoList.Insert(0, new AreaDto("", "全部", "", "01"));
        }

        private void Shop_Load(object sender, EventArgs e)
        {
            this.CSParentForm.EnabelButton(ButtonType.AddRowButton, false);
            this.CSParentForm.EnabelButton(ButtonType.SaveButton, false);
        }
        private void gridView1_ShowingEditor(object sender, CancelEventArgs e)
        {
            ShopDto shop = grvShop.GetRow(grvShop.FocusedRowHandle) as ShopDto;
            if (grvShop.FocusedColumn == gcShopCode)
            {
                if (shop.StatusType != 'I')
                {
                    e.Cancel = true;

                }
                else
                {
                    e.Cancel = false;
                }
            }
        }
        private void gridView1_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            if (grcShop.DataSource == null || grvShop.RowCount == 0) return;
            ShopDto shop = grvShop.GetRow(e.RowHandle) as ShopDto;
            if (e.Column == gcShopCode && shop.StatusType != 'I')
            {
                e.Appearance.BackColor = Color.Gray;
            }
        }
        private void cboSaleBigArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.ComboBoxEdit cbo = sender as DevExpress.XtraEditors.ComboBoxEdit;
            string saleBigAreaCode = CommonHandler.GetComboBoxSelectedValue(cbo) as string;
            List<AreaDto> areaDtoList = (from x in _saleSmallAreaDtoList where x.UpperAreaCode == saleBigAreaCode select x).ToList<AreaDto>();
            areaDtoList.Insert(0, new AreaDto("", "全部", "", "03"));
            CommonHandler.SetComboBoxEditItems(cboSaleSmallArea, areaDtoList, "AreaName", "AreaCode");
        }
        private void cboAfterBigArea_SelectedIndexChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.ComboBoxEdit cbo = sender as DevExpress.XtraEditors.ComboBoxEdit;
            string afterBigAreaCode = CommonHandler.GetComboBoxSelectedValue(cbo) as string;
            List<AreaDto> areaDtoList = (from x in _afterSmallAreaDtoList where x.UpperAreaCode == afterBigAreaCode select x).ToList<AreaDto>();
            areaDtoList.Insert(0, new AreaDto("", "全部", "", "04"));
            CommonHandler.SetComboBoxEditItems(cboAfterSmallArea, areaDtoList, "AreaName", "AreaCode");
        }
        private void grvShop_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.Column == gcSaleSmallArea)
            {
                ShopDto shopDto = grvShop.GetRow(e.RowHandle) as ShopDto;
                if (shopDto != null)
                {
                    RepositoryItemComboBox cboSaleSmallAreaInGrid = new RepositoryItemComboBox();
                    CommonHandler.BindComboBoxItems<AreaDto>(cboSaleSmallAreaInGrid, (from x in _saleSmallAreaDtoList where x.UpperAreaCode == shopDto.SaleBig select x).ToList<AreaDto>(), "AreaName", "AreaCode");
                    e.RepositoryItem = cboSaleSmallAreaInGrid;
                }
            }
            else if (e.Column == gcAfterSmall)
            {
                ShopDto shopDto = grvShop.GetRow(e.RowHandle) as ShopDto;
                if (shopDto != null)
                {
                    RepositoryItemComboBox cboAfterSmallAreaInGrid = new RepositoryItemComboBox();
                    CommonHandler.BindComboBoxItems<AreaDto>(cboAfterSmallAreaInGrid, (from x in _afterSmallAreaDtoList where x.UpperAreaCode == shopDto.AfterBig select x).ToList<AreaDto>(), "AreaName", "AreaCode");
                    e.RepositoryItem = cboAfterSmallAreaInGrid;
                }
            }
            else
            {
                //continue;
            }
        }

        private void btnModule_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog ofp = new OpenFileDialog();
            ofp.Filter = "Excel(*.xlsx)|";
            ofp.FilterIndex = 2;
            if (ofp.ShowDialog() == DialogResult.OK)
            {
                btnModule.Text = ofp.FileName;
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            Workbook workbook = msExcelUtil.OpenExcelByMSExcel(btnModule.Text);
            Worksheet worksheet_FengMian = workbook.Worksheets["Shop"] as Worksheet;
            for (int i = 2; i < 300; i++)
            {
                string shopCode = msExcelUtil.GetCellValue(worksheet_FengMian, "A", i);
                if (!string.IsNullOrEmpty(shopCode))
                {
                    XHX.localhost.ShopDto shop = new XHX.localhost.ShopDto();
                    shop.ShopCode = shopCode;
                    shop.ShopName = msExcelUtil.GetCellValue(worksheet_FengMian, "B", i);
                    shop.Province = msExcelUtil.GetCellValue(worksheet_FengMian, "C", i);
                    shop.City = msExcelUtil.GetCellValue(worksheet_FengMian, "D", i);
                    shop.GroupName = msExcelUtil.GetCellValue(worksheet_FengMian, "E", i);
                    shop.ShopType = msExcelUtil.GetCellValue(worksheet_FengMian, "F", i);
                    shop.SaleSmallAreaCode = msExcelUtil.GetCellValue(worksheet_FengMian, "G", i);
                    shop.AfterSmallAreaCode = "";
                    shop.ShortName = msExcelUtil.GetCellValue(worksheet_FengMian, "H", i);
                    shop.UserId = msExcelUtil.GetCellValue(worksheet_FengMian, "I", i);
                    shop.Password = msExcelUtil.GetCellValue(worksheet_FengMian, "J", i);
                    shop.UseChk = true;
                    webService.DSATReport_ShopSave(shop, "sysadmin");
                }
            }
            CommonHandler.ShowMessage(MessageType.Information, "上传完毕");
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            Workbook workbook = msExcelUtil.OpenExcelByMSExcel(btnModule.Text);
            Worksheet worksheet_FengMian = workbook.Worksheets["Group"] as Worksheet;
            for (int i = 2; i < 60; i++)
            {
                string groupCode = msExcelUtil.GetCellValue(worksheet_FengMian, "A", i);
                if (!string.IsNullOrEmpty(groupCode))
                {
                    XHX.localhost.GroupDto group = new XHX.localhost.GroupDto();
                    group.GroupCode = groupCode.Trim().Replace(" ", "");
                    group.GroupName = msExcelUtil.GetCellValue(worksheet_FengMian, "B", i).Trim().Replace(" ", "");
                    group.UserId = msExcelUtil.GetCellValue(worksheet_FengMian, "C", i).Trim().Replace(" ", "");
                    group.Password = msExcelUtil.GetCellValue(worksheet_FengMian, "D", i).Trim().Replace(" ", "");
                    webService.DSATReport_GroupSave(group, "sysadmin");
                }
            }
            CommonHandler.ShowMessage(MessageType.Information, "上传完毕");
        }
        /// <summary>
        /// 上传人员信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton3_Click(object sender, EventArgs e)
        {
            Workbook workbook = msExcelUtil.OpenExcelByMSExcel(btnModule.Text);
            Worksheet worksheet_FengMian = workbook.Worksheets["人员信息表"] as Worksheet;
            for (int i = 2; i < 6600; i++)
            {
                string IdCardNo = msExcelUtil.GetCellValue(worksheet_FengMian, "J", i).Trim().Replace(" ", "");
                if (!string.IsNullOrEmpty(IdCardNo))
                {
                    string year = msExcelUtil.GetCellValue(worksheet_FengMian, "A", i).Trim().Replace(" ", "");
                    string month = msExcelUtil.GetCellValue(worksheet_FengMian, "B", i).Trim().Replace(" ", "");
                    string shopCode = msExcelUtil.GetCellValue(worksheet_FengMian, "G", i).Trim().Replace(" ", "");
                    string staffName = msExcelUtil.GetCellValue(worksheet_FengMian, "H", i).Trim().Replace(" ", "");
                    string sex = msExcelUtil.GetCellValue(worksheet_FengMian, "I", i).Trim().Replace(" ", "");
                    string email = msExcelUtil.GetCellValue(worksheet_FengMian, "K", i).Trim().Replace(" ", "");
                    string positionName = msExcelUtil.GetCellValue(worksheet_FengMian, "L", i).Trim().Replace(" ", "");
                    string telNumber = msExcelUtil.GetCellValue(worksheet_FengMian, "M", i).Trim().Replace(" ", "");
                    string birthday = DateTime.Now.ToString();
                    string entryDate = "";
                    if (msExcelUtil.GetCellValue(worksheet_FengMian, "N", i).Trim().Replace(" ", "").Contains("-") || msExcelUtil.GetCellValue(worksheet_FengMian, "N", i).Trim().Replace(" ", "").Contains("/")
                        || string.IsNullOrEmpty(msExcelUtil.GetCellValue(worksheet_FengMian, "N", i).Trim().Replace(" ", "")))
                    {
                        entryDate = msExcelUtil.GetCellValue(worksheet_FengMian, "N", i).Trim().Replace(" ", "");
                    }
                    else
                    {
                        entryDate = DateTime.FromOADate(Convert.ToInt32(msExcelUtil.GetCellValue(worksheet_FengMian, "N", i).Trim().Replace(" ", ""))).ToString("d");
                    }
                    string startDate = "";
                    if (msExcelUtil.GetCellValue(worksheet_FengMian, "O", i).Trim().Replace(" ", "").Contains("-") 
                        || msExcelUtil.GetCellValue(worksheet_FengMian, "O", i).Trim().Replace(" ", "").Contains("/")
                        || string.IsNullOrEmpty(msExcelUtil.GetCellValue(worksheet_FengMian, "O", i).Trim().Replace(" ", "")))
                    {
                        startDate = msExcelUtil.GetCellValue(worksheet_FengMian, "O", i).Trim().Replace(" ", "");
                    }
                    else
                    {
                        startDate = DateTime.FromOADate(Convert.ToInt32(msExcelUtil.GetCellValue(worksheet_FengMian, "O", i).Trim().Replace(" ", ""))).ToString("d");
                    }
                    string endDate = "";
                    if (msExcelUtil.GetCellValue(worksheet_FengMian, "R", i).Trim().Replace(" ", "").Contains("-") 
                        || msExcelUtil.GetCellValue(worksheet_FengMian, "R", i).Trim().Replace(" ", "").Contains("/")
                        || string.IsNullOrEmpty(msExcelUtil.GetCellValue(worksheet_FengMian, "R", i).Trim().Replace(" ", "")))
                    {
                        endDate = msExcelUtil.GetCellValue(worksheet_FengMian, "R", i).Trim().Replace(" ", "");
                    }
                    else
                    {
                        endDate = DateTime.FromOADate(Convert.ToInt32(msExcelUtil.GetCellValue(worksheet_FengMian, "R", i).Trim().Replace(" ", ""))).ToString("d");
                    }
                    //string endDate = msExcelUtil.GetCellValue(worksheet_FengMian, "R", i).Trim().Replace(" ", "");
                    string sourceName = msExcelUtil.GetCellValue(worksheet_FengMian, "P", i).Trim().Replace(" ", "");
                    string onJobTemp = msExcelUtil.GetCellValue(worksheet_FengMian, "Q", i).Trim().Replace(" ", "");
                    bool onJobChk = true;
                    if (onJobTemp == "是")
                        onJobChk = true;
                    else onJobChk = false;

                    string reasonName = msExcelUtil.GetCellValue(worksheet_FengMian, "S", i).Trim().Replace(" ", "");
                    string fateName = msExcelUtil.GetCellValue(worksheet_FengMian, "T", i).Trim().Replace(" ", "");
                    string remark = msExcelUtil.GetCellValue(worksheet_FengMian, "U", i).Trim().Replace(" ", "");
                    string salePerformance = msExcelUtil.GetCellValue(worksheet_FengMian, "X", i).Trim().Replace(" ", "");
                    string createdate = "";
                    if (msExcelUtil.GetCellValue(worksheet_FengMian, "V", i).Trim().Replace(" ", "").Contains("-")
                       || msExcelUtil.GetCellValue(worksheet_FengMian, "V", i).Trim().Replace(" ", "").Contains("/")
                       || string.IsNullOrEmpty(msExcelUtil.GetCellValue(worksheet_FengMian, "V", i).Trim().Replace(" ", "")))
                    {
                        createdate = msExcelUtil.GetCellValue(worksheet_FengMian, "V", i).Trim().Replace(" ", "");
                    }
                    else
                    {
                        createdate = DateTime.FromOADate(Convert.ToInt32(msExcelUtil.GetCellValue(worksheet_FengMian, "V", i).Trim().Replace(" ", ""))).ToString("d");
                    }
                    //string createdate = DateTime.FromOADate(Convert.ToInt32(msExcelUtil.GetCellValue(worksheet_FengMian, "V", i).Trim().Replace(" ", ""))).ToString("d"); ;

                    webService.DSATReport_StaffInfoSave(IdCardNo, year, month, shopCode, staffName, sex, birthday, email,
                        positionName, telNumber, entryDate, startDate, endDate, sourceName, onJobChk,
                        reasonName, fateName, remark, salePerformance, "sysadmin", createdate);
                }
            }
            CommonHandler.ShowMessage(MessageType.Information, "上传完毕");
        }

        private void simpleButton4_Click(object sender, EventArgs e)
        {
            Workbook workbook = msExcelUtil.OpenExcelByMSExcel(btnModule.Text);
            Worksheet worksheet_FengMian = workbook.Worksheets["ShortName"] as Worksheet;
            for (int i = 2; i < 250; i++)
            {
                string shopCode = msExcelUtil.GetCellValue(worksheet_FengMian, "A", i);
                if (!string.IsNullOrEmpty(shopCode))
                {
                    string shortName = msExcelUtil.GetCellValue(worksheet_FengMian, "B", i).Trim().Replace(" ", "");

                    webService.DSATReport_ShopSaveUpdateShortName(shopCode, shortName);
                }
            }
            CommonHandler.ShowMessage(MessageType.Information, "上传完毕");
        }

        private void simpleButton5_Click(object sender, EventArgs e)
        {
            Workbook workbook = msExcelUtil.OpenExcelByMSExcel(btnModule.Text);
            Worksheet worksheet_FengMian = workbook.Worksheets["ACScore"] as Worksheet;
            for (int i = 2; i < 900; i++)
            {
                //if (i == 670)
                //{
                //    MessageBox.Show("aa");
                //}
                string idCareNO = msExcelUtil.GetCellValue(worksheet_FengMian, "B", i).Trim().Replace(" ", "");
                if (!string.IsNullOrEmpty(idCareNO))
                {
                    string positionCode = msExcelUtil.GetCellValue(worksheet_FengMian, "C", i).Trim().Replace(" ", "");
                    DateTime date = DateTime.FromOADate(Convert.ToInt32(msExcelUtil.GetCellValue(worksheet_FengMian, "D", i).Trim().Replace(" ", "")));
                    int year = date.Year;
                    int month = date.Month;
                    string acStatusId = msExcelUtil.GetCellValue(worksheet_FengMian, "E", i).Trim().Replace(" ", "");
                    webService.DSATReport_StaffACScoreUpload(idCareNO, "", year.ToString(), month.ToString(), Convert.ToInt32(acStatusId), Convert.ToInt32(positionCode), "sysadmin");
                }
            }
            CommonHandler.ShowMessage(MessageType.Information, "上传完毕");
        }

        private void simpleButton6_Click(object sender, EventArgs e)
        {
            Workbook workbook = msExcelUtil.OpenExcelByMSExcel(btnModule.Text);
            Worksheet worksheet_FengMian = workbook.Worksheets["人员信息表 (2)"] as Worksheet;
            for (int i = 2; i < 1000; i++)
            {
                string IdCardNo = msExcelUtil.GetCellValue(worksheet_FengMian, "J", i).Trim().Replace(" ", "");
                if (!string.IsNullOrEmpty(IdCardNo))
                {
                    string year = msExcelUtil.GetCellValue(worksheet_FengMian, "A", i).Trim().Replace(" ", "");
                    string month = msExcelUtil.GetCellValue(worksheet_FengMian, "B", i).Trim().Replace(" ", "");
                    string shopCode = msExcelUtil.GetCellValue(worksheet_FengMian, "G", i).Trim().Replace(" ", "");
                    string staffName = msExcelUtil.GetCellValue(worksheet_FengMian, "H", i).Trim().Replace(" ", "");
                    //string sex = msExcelUtil.GetCellValue(worksheet_FengMian, "I", i).Trim().Replace(" ", "");
                    //string email = msExcelUtil.GetCellValue(worksheet_FengMian, "K", i).Trim().Replace(" ", "");
                    //string positionName = msExcelUtil.GetCellValue(worksheet_FengMian, "L", i).Trim().Replace(" ", "");
                    //string telNumber = msExcelUtil.GetCellValue(worksheet_FengMian, "M", i).Trim().Replace(" ", "");
                    //string birthday = DateTime.Now.ToString();
                    //string entryDate = "";
                    //if (msExcelUtil.GetCellValue(worksheet_FengMian, "N", i).Trim().Replace(" ", "").Contains("-") || msExcelUtil.GetCellValue(worksheet_FengMian, "N", i).Trim().Replace(" ", "").Contains("/")
                    //    || string.IsNullOrEmpty(msExcelUtil.GetCellValue(worksheet_FengMian, "N", i).Trim().Replace(" ", "")))
                    //{
                    //    entryDate = msExcelUtil.GetCellValue(worksheet_FengMian, "N", i).Trim().Replace(" ", "");
                    //}
                    //else
                    //{
                    //    entryDate = DateTime.FromOADate(Convert.ToInt32(msExcelUtil.GetCellValue(worksheet_FengMian, "N", i).Trim().Replace(" ", ""))).ToString("d");
                    //}
                    //string startDate = "";
                    //if (msExcelUtil.GetCellValue(worksheet_FengMian, "O", i).Trim().Replace(" ", "").Contains("-")
                    //    || msExcelUtil.GetCellValue(worksheet_FengMian, "O", i).Trim().Replace(" ", "").Contains("/")
                    //    || string.IsNullOrEmpty(msExcelUtil.GetCellValue(worksheet_FengMian, "O", i).Trim().Replace(" ", "")))
                    //{
                    //    startDate = msExcelUtil.GetCellValue(worksheet_FengMian, "O", i).Trim().Replace(" ", "");
                    //}
                    //else
                    //{
                    //    startDate = DateTime.FromOADate(Convert.ToInt32(msExcelUtil.GetCellValue(worksheet_FengMian, "O", i).Trim().Replace(" ", ""))).ToString("d");
                    //}
                    //string endDate = "";
                    //if (msExcelUtil.GetCellValue(worksheet_FengMian, "R", i).Trim().Replace(" ", "").Contains("-")
                    //    || msExcelUtil.GetCellValue(worksheet_FengMian, "R", i).Trim().Replace(" ", "").Contains("/")
                    //    || string.IsNullOrEmpty(msExcelUtil.GetCellValue(worksheet_FengMian, "R", i).Trim().Replace(" ", "")))
                    //{
                    //    endDate = msExcelUtil.GetCellValue(worksheet_FengMian, "R", i).Trim().Replace(" ", "");
                    //}
                    //else
                    //{
                    //    endDate = DateTime.FromOADate(Convert.ToInt32(msExcelUtil.GetCellValue(worksheet_FengMian, "R", i).Trim().Replace(" ", ""))).ToString("d");
                    //}
                    ////string endDate = msExcelUtil.GetCellValue(worksheet_FengMian, "R", i).Trim().Replace(" ", "");
                    //string sourceName = msExcelUtil.GetCellValue(worksheet_FengMian, "P", i).Trim().Replace(" ", "");
                    //string onJobTemp = msExcelUtil.GetCellValue(worksheet_FengMian, "Q", i).Trim().Replace(" ", "");
                    //bool onJobChk = true;
                    //if (onJobTemp == "是")
                    //    onJobChk = true;
                    //else onJobChk = false;

                    //string reasonName = msExcelUtil.GetCellValue(worksheet_FengMian, "S", i).Trim().Replace(" ", "");
                    //string fateName = msExcelUtil.GetCellValue(worksheet_FengMian, "T", i).Trim().Replace(" ", "");
                    //string remark = msExcelUtil.GetCellValue(worksheet_FengMian, "U", i).Trim().Replace(" ", "");
                    //string salePerformance = msExcelUtil.GetCellValue(worksheet_FengMian, "X", i).Trim().Replace(" ", "");
                    //string createdate = "";
                    //if (msExcelUtil.GetCellValue(worksheet_FengMian, "V", i).Trim().Replace(" ", "").Contains("-")
                    //   || msExcelUtil.GetCellValue(worksheet_FengMian, "V", i).Trim().Replace(" ", "").Contains("/")
                    //   || string.IsNullOrEmpty(msExcelUtil.GetCellValue(worksheet_FengMian, "V", i).Trim().Replace(" ", "")))
                    //{
                    //    createdate = msExcelUtil.GetCellValue(worksheet_FengMian, "V", i).Trim().Replace(" ", "");
                    //}
                    //else
                    //{
                    //    createdate = DateTime.FromOADate(Convert.ToInt32(msExcelUtil.GetCellValue(worksheet_FengMian, "V", i).Trim().Replace(" ", ""))).ToString("d");
                    //}
                    //string createdate = DateTime.FromOADate(Convert.ToInt32(msExcelUtil.GetCellValue(worksheet_FengMian, "V", i).Trim().Replace(" ", ""))).ToString("d"); ;

                    string addM = msExcelUtil.GetCellValue(worksheet_FengMian, "Y", i).Trim().Replace(" ", "");

                    webService.DSATReport_StaffInfoInDateTimeUpload(IdCardNo, year, month, shopCode,addM);
                }
            }
            CommonHandler.ShowMessage(MessageType.Information, "上传完毕");
        }
    }


}
