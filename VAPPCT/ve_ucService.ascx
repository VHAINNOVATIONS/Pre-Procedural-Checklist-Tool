<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ve_ucService.ascx.cs"
    Inherits="ve_ucService" %>
    
<%@ Register TagPrefix="uc" TagName="ServiceEdit" Src="~/ve_ucServiceEdit.ascx" %>

<h1 class="app_label1">
    Service(s)
</h1>
<asp:UpdatePanel ID="upService" runat="server"
    RenderMode="Block"
    UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Panel ID="pnlService" runat="server"
            SkinID="VariableEditor">
            <asp:GridView ID="gvService" runat="server"
                AllowSorting="true"
                DataKeyNames="service_id"
                EmptyDataText="Click &quot;New&quot; to add a Service."
                OnRowDataBound="OnRowDataBoundService"
                OnSelectedIndexChanged="OnSelIndexChangedService"
                OnSorting="OnSortingService">
                <Columns>
                    <asp:TemplateField AccessibleHeaderText="Label"
                        HeaderStyle-CssClass="gv_pleasewait"
                        HeaderText="Label"
                        ItemStyle-CssClass="gv_pleasewait"
                        SortExpression="SERVICE_LABEL">
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
        <asp:Button ID="btnNew" runat="server"
            OnClick="OnClickNew"
            Text="New" />
        <asp:Button ID="btnEdit" runat="server"
            Enabled="false"
            OnClick="OnClickEdit"
            Text="Edit" />
        <!--modal popup for editing-->
        <asp:HiddenField ID="hfMPETarget" runat="server" />
        <asp:ModalPopupExtender ID="mpeServiceEdit" runat="server"
            PopupControlID="pnlServiceEdit"
            TargetControlID="hfMPETarget">
        </asp:ModalPopupExtender>
        <!--user control for edit, must be in a panel to prevent js errors-->
        <asp:Panel ID="pnlServiceEdit" runat="server"
            CssClass="service_edit">
            <uc:ServiceEdit ID="ucServiceEdit" runat="server"
                OnSave="OnSaveService"
                Visible="false" />
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
