<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ce_ucChecklistSelector.ascx.cs"
    Inherits="ce_ucChecklistSelector" %>
    
<h1 class="app_label1">
    <asp:Button ID="btnCollapseFilters" runat="server"
        OnClick="OnClickCollapseFilters"
        Text="-"
        Width="30" />
    Checklist Filters
</h1>
<asp:Panel ID="pnlFilters" runat="server"
    DefaultButton="btnSearch">
    <!-- name -->
    <asp:CheckBox ID="chkFilterByCLName" runat="server"
        AccessKey="C"
        AutoPostBack="true"
        CssClass="app_label4"
        OnCheckedChanged="OnCheckedChangedCLName"
        Text="Enable <span class=access_key>C</span>hecklist Label Filter" />
    <div class="app_horizontal_spacer">
    </div>
    <asp:Label ID="lblCLName" runat="server"
        AccessKey="K"
        AssociatedControlID="txtFilterByCLName"
        CssClass="app_label4"
        Text="Chec<span class=access_key>k</span>list Label">
    </asp:Label>
    <asp:TextBox ID="txtFilterByCLName" runat="server">
    </asp:TextBox>
    <div class="app_horizontal_spacer">
    </div>
    
    <!-- service -->
    <asp:CheckBox ID="chkFilterByCLService" runat="server"
        AccessKey="V"
        AutoPostBack="true"
        CssClass="app_label4"
        OnCheckedChanged="OnCheckedChangedCLService"
        Text="Enable Ser<span class=access_key>v</span>ice Filter" />
    <div class="app_horizontal_spacer">
    </div>
    <asp:Label ID="lblService" runat="server"
        AccessKey="I"
        AssociatedControlID="ddlFilterByService"
        CssClass="app_label4"
        Text="Serv<span class=access_key>i</span>ce">
    </asp:Label>
    <asp:DropDownList ID="ddlFilterByService" runat="server">
    </asp:DropDownList>
    <div class="app_horizontal_spacer">
    </div>
    <asp:Button ID="btnSearch" runat="server"
        OnClick="OnClickSearch"
        Text="Search" />
</asp:Panel>

<h1 class="app_label1">
    Checklist(s)
</h1>
<asp:UpdatePanel ID="upChecklist" runat="server"
    RenderMode="Block"
    UpdateMode="Conditional">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
    </Triggers>
    <ContentTemplate>
        <asp:Panel ID="pnlCLgv" runat="server"
            CssClass="app_panel"
            Height="250"
            ScrollBars="Vertical"
            Width="775">
            <asp:GridView ID="gvCL" runat="server"
                AllowSorting="true"
                DataKeyNames="CHECKLIST_ID"
                EmptyDataText="Please search with zero or more filters."
                OnRowDataBound="OnRowDataBoundCL"
                OnSelectedIndexChanged="OnSelIndexChangedCL"
                OnSorting="OnSortingCL">
                <Columns>
                    <asp:TemplateField AccessibleHeaderText="Checklist"
                        ControlStyle-CssClass="gv_pleasewait"
                        HeaderStyle-CssClass="gv_pleasewait"
                        HeaderText="Checklist"
                        ItemStyle-CssClass="gv_pleasewait"
                        SortExpression="CHECKLIST_LABEL">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkSelect" runat="server"
                                CommandName="Select"
                                CssClass="gv_truncated"
                                ForeColor="Blue"
                                Width="602">
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="Service"
                        HeaderText="Service"
                        HeaderStyle-CssClass="gv_pleasewait"
                        SortExpression="SERVICE_LABEL">
                        <ItemTemplate>
                            <asp:Label ID="lblService" runat="server"
                                CssClass="gv_truncated"
                                Width="150">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
<div class="app_horizontal_spacer">
</div>
<asp:Button ID="btnSelect" runat="server"
    OnClick="OnClickSelect"
    Text="Select" />
<asp:Button ID="btnCancel" runat="server"
    OnClick="OnClickCancel"
    Text="Close" />