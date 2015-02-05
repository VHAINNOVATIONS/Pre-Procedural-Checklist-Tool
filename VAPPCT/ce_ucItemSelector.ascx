<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ce_ucItemSelector.ascx.cs"
    Inherits="ce_ucItemSelector" %>
    
<%@ Register TagPrefix="uc" TagName="ItemLookup" Src="~/app_ucItemLookup.ascx" %>

<uc:ItemLookup ID="ucItemLookup" runat="server"
    OnCollapse="OnFiltersCollapse"
    OnSearch="OnSearchItems" />
<h1 class="app_label1">
    Item(s)
</h1>
<asp:UpdatePanel ID="upItemList" runat="server"
    RenderMode="Block"
    UpdateMode="Conditional">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="ucItemLookup" EventName="Search" />
    </Triggers>
    <ContentTemplate>
        <asp:Panel ID="pnlItemSelector" runat="server"
            CssClass="app_panel"
            Height="250"
            ScrollBars="Vertical">
            <asp:GridView ID="gvItems" runat="server"
                AllowSorting="true"
                DataKeyNames="item_id"
                EmptyDataText="Please search with zero or more filters."
                OnRowDataBound="OnRowDataBoundItem"
                OnSelectedIndexChanged="OnSelIndexChangedItem"
                OnSorting="OnSortingItem">
                <Columns>
                    <asp:TemplateField AccessibleHeaderText="Label"
                        HeaderStyle-CssClass="gv_pleasewait"
                        HeaderText="Label"
                        ItemStyle-CssClass="gv_pleasewait"
                        SortExpression="ITEM_LABEL">
                        <ItemTemplate>
                        <asp:LinkButton ID="lnkSelect" runat="server"
                            CommandName="Select"
                            ForeColor="Blue"
                            Width="185"
                            CssClass="gv_truncated">
                        </asp:LinkButton>
                    </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="Type"
                        HeaderText="Type"
                        HeaderStyle-CssClass="gv_pleasewait"
                        SortExpression="ITEM_TYPE_LABEL">
                        <ItemTemplate>
                            <asp:Label ID="lblType" runat="server"
                                Width="115"
                                CssClass="gv_truncated">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="Group"
                        HeaderText="Group"
                        HeaderStyle-CssClass="gv_pleasewait"
                        SortExpression="ITEM_GROUP_LABEL">
                        <ItemTemplate>
                            <asp:Label ID="lblGroup" runat="server"
                                Width="100"
                                CssClass="gv_truncated">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="Description"
                        HeaderText="Description"
                        HeaderStyle-CssClass="gv_pleasewait"
                        SortExpression="ITEM_DESCRIPTION">
                        <ItemTemplate>
                            <asp:Label ID="lblDescription" runat="server"
                                Width="280"
                                CssClass="gv_truncated">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="Lookback Time"
                        HeaderText="Lookback Time"
                        HeaderStyle-CssClass="gv_pleasewait"
                        SortExpression="LOOKBACK_TIME">
                        <ItemTemplate>
                            <asp:Label ID="lblLookbackTime" runat="server"
                                Width="60"
                                CssClass="gv_truncated">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="Active"
                        HeaderText="Active"
                        HeaderStyle-CssClass="gv_pleasewait"
                        SortExpression="ACTIVE_LABEL">
                        <ItemTemplate>
                            <asp:Label ID="lblActive" runat="server"
                                Width="50"
                                CssClass="gv_truncated">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
<!--spacer-->
<div class="app_horizontal_spacer">
</div>
<asp:Button ID="btnSelect" runat="server"
    Text="Select"
    OnClick="OnClickSelect" />
<asp:Button ID="btnCancel" runat="server"
    Text="Cancel"
    OnClick="OnClickCancel" />
