<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ve_ucDecisionState.ascx.cs"
    Inherits="ve_ucDecisionState" %>
    
<%@ Register TagPrefix="uc" TagName="DecisionStateEdit" Src="~/ve_ucDecisionStateEdit.ascx" %>

<h1 class="app_label1">
    Decision State(s)
</h1>
<asp:UpdatePanel ID="upDSEdit" runat="server"
    RenderMode="Block"
    UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="pnlDecisionStates" runat="server"
            SkinID="VariableEditor">
            <asp:GridView ID="gvDecisionStates" runat="server"
                AllowSorting="true"
                DataKeyNames="ds_id"
                OnRowDataBound="OnRowDataBoundDS"
                OnSelectedIndexChanged="OnSelIndexChangedDS"
                OnSorting="OnSortingDS">
                <Columns>
                    <asp:TemplateField AccessibleHeaderText="Label"
                        HeaderStyle-CssClass="gv_pleasewait"
                        HeaderText="Label"
                        ItemStyle-CssClass="gv_pleasewait"
                        SortExpression="DS_LABEL">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkSelect" runat="server"
                                CommandName="Select"
                                CssClass="gv_truncated"
                                ForeColor="Blue"
                                Width="231">
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="Definition"
                        HeaderStyle-CssClass="gv_pleasewait"
                        HeaderText="Definition"
                        SortExpression="DS_DEFINITION_LABEL">
                        <ItemTemplate>
                            <asp:Label ID="lblDefinition" runat="server"
                                CssClass="gv_truncated"
                                Width="100">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="Active"
                        HeaderStyle-CssClass="gv_pleasewait"
                        HeaderText="Active"
                        SortExpression="IS_ACTIVE_LABEL">
                        <ItemTemplate>
                            <asp:Label ID="lblActive" runat="server"
                                CssClass="gv_truncated"
                                Width="50">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="Default"
                        HeaderStyle-CssClass="gv_pleasewait"
                        HeaderText="Default"
                        SortExpression="IS_DEFAULT_LABEL">
                        <ItemTemplate>
                            <asp:Label ID="lblDefault" runat="server"
                                CssClass="gv_truncated"
                                Width="65">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </asp:Panel>
        <div class="app_horizontal_spacer">
        </div>
        <asp:Button ID="btnPopupDSNew" runat="server"
            OnClick="OnClickNew"
            Text="New" />
        <asp:Button ID="btnPopupDSEdit" runat="server"
            Enabled="false"
            OnClick="OnClickEdit"
            Text="Edit" />
        <asp:HiddenField ID="hfMPETarget" runat="server" />
        <asp:ModalPopupExtender ID="mpeDSEdit" runat="server"
            PopupControlID="pnlDSEdit"
            TargetControlID="hfMPETarget">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlDSEdit" runat="server"
            CssClass="decision_state_edit">
            <uc:DecisionStateEdit ID="ucDecisionStateEdit" runat="server"
                OnSave="OnSaveDecisionState"
                Visible="false" />
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
