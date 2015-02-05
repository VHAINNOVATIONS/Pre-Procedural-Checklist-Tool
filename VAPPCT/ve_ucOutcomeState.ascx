<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ve_ucOutcomeState.ascx.cs"
    Inherits="ve_ucOutcomeState" %>
    
<%@ Register TagPrefix="uc" TagName="OutcomeStateEdit" Src="~/ve_ucOutcomeStateEdit.ascx" %>
    
<h1 class="app_label1">
    Outcome State(s)
</h1>
<asp:UpdatePanel ID="upOSEdit" runat="server"
    RenderMode="Block"
    UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="pnlOutcomeStates" runat="server"
            SkinID="VariableEditor">
            <asp:GridView ID="gvOutcomeStates" runat="server"
                AllowSorting="true"
                DataKeyNames="os_id"
                OnRowDataBound="OnRowDataBoundOS"
                OnSelectedIndexChanged="OnSelIndexChangedOS"
                OnSorting="OnSortingOS">
                <Columns>
                    <asp:TemplateField AccessibleHeaderText="Label"
                        HeaderStyle-CssClass="gv_pleasewait"
                        HeaderText="Label"
                        ItemStyle-CssClass="gv_pleasewait"
                        SortExpression="OS_LABEL">
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
                        SortExpression="OS_DEFINITION_LABEL">
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
        <asp:Button ID="btnPopupOSNew" runat="server"
            OnClick="OnClickNew"
            Text="New" />
        <asp:Button ID="btnPopupOSEdit" runat="server"
            Enabled="false"
            OnClick="OnClickEdit"
            Text="Edit" />
        <asp:HiddenField ID="hfMPETarget" runat="server" />
        <asp:ModalPopupExtender ID="mpeOSEdit" runat="server"
            PopupControlID="pnlOSEdit"
            TargetControlID="hfMPETarget">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlOSEdit" runat="server"
            CssClass="outcome_state_edit">
            <uc:OutcomeStateEdit ID="ucOutcomeStateEdit" runat="server"
                OnSave="OnSaveOutcomeState"
                Visible="false" />
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
