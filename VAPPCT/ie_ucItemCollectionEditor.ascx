<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ie_ucItemCollectionEditor.ascx.cs"
    Inherits="ie_ucItemCollectionEditor" %>
    
<%@ Register TagPrefix="uc" TagName="ItemSelector" Src="~/ce_ucItemSelector.ascx" %>
    
<asp:UpdatePanel ID="upItemCollection" runat="server"
    RenderMode="Block">
    <ContentTemplate>
        <h2 class="app_label2">
            Collection Item(s)
            <asp:Button ID="btnAdd" runat="server"
                OnClick="OnClickAdd"
                Text="Add Item" />
        </h2>
        <!-- item selector popup -->
        <asp:HiddenField ID="hfAdd" runat="server" />
        <asp:ModalPopupExtender ID="mpeAddItem" runat="server"
            PopupControlID="pnlAddItem"
            TargetControlID="hfAdd">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlAddItem" runat="server"
            CssClass="item_selector">
            <uc:ItemSelector ID="ucItemSelector" runat="server"
                OnSelect="OnSelectItem"
                Visible="false" />
        </asp:Panel>
        <div class="app_horizontal_spacer">
        </div>
        <asp:Panel ID="pnlGridView" runat="server"
            CssClass="app_panel"
            Height="185"
            ScrollBars="Vertical">
            <asp:GridView ID="gvItemCollection" runat="server"
                DataKeyNames="ITEM_ID"
                OnRowDataBound="OnRowDataBoundItem"
                ShowHeader="false">
                <RowStyle CssClass="gv_rowstyle_bw" />
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label ID="lblItem" runat="server"
                                CssClass="gv_truncated"
                                Width="573">
                            </asp:Label>
                            <asp:Label ID="lblSortOrder" runat="server"
                                Text="Sort Order"
                                AssociatedControlID="tbSortOrder">
                            </asp:Label>
                            <asp:TextBox ID="tbSortOrder" runat="server"
                                MaxLength="2"
                                Width="20">
                            </asp:TextBox>
                            <asp:FilteredTextBoxExtender ID="ftbSortOrder" runat="server"
                                FilterMode="ValidChars"
                                FilterType="Numbers"
                                TargetControlID="tbSortOrder">
                            </asp:FilteredTextBoxExtender>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
