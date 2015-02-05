<%@ Page Title="Item Editor" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="ie_item_editor.aspx.cs" Inherits="ie_item_editor" %>

<%@ MasterType VirtualPath="MasterPage.master" %>
<%@ Register TagPrefix="uc" TagName="ItemLookup" Src="~/app_ucItemLookup.ascx" %>
<%@ Register TagPrefix="uc" TagName="ItemEditor" Src="~/ie_ucItemEditor.ascx" %>

<asp:Content ID="ctHead" runat="Server"
    ContentPlaceHolderID="cphHead">
</asp:Content>
<asp:Content ID="ctBody" runat="Server"
    ContentPlaceHolderID="cphBody">
    <asp:UpdatePanel ID="upItemLookup" runat="server"
        RenderMode="Block"
        UpdateMode="Always">
        <ContentTemplate>
            <uc:ItemLookup ID="ucItemLookup" runat="server"
                OnCollapse="OnFiltersCollapse"
                OnSearch="OnSearchItems" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <h1 class="app_label1">
        Item(s)
    </h1>
    <asp:UpdatePanel ID="upItems" runat="server"
        RenderMode="Block"
        UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ucItemLookup" EventName="Search" />
            <asp:AsyncPostBackTrigger ControlID="ucItemLookup" EventName="Collapse" />
        </Triggers>
        <ContentTemplate>
            <asp:Panel ID="pnlGridView" runat="server"
                CssClass="app_panel"
                Height="302"
                ScrollBars="Vertical"
                Width="960">
                <asp:GridView ID="gvItems" runat="server"
                    AllowSorting="true"
                    DataKeyNames="ITEM_ID"
                    EmptyDataText="Please search with one or more filters."
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
                                    CssClass="gv_truncated"
                                    ForeColor="Blue"
                                    Width="245">
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField AccessibleHeaderText="Type"
                            HeaderStyle-CssClass="gv_pleasewait"
                            HeaderText="Type"
                            SortExpression="ITEM_TYPE_LABEL">
                            <ItemTemplate>
                                <asp:Label ID="lblType" runat="server"
                                    CssClass="gv_truncated"
                                    Width="120">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField AccessibleHeaderText="Group"
                            HeaderStyle-CssClass="gv_pleasewait"
                            HeaderText="Group"
                            SortExpression="ITEM_GROUP_LABEL">
                            <ItemTemplate>
                                <asp:Label ID="lblGroup" runat="server"
                                    CssClass="gv_truncated"
                                    Width="100">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField AccessibleHeaderText="Description"
                            HeaderStyle-CssClass="gv_pleasewait"
                            HeaderText="Description"
                            SortExpression="ITEM_DESCRIPTION">
                            <ItemTemplate>
                                <asp:Label ID="lblDescription" runat="server"
                                    CssClass="gv_truncated"
                                    Width="350">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField AccessibleHeaderText="Lookback Time"
                            HeaderStyle-CssClass="gv_pleasewait"
                            HeaderText="Lookback Time"
                            SortExpression="LOOKBACK_TIME">
                            <ItemTemplate>
                                <asp:Label ID="lblLookbackTime" runat="server"
                                    CssClass="gv_truncated"
                                    Width="60">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField AccessibleHeaderText="Active"
                            HeaderStyle-CssClass="gv_pleasewait"
                            HeaderText="Active"
                            SortExpression="ACTIVE_LABEL">
                            <ItemTemplate>
                                <asp:Label ID="lblActive" runat="server"
                                    CssClass="gv_truncated"
                                    Width="50">
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
    <asp:UpdatePanel ID="upDialog" runat="server"
        RenderMode="Block"
        UpdateMode="Always">
        <ContentTemplate>
            <asp:Button ID="btnAdd" runat="server"
                OnClick="OnClickAdd"
                Text="New" />
            <asp:Button ID="btnEdit" runat="server"
                Enabled="false"
                OnClick="OnClickEdit"
                Text="Edit" />
            <!-- item editor dialog -->
            <asp:HiddenField ID="hfTarget" runat="server" />
            <asp:ModalPopupExtender ID="mpeItemEditor" runat="server"
                PopupControlID="pnlItemEditor"
                TargetControlID="hfTarget">
            </asp:ModalPopupExtender>
            <asp:Panel ID="pnlItemEditor" runat="server"
                CssClass="item_editor">
                <uc:ItemEditor ID="ucItemEditor" runat="server"
                    Visible="false" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
