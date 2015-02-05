<%@ Control Language="C#" AutoEventWireup="true" CodeFile="sp_ucSinglePatientPopup.ascx.cs"
    Inherits="sp_ucSinglePatientPopup" %>
    
<%@ Register TagPrefix="uc" TagName="SinglePatientEditor" Src="~/sp_ucPatientChecklist.ascx" %>

<uc:SinglePatientEditor ID="ucSinglePatientEditor" runat="server" />
<div class="app_horizontal_spacer">
</div>
<asp:Button ID="btnOK" runat="server" Text="Ok"
    Width="75" />
<asp:Button ID="btnCancel" runat="server" Text="Cancel"
    Width="75" />
