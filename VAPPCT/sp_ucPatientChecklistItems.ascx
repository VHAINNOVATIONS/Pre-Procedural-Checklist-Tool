<%@ Control Language="C#" AutoEventWireup="true" CodeFile="sp_ucPatientChecklistItems.ascx.cs"
    Inherits="sp_ucPatientChecklistItems" %>
    
<%@ Register TagPrefix="uc" TagName="ItemEditor" Src="~/sp_ucPatItemEditor.ascx" %>
<%@ Register TagPrefix="uc" TagName="PatCLIEditor" Src="~/sp_ucPatCLIEditor.ascx" %>
<%@ Register TagPrefix="uc" TagName="ViewValuePopup" Src="~/sp_ucViewValuePopup.ascx" %>
<%@ Register TagPrefix="uc" TagName="PatItemHistory" Src="~/sp_ucPatItemHistory.ascx" %>

<div align="center">
    <div style="height:30px; width:160px;">
        <div style="float:left; padding-top:7px;">
            <asp:Label ID="lblSummaryState" runat="server"
                AssociatedControlID="imgSummaryState"
                CssClass="app_label1"
                Text="Summary State"
                Visible="false">
            </asp:Label>
        </div>
        <div style="float:right;">
            <asp:Image ID="imgSummaryState" runat="server"
                AlternateText="Summary State Image"
                Height="30"
                Visible="false" 
                Width="30" />
        </div>
    </div>
</div>
<h2 class="app_label1"
    style="clear:both;">
    Checklist Item(s)
</h2>
<asp:Panel ID="pnlPatCLItems" runat="server"
    CssClass="app_panel"
    Height="342"
    ScrollBars="Vertical">
    <asp:GridView ID="gvPatCLItems" runat="server"
        DataKeyNames="ITEM_ID" 
        OnRowDataBound="OnRowDataBoundItem">
        <RowStyle CssClass="gv_rowstyle_bw" />
        <Columns>
            <asp:TemplateField AccessibleHeaderText="Row State"
                HeaderText="Row State"
                HeaderStyle-Width="40"
                ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:HiddenField ID="hfEnableID" runat="server" />
                    <asp:Image ID="imgRowState" runat="server"
                        AlternateText="Row State Image" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField AccessibleHeaderText="Item"
                HeaderText="Item">
                <ItemTemplate>
                    <asp:Label ID="lblItemLabel" runat="server"
                        CssClass="gv_truncated"
                        Width="181" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField AccessibleHeaderText="Values"
                HeaderText="Values">
                <ItemTemplate>
                    <asp:Panel ID="pnlComponents" runat="server"
                        Visible="false">
                        <asp:Button ID="btnShowHiddenValues" runat="server"
                            OnClientClick="javascript:if(this.value == '+'){this.value = '-';}else{this.value = '+';}"
                            Text="-"
                            Width="30" />
                        <asp:Label ID="lblValues" runat="server"
                            Width="200">
                            <asp:Panel ID="pnlShownValues" runat="server">
                                <asp:Literal ID="litShownValues" runat="server" />
                            </asp:Panel>
                            <asp:Panel ID="pnlHiddenValues" runat="server">
                                <asp:Literal ID="litHiddenValues" runat="server" />
                            </asp:Panel>
                            <asp:CollapsiblePanelExtender ID="cpeHiddenValues" runat="server"
                                AutoCollapse="false"
                                AutoExpand="false"
                                CollapseControlID="btnShowHiddenValues"
                                Collapsed="false" 
                                ExpandControlID="btnShowHiddenValues"
                                ExpandDirection="Vertical"
                                SuppressPostBack="true"
                                TargetControlID="pnlHiddenValues" />
                        </asp:Label>
                    </asp:Panel>
                    <asp:Panel ID="pnlViewValue" runat="server"
                        Visible="false"
                        Width="230">
                        <asp:Button ID="btnViewValue" runat="server"
                            OnClick="OnClickViewValue"
                            Text="View Value(s)"/>
                    </asp:Panel>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-VerticalAlign="Middle">
                <HeaderTemplate>
                    <asp:Image ID="imgTSColumnState" runat="server"
                        AlternateText="Temporal State Column State Image"
                        Visible="false" />
                    <div class="app_horizontal_spacer">
                    </div>
                    TS
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblTemporalState" runat="server"
                        CssClass="gv_truncated"
                        Height="23"
                        Width="110">
                    </asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-VerticalAlign="Middle">
                <HeaderTemplate>
                    <asp:Image ID="imgOSColumnState" runat="server"
                        AlternateText="Outcome State Column State Image"
                        Visible="false" />
                    <div class="app_horizontal_spacer">
                    </div>
                    OS
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblOutcomeState" runat="server"
                        CssClass="gv_truncated"
                        Height="23"
                        Width="110">
                    </asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField ItemStyle-VerticalAlign="Middle">
                <HeaderTemplate>
                    <asp:Image ID="imgDSColumnState" runat="server"
                        AlternateText="Decision State Column State Image"
                        Visible="false" />
                    <div class="app_horizontal_spacer">
                    </div>
                    DS
                </HeaderTemplate>
                <ItemTemplate>
                    <asp:Label ID="lblDecisionState" runat="server"
                        CssClass="gv_truncated"
                        Height="23"
                        Width="110">
                    </asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField AccessibleHeaderText="Options"
                HeaderText="Options">
                <ItemTemplate>
                    <asp:Button ID="btnOverride" runat="server"
                        OnClick="OnClickOverrideItem"
                        Text="Ovr" />
                    <asp:Button ID="btnEdit" runat="server"
                        OnClick="OnClickEditItem"
                        Text="Upd" />
                    <asp:Button ID="btnTrend" runat="server"
                        OnClick="OnClickTrend"
                        Text="Trd" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Panel>

<asp:HiddenField ID="hfTargetOne" runat="server" />
<asp:ModalPopupExtender ID="mpeItemEditor" runat="server"
    PopupControlID="pnlUCItemEditor"
    TargetControlID="hfTargetOne">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlUCItemEditor" runat="server"
    CssClass="patient_item_editor">
    <uc:ItemEditor ID="ucItemEditor" runat="server"
        Visible="false" />
</asp:Panel>

<asp:HiddenField ID="hfTargetTwo" runat="server" />
<asp:ModalPopupExtender ID="mpePatCLIEditor" runat="server"
    PopupControlID="pnlPatCLIEditor"
    TargetControlID="hfTargetTwo">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlPatCLIEditor" runat="server"
    CssClass="pat_cli_editor">
    <uc:PatCLIEditor ID="ucPatCLIEditor" runat="server"
        Visible="false" />
</asp:Panel>

<asp:HiddenField ID="hfTargetThree" runat="server" />
<asp:ModalPopupExtender ID="mpeViewValuePopup" runat="server"
    PopupControlID="pnlViewValuePopup"
    TargetControlID="hfTargetThree">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlViewValuePopup" runat="server"
    CssClass="view_value_popup">
    <uc:ViewValuePopup ID="ucViewValuePopup" runat="server"
        Visible="false" />
</asp:Panel>

<asp:HiddenField ID="hfPatItemHistory" runat="server" />
<asp:ModalPopupExtender ID="mpePatItemHistory" runat="server"
    PopupControlID="pnlPatItemHistory"
    TargetControlID="hfPatItemHistory">
</asp:ModalPopupExtender>
<asp:Panel ID="pnlPatItemHistory" runat="server"
    CssClass="item_history">
    <uc:PatItemHistory ID="ucPatItemHistory" runat="server"
        Visible="false" />
</asp:Panel>
