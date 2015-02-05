<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ce_ucSaveAs.ascx.cs" Inherits="ce_ucSaveAs" %>

<asp:Panel ID="pnlCopy" runat="server"
    DefaultButton="btnucSave">
    <asp:Label ID="lblCopy" runat="server"
        Text="Copy:"
        CssClass="app_label2"
        AssociatedControlID="lblTarget">
    </asp:Label>
    <asp:Label ID="lblTarget" runat="server"
        Width="345"
        CssClass="app_label4">
    </asp:Label>
    <div class="app_horizontal_spacer">
    </div>
    <asp:Label ID="lblAs" runat="server"
        Text="<span class=access_key>A</span>s"
        CssClass="app_label4"
        AccessKey="A"
        AssociatedControlID="txtAs">
    </asp:Label>
    <asp:TextBox ID="txtAs" runat="server"
        Width="365">
    </asp:TextBox>
    <div class="app_horizontal_spacer">
    </div>
    <div class="app_label4">
        <span class="app_label2">Note:</span>&nbsp;The entire checklist including commited changes will be copied. Any changes made to the checklist that were not saved prior to opening this dialog will be lost.
    </div>
</asp:Panel>
<div class="app_horizontal_spacer">
</div>
<asp:Button ID="btnucSave" runat="server"
    Text="Save"
    OnClick="btnucSave_Click" />
<asp:Button ID="btnucCancel" runat="server"
    Text="Cancel"
    OnClick="btnucCancel_Click" />