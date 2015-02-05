<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ce_ucOutcomeStateSelector.ascx.cs"
    Inherits="ce_ucOutcomeStateSelector" %>

<h2 class="app_label2">
    Outcome States
</h2>
<asp:Panel ID="pnlOS" runat="server"
    SkinID="StateSelector">
    <asp:GridView ID="gvOS" runat="server"
        AllowSorting="true"
        DataKeyNames="os_id"
        OnRowDataBound="OnRowDataBoundOS"
        OnSelectedIndexChanged="OnSelIndexChangedOS"
        OnSorting="OnSortingOS">
        <Columns>
            <asp:TemplateField AccessibleHeaderText="Label"
                HeaderStyle-CssClass="gv_pleasewait"
                HeaderText="Label"
                SortExpression="os_label">
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelect" runat="server"
                        CssClass="gv_truncated"
                        Width="200" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField AccessibleHeaderText="Definition"
                HeaderStyle-CssClass="gv_pleasewait"
                HeaderText="Definition"
                SortExpression="os_definition_label">
                <ItemTemplate>
                    <asp:Label ID="lblDefinition" runat="server"
                        CssClass="gv_truncated"
                        Width="75">
                    </asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Panel>