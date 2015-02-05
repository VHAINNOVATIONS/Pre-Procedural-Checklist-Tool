<%@ Control Language="C#" AutoEventWireup="true" CodeFile="app_ucItemLookup.ascx.cs" Inherits="app_ucItemLookup" %>

<h1 class="app_label1">
    <asp:Button ID="btnCollapseFilters" runat="server"
        OnClick="OnClickCollapseFilters"
        Text="-"
        Width="30" />
    Item Filters
</h1>
<asp:UpdatePanel ID="upSearch" runat="server"
    RenderMode="Block"
    UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="pnlFilters" runat="server"
            DefaultButton="btnSearch">
            <asp:CheckBox ID="chkFilterByName" runat="server"
                AutoPostBack="true"
                CssClass="app_label4"
                Text="Enable Item <span class=access_key>L</span>abel Filter"
                OnCheckedChanged="OnCheckedChangedName"
                AccessKey="L" />
            <div class="app_horizontal_spacer">
            </div>
            <asp:Label ID="lblItemName" runat="server"
                Text="Item L<span class=access_key>a</span>bel"
                AccessKey="A"
                AssociatedControlID="txtFilterByName">
            </asp:Label>
            <asp:TextBox ID="txtFilterByName" runat="server"
                MaxLength="255"
                Enabled="false">
            </asp:TextBox>
            <div class="app_horizontal_spacer">
            </div>
            <asp:CheckBox ID="chkFilterType" runat="server"
                OnCheckedChanged="OnCheckedChangedType"
                AutoPostBack="true"
                Text="Enable Item <span class=access_key>T</span>ype Filter"
                AccessKey="T"
                CssClass="app_label4" />
            <div class="app_horizontal_spacer">
            </div>
            <asp:Label ID="lblItemType" runat="server"
                Text="Item T<span class=access_key>y</span>pe"
                AccessKey="Y"
                AssociatedControlID="ddlFilterType">
            </asp:Label>
            <asp:DropDownList ID="ddlFilterType" runat="server"
                Enabled="false">
                </asp:DropDownList>
            <div class="app_horizontal_spacer">
            </div>
            <asp:CheckBox ID="chkFilterByGroup" runat="server"
                AutoPostBack="true"
                CssClass="app_label4"
                Text="Enable Item <span class=access_key>G</span>roup Filter"
                AccessKey="G"
                OnCheckedChanged="OnCheckedChangedGroup" />
            <div class="app_horizontal_spacer">
            </div>
            <asp:Label ID="lblItemGroup" runat="server"
                Text="Item G<span class=access_key>r</span>oup"
                AccessKey="R"
                AssociatedControlID="ddlFilterByGroup">
            </asp:Label>
            <asp:DropDownList ID="ddlFilterByGroup" runat="server"
                Enabled="false">
            </asp:DropDownList>
            <div class="app_horizontal_spacer">
            </div>
            <asp:Button ID="btnSearch" runat="server"
                Text="Search"
                OnClick="OnClickSearch" />
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>