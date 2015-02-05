<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ce_ucDecisionStateSelector.ascx.cs"
    Inherits="ce_ucDecisionStateSelector" %>

<h2 class="app_label2">
    Decision States
</h2>
<asp:Panel ID="pnlDS" runat="server"
    SkinID="StateSelector">
    <asp:GridView ID="gvDS" runat="server"
        AllowSorting="true"
        DataKeyNames="ds_id"
        OnRowDataBound="OnRowDataBoundDS"
        OnSelectedIndexChanged="OnSelIndexChangedDS"
        OnSorting="OnSortingDS">
        <Columns>
            <asp:TemplateField AccessibleHeaderText="Label"
                HeaderStyle-CssClass="gv_pleasewait"
                HeaderText="Label"
                SortExpression="ds_label">
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelect" runat="server"
                        CssClass="gv_truncated"
                        Width="200" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField AccessibleHeaderText="Definition"
                HeaderStyle-CssClass="gv_pleasewait"
                HeaderText="Definition"
                SortExpression="ds_definition_label">
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