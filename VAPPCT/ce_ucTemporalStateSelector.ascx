<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ce_ucTemporalStateSelector.ascx.cs"
    Inherits="ce_ucTemporalStateSelector" %>

<h2 class="app_label2">
    Temporal States
</h2>
<asp:Panel ID="pnlTS" runat="server"
    SkinID="StateSelector">
    <asp:GridView ID="gvTS" runat="server"
        AllowSorting="true"
        DataKeyNames="ts_id"
        OnRowDataBound="OnRowDataBoundTS"
        OnSelectedIndexChanged="OnSelIndexChangedTS"
        OnSorting="OnSortingTS">
        <Columns>
            <asp:TemplateField AccessibleHeaderText="Label"
                HeaderStyle-CssClass="gv_pleasewait"
                HeaderText="Label"
                SortExpression="ts_label">
                <ItemTemplate>
                    <asp:CheckBox ID="chkSelect" runat="server"
                        CssClass="gv_truncated"
                        Width="200" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField AccessibleHeaderText="Definition"
                HeaderStyle-CssClass="gv_pleasewait"
                HeaderText="Definition"
                SortExpression="ts_definition_label">
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