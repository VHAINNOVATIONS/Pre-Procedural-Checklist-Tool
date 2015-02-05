<%@ Control Language="C#" AutoEventWireup="true" CodeFile="sp_ucTIUNote.ascx.cs"
    Inherits="sp_ucTIUNote" %>
    
<asp:Panel ID="pnlTIUNote" runat="server"
    DefaultButton="btnSave">
    <asp:Label ID="lblNoteTitle" runat="server"
        CssClass="app_label1">
    </asp:Label>
    <div class="app_horizontal_spacer">
    </div>
    <asp:Label ID="lblSelectClinic" runat="server"
        Text="<span class=access_key>C</span>linic"
        CssClass="app_label4"
        AccessKey="C"
        AssociatedControlID="ddlClinics">
    </asp:Label>
    <asp:DropDownList ID="ddlClinics" runat="server">
    </asp:DropDownList>
    <div class="app_horizontal_spacer">
    </div>
    <asp:Label ID="lblNoteText" runat="server"
        Text="<span class=access_key>N</span>ote Text"
        CssClass="app_label4"
        AccessKey="N"
        AssociatedControlID="txtTIU">
    </asp:Label>
    <div class="app_horizontal_spacer">
    </div>
    <asp:TextBox ID="txtTIU" runat="server"
        TextMode="MultiLine"
        Height="350"
        Width="700">
    </asp:TextBox>
    <div class="app_horizontal_spacer">
    </div>
    <asp:Label ID="lblSign" runat="server"
        Text="<span class=access_key>S</span>ign"
        CssClass="app_label4"
        AccessKey="S"
        AssociatedControlID="txtSign">
    </asp:Label>
    <asp:TextBox ID="txtSign" runat="server"
        TextMode="Password">
    </asp:TextBox>
    <div class="app_horizontal_spacer">
    </div>
    <asp:Button ID="btnSave" runat="server"
        Text="Save"
        OnClick="btnSave_Click" />
    <asp:Button ID="btnCancel" runat="server"
        Text="Cancel"
        OnClick="btnCancel_Click" />
</asp:Panel>