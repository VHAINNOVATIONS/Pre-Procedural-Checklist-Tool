<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ve_ucTemporalState.ascx.cs"
    Inherits="ve_ucTemporalState" %>
    
<%@ Register TagPrefix="uc" TagName="TemporalStateEdit" Src="~/ve_ucTemporalStateEdit.ascx" %>

<h1 class="app_label1">
    Temporal State(s)
</h1>
<asp:UpdatePanel ID="upTSEdit" runat="server"
    RenderMode="Block"
    UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="pnlTemporalStates" runat="server"
            SkinID="VariableEditor">
            <asp:GridView ID="gvTemporalStates" runat="server"
                AllowSorting="true"
                DataKeyNames="ts_id"
                OnRowDataBound="OnRowDataBoundTS"
                OnSelectedIndexChanged="OnSelIndexChangedTS"
                OnSorting="OnSortingTS">
                <Columns>
                    <asp:TemplateField AccessibleHeaderText="Label"
                        HeaderStyle-CssClass="gv_pleasewait"
                        HeaderText="Label"
                        ItemStyle-CssClass="gv_pleasewait"
                        SortExpression="TS_LABEL">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkSelect" runat="server"
                                CommandName="Select"
                                ForeColor="Blue"
                                Width="231"
                                CssClass="gv_truncated">
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="Definition"
                        HeaderStyle-CssClass="gv_pleasewait"
                        HeaderText="Definition"
                        SortExpression="TS_DEFINITION_LABEL">
                        <ItemTemplate>
                            <asp:Label ID="lblDefinition" runat="server"
                                Width="100"
                                CssClass="gv_truncated">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="Active"
                        HeaderStyle-CssClass="gv_pleasewait"
                        HeaderText="Active"
                        SortExpression="IS_ACTIVE_LABEL">
                        <ItemTemplate>
                            <asp:Label ID="lblActive" runat="server"
                                Width="50"
                                CssClass="gv_truncated">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="Default"
                        HeaderStyle-CssClass="gv_pleasewait"
                        HeaderText="Default"
                        SortExpression="IS_DEFAULT_LABEL">
                        <ItemTemplate>
                            <asp:Label ID="lblDefault" runat="server"
                                Width="65"
                                CssClass="gv_truncated">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </asp:Panel>
        <div class="app_horizontal_spacer">
        </div>
        <asp:Button ID="btnPopupTSNew" runat="server"
            Text="New"
            OnClick="OnClickNew" />
        <asp:Button ID="btnPopupTSEdit" runat="server"
            Enabled="false"
            OnClick="OnClickEdit"
            Text="Edit" />
        <asp:HiddenField ID="hfMPETarget" runat="server" />
        <asp:ModalPopupExtender ID="mpeTSEdit" runat="server"
            TargetControlID="hfMPETarget"
            PopupControlID="pnlTSEdit">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlTSEdit" runat="server"
            CssClass="temporal_state_edit">
            <uc:TemporalStateEdit id="ucTemporalStateEdit" runat="server"
                Visible="false"
                OnSave="OnSaveTemporalSave" />
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
