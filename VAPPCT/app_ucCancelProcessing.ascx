<%@ Control Language="C#" AutoEventWireup="true" CodeFile="app_ucCancelProcessing.ascx.cs" Inherits="app_ucCancelProcessing" %>
<div class="app_Label4">
    Processing...
    <div class="app_horizontal_spacer">
    </div>
    <asp:Label ID="lblCount" runat="server">
    </asp:Label>
    records remaining.
</div>
<div class="app_horizontal_spacer">
</div>
<asp:Button ID="btnCancel" runat="server"
    OnClick="OnClickCancel"
    Text="Cancel Processing" />