<%@ Control Language="C#" AutoEventWireup="true" CodeFile="app_ucTimePicker.ascx.cs"
    Inherits="app_ucTimePicker" %>
<asp:Label ID="lblHH" runat="server"
    Text="<span class=access_key>H</span>H"
    CssClass="app_label4"
    AccessKey="H"
    AssociatedControlID="ddlHH">
</asp:Label>
<asp:DropDownList ID="ddlHH" runat="server">
</asp:DropDownList>
<asp:Label ID="lblMM" runat="server"
    Text="<span class=access_key>M</span>M"
    CssClass="app_label4"
    AccessKey="M"
    AssociatedControlID="ddlMM">
</asp:Label>
<asp:DropDownList ID="ddlMM" runat="server">
</asp:DropDownList>
<asp:Label ID="lblSS" runat="server"
    Text="<span class=access_key>S</span>S"
    CssClass="app_label4"
    AccessKey="S"
    AssociatedControlID="ddlSS"
    Visible="false"
    Enabled="false">
</asp:Label>
<asp:DropDownList ID="ddlSS" runat="server"
    Visible="false"
    Enabled="false">
</asp:DropDownList>
