<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ve_ucItemGroup.ascx.cs"
    Inherits="ve_ucItemGroup" %>
    
<%@ Register TagPrefix="uc" TagName="ItemGroupsEdit" Src="~/ve_ucItemGroupsEdit.ascx" %>

<h1 class="app_label1">
    Item Group(s)
</h1>
<asp:UpdatePanel ID="upIGEdit" runat="server"
    RenderMode="Block"
    UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="pnlItemGroups" runat="server"
            SkinID="VariableEditor">
            <asp:GridView ID="gvItemGroups" runat="server"
                AllowSorting="true"
                DataKeyNames="item_group_id"
                EmptyDataText="Click &quot;New&quot; to add an Item Group."
                OnRowDataBound="OnRowDataBoundItemGroup"
                OnSelectedIndexChanged="OnSelIndexChangedItemGroup"
                OnSorting="OnSortingItemGroup">
                <Columns>
                    <asp:TemplateField AccessibleHeaderText="Label"
                        HeaderStyle-CssClass="gv_pleasewait"
                        HeaderText="Label"
                        ItemStyle-CssClass="gv_pleasewait"
                        SortExpression="ITEM_GROUP_LABEL">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkSelect" runat="server"
                                CommandName="Select"
                                CssClass="gv_truncated"
                                ForeColor="Blue"
                                Width="402">
                            </asp:LinkButton>
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
        <div class="app_horizontal_spacer">
        </div>
        <asp:Button ID="btnPopupIGNew" runat="server"
            OnClick="OnClickNew"
            Text="New" />
        <asp:Button ID="btnPopupIGEdit" runat="server"
            Enabled="false"
            OnClick="OnClickEdit"
            Text="Edit" />
        <asp:HiddenField ID="hfMPETarget" runat="server" />
        <asp:ModalPopupExtender ID="mpeIGEdit" runat="server"
            PopupControlID="pnlIGEdit"
            TargetControlID="hfMPETarget">
        </asp:ModalPopupExtender>
        <!--user control for edit, must be in a panel to prevent js errors-->
        <asp:Panel ID="pnlIGEdit" runat="server"
            CssClass="item_group_edit">
            <uc:ItemGroupsEdit ID="ucItemGroupsEdit" runat="server"
                OnSave="OnSaveItemGroup"
                Visible="false" />
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
