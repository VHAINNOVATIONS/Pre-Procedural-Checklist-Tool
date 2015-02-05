<%@ Control Language="C#" AutoEventWireup="true" CodeFile="sp_ucExistingChecklist.ascx.cs"
    Inherits="sp_ucExistingChecklist" %>
<div class="app_label4">
    The patient currently has the selected checklist open. Do you want to assign the checklist to the patient again?
</div>
<div class="app_horizontal_spacer">
</div>
<asp:Button ID="btnContinue" runat="server"
    Text="Yes"
    OnClick="OnClickContinue" />
<asp:Button ID="btnCancel" runat="server"
    Text="No"
    OnClick="OnClickCancel" />