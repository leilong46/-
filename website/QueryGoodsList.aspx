<%@ Page Language="C#" AutoEventWireup="true" CodeFile="QueryGoodsList.aspx.cs" Inherits="QueryGoodsList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <br />
            AppID<asp:TextBox ID="TextBoxAppID" runat="server" Width="535px">eacc4709520d03d2e4</asp:TextBox>
            <br />
            client_secret<asp:TextBox ID="TextBoxSecret" runat="server" Width="439px">a57203cca512fb1c4f0ba8c8e9b1ebda</asp:TextBox>
            <br />
            店铺ID<asp:TextBox ID="TextBoxMaketID" runat="server">40988775</asp:TextBox>
            <br />
            会员卡<asp:TextBox ID="TextBox会员卡" runat="server" Width="335px"></asp:TextBox>
            <br />
            <asp:Button ID="ButtonXBQ" runat="server" OnClick="ButtonXBQ_Click" Text="西比奇" />
            <br />
            <br />
            <asp:Button ID="ButtonQuery" runat="server" OnClick="ButtonQuery_Click" Text="检索商品" />
            <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="获取所有会员卡" />
            <asp:TextBox ID="TextBoxProductID" runat="server"></asp:TextBox>
            <br />
            <br />
        </div>
        <div>
            <asp:GridView ID="GridViewMemberCard" runat="server" AutoGenerateColumns="False" EnableViewState="False" Width="100%">
                <Columns>
                    <asp:BoundField DataField="id" HeaderText="id" SortExpression="id" />
                    <asp:BoundField DataField="名称" HeaderText="名称" SortExpression="名称" />
                </Columns>
            </asp:GridView>
            <br />
            <br />
            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" Width="100%" EnableViewState="False">
                <Columns>
                    <asp:BoundField DataField="门店id" HeaderText="门店id" SortExpression="门店id" />
                    <asp:BoundField DataField="id" HeaderText="id" SortExpression="id" />
                    <asp:BoundField DataField="商品条码" HeaderText="商品条码" SortExpression="商品条码" />
                    <asp:BoundField DataField="名称" HeaderText="名称" SortExpression="名称" />
                    <asp:BoundField DataField="单位" HeaderText="单位" SortExpression="单位" />
                    <asp:BoundField DataField="零售价" HeaderText="零售价" SortExpression="零售价" />
                    <asp:BoundField DataField="会员价" HeaderText="会员价" SortExpression="会员价" />
                     <asp:BoundField DataField="会员卡名称" HeaderText="会员卡名称" SortExpression="会员卡名称" /> 
                    <asp:BoundField DataField="会员价2" HeaderText="会员价2" SortExpression="会员价2" />
                     <asp:BoundField DataField="会员卡名称2" HeaderText="会员卡名称2" SortExpression="会员卡名称2" /> 
                    <asp:BoundField DataField="会员价3" HeaderText="会员价3" SortExpression="会员价3" />
                     <asp:BoundField DataField="会员卡名称3" HeaderText="会员卡名称3" SortExpression="会员卡名称3" />
                    <asp:BoundField DataField="会员价4" HeaderText="会员价4" SortExpression="会员价4" />
                     <asp:BoundField DataField="会员卡名称4" HeaderText="会员卡名称4" SortExpression="会员卡名称4" />
                    <asp:BoundField DataField="产地" HeaderText="产地" SortExpression="产地" />
                    <asp:BoundField DataField="规格" HeaderText="规格" SortExpression="规格" />
                    <asp:BoundField DataField="等级" HeaderText="等级" SortExpression="等级" />
                </Columns>
            </asp:GridView>
            <asp:TextBox ID="TextBox1" runat="server" Height="479px" TextMode="MultiLine" Width="831px"></asp:TextBox>
            <br />
        </div>
    </form>
</body>
</html>
