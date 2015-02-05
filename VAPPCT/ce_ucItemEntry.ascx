<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ce_ucItemEntry.ascx.cs"
    Inherits="ce_ucItemEntry" %>
    
<%@ Register TagPrefix="uc" TagName="ItemSelector" Src="~/ce_ucItemSelector.ascx" %>
<%@ Register TagPrefix="uc" TagName="StateLogicEditor" Src="~/ce_ucStateLogicEditor.ascx" %>

<h2 class="app_label2">
    Checklist Item(s)
    <asp:Button ID="btnAddItem" runat="server"
        Text="Add Item"
        OnClick="OnClickAddItem"
        Enabled="false" />
</h2>
<asp:Panel ID="pnlChecklistItems" runat="server"
    CssClass="app_panel"
    Height="137"
    ScrollBars="Vertical"
    Width="960">
    <asp:GridView ID="gvChecklistItems" runat="server"
        DataKeyNames="ITEM_ID"
        OnRowDataBound="OnRowDataBoundItem"
        ShowHeader="false">
        <RowStyle CssClass="gv_rowstyle_bw" />
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:CheckBox ID="chkItemActive" runat="server"
                        Text="Active"
                        Width="70" />
                    <asp:Label ID="lblItemLabel" runat="server"
                        Width="262"
                        CssClass="gv_truncated" />
                    <asp:Label ID="lblTSTimePeriod" runat="server"
                        Text="Temporal State Time Period"
                        AssociatedControlID="txtTSTimePeriod" />
                    <asp:TextBox ID="txtTSTimePeriod" runat="server"
                        Width="30"
                        MaxLength="4" />
                    <asp:FilteredTextBoxExtender ID="ftbTSTimePeriod" runat="server"
                        FilterType="Numbers"
                        TargetControlID="txtTSTimePeriod" />
                    <asp:Label ID="lblTimePeriodUnits" runat="server"
                        Text="Time Period Units"
                        AssociatedControlID="ddlUnit" />
                    <asp:DropDownList ID="ddlUnit" runat="server">
                    </asp:DropDownList>
                    <asp:Label ID="lblSortOrder" runat="server"
                        Text="Sort Order"
                        AssociatedControlID="txtSortOrder" />
                    <asp:TextBox ID="txtSortOrder" runat="server"
                        Width="25"
                        MaxLength="3" />
                    <asp:FilteredTextBoxExtender ID="ftbSortOrder" runat="server"
                        FilterType="Numbers"
                        TargetControlID="txtSortOrder" />
                    <asp:Button ID="btnStates" runat="server"
                        OnClick="OnClickStates" 
                        Text="States & Logic" />
                    <div class="app_horizontal_spacer">
                    </div>
                    <div style="float:left; width:175px; padding-top:5px;">
                        <asp:Label ID="lblDSChangeable" runat="server"
                            Text="Decision State Changeable By"
                            AssociatedControlID="cblDSChangeable" />
                    </div>
                    <div style="float:right; width:760px;">
                        <div style="float:left;">
                            <asp:CheckBoxList ID="cblDSChangeable" runat="server"
                                DataValueField="USER_ROLE_ID"
                                DataTextField="USER_ROLE_LABEL"
                                RepeatDirection="Horizontal" />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Panel>

<!-- item selector popup -->
<asp:HiddenField ID="hfTargetItem" runat="server" />
<asp:ModalPopupExtender ID="mpeAddItem" runat="server"
    TargetControlID="hfTargetItem"
    PopupControlID="pnlAddItem">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlAddItem" runat="server"
    CssClass="item_selector">
    <uc:ItemSelector ID="ucItemSelector" runat="server"
        Visible="false"
        OnSelect="OnSelectItem" />
</asp:Panel>

<!-- item state popup -->
<asp:HiddenField ID="hfTargetState" runat="server" />
<asp:ModalPopupExtender ID="mpeStateSelect" runat="server"
    TargetControlID="hfTargetState"
    PopupControlID="pnlStateSelect">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlStateSelect" runat="server"
    CssClass="state_selector">
    <uc:StateLogicEditor ID="ucStateLogicEditor" runat="server"
        Visible="false" />
</asp:Panel>
