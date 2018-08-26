using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YZOpenSDK;

public partial class QueryGoodsList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }
   protected void ButtonQuery_Click(object sender, EventArgs e)
    {
        if (!GlobalInfo.IsTokenValidate(TextBoxAppID.Text))
        {
            GlobalInfo.GetToken(TextBoxAppID.Text, TextBoxSecret.Text, TextBoxMaketID.Text);
        }
        if (GlobalInfo.IsTokenValidate(TextBoxAppID.Text))
        {
            var QueryResult = GlobalInfo.GetProdustsInfo();
            GridView1.DataSource = QueryResult;
            GridView1.DataBind();
        }
    }

  


    protected void Button1_Click(object sender, EventArgs e)
    {
        if (!GlobalInfo.IsTokenValidate(TextBoxAppID.Text))
        {
            GlobalInfo.GetToken(TextBoxAppID.Text,TextBoxSecret.Text,TextBoxMaketID.Text);
        }
        if (GlobalInfo.IsTokenValidate(TextBoxAppID.Text))
        {
            var resultList = GlobalInfo.GetDiscountCardsInfo();
            GridViewMemberCard.DataSource = resultList;
            GridViewMemberCard.DataBind();
        }           
    }

 
    protected void ButtonXBQ_Click(object sender, EventArgs e)
    {
        TextBoxAppID.Text = "a2b31c521fda70634a";
        TextBoxSecret.Text = "d2f2111b804f20f51b88baf942a4dbbb";
        TextBoxMaketID.Text = "40892323";
        TextBox会员卡.Text = "100501980";
    }
}