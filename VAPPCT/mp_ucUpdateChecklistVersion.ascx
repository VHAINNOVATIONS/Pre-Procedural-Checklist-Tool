<%@ Control Language="C#" AutoEventWireup="true" CodeFile="mp_ucUpdateChecklistVersion.ascx.cs" Inherits="mp_ucUpdateChecklistVersion" %>
<asp:Panel ID="Panel1" runat="server" Width="710px" Height="300px" ScrollBars="Vertical">


<asp:GridView ID="gvOutOfDateCL" runat="server" Width="690px" 
    AllowSorting="false"
    DataKeyNames="pat_cl_id"
    EmptyDataText="">
    <Columns>
        <asp:TemplateField 
            AccessibleHeaderText="Update"
            HeaderText="Update"
            HeaderStyle-CssClass="gv_pleasewait"
            ItemStyle-CssClass="gv_pleasewait">
            <ItemTemplate>
                <asp:CheckBox ID="chkSelect" runat="server"
                    Width="50"
                    CssClass="gv_truncated" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:boundfield datafield="last_name" headertext="Last Name" />
        <asp:boundfield datafield="first_name" headertext="First Name" />
        <asp:boundfield datafield="checklist_label" headertext="Checklist" />
        <asp:boundfield datafield="cl_version_date" headertext="Current Version" />
        <asp:boundfield datafield="date_last_updated" headertext="New Version" />
        
    </Columns>
</asp:GridView>
</asp:Panel>

<br />
<asp:Button ID="btnSave" runat="server" Text="Apply New Version(s)" 
    onclick="btnSave_Click" />

<asp:Button ID="btnCancel" runat="server" Text="Cancel" 
    onclick="btnCancel_Click" />
