<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ie_ucMapNoteTitle.ascx.cs"
    Inherits="ie_ucMapNoteTitle" %>
    
<asp:Panel ID="pnlNoteTitle" runat="server"
    DefaultButton="btnSelect">
    <asp:Panel ID="pnlSearch" runat="server"
        DefaultButton="btnSearchOptions">
        <asp:Label ID="lblSearchOptions" runat="server"
            Text="<span class=access_key>S</span>earch Note Title(s)"
            AccessKey="S"
            AssociatedControlID="txtSearchOptions"
            CssClass="app_label4">
        </asp:Label>
        <asp:TextBox ID="txtSearchOptions" runat="server"
            Width="300">
        </asp:TextBox>
        <asp:Button ID="btnSearchOptions" runat="server"
            Text="Search"
            OnClick="btnSearchOptions_Click" />
    </asp:Panel>
    <div class="app_horizontal_spacer">
    </div>
    <asp:Label ID="lblNoteTitles" runat="server"
        Text="<span class=access_key>N</span>ote Title(s)"
        AssociatedControlID="lbNoteTitles"
        CssClass="app_label4">
    </asp:Label>
    <div class="app_horizontal_spacer">
    </div>
    <asp:ListBox ID="lbNoteTitles" runat="server"
        AccessKey="N"
        Width="750"
        Height="300">
    </asp:ListBox>
    <div class="app_horizontal_spacer">
    </div>
    <asp:Button ID="btnSelect" runat="server"
        Text="Select"
        OnClick="btnSelect_Click" />
    <asp:Button ID="btnCancel" runat="server"
        Text="Cancel"
        OnClick="btnCancel_Click" />
    <div class="app_horizontal_spacer">
    </div>
</asp:Panel>
