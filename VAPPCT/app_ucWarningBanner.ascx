<%@ Control Language="C#" AutoEventWireup="true" CodeFile="app_ucWarningBanner.ascx.cs" Inherits="app_ucWarningBanner" %>
<asp:TextBox ID="txtWarning" 
    runat="server" 
    Height="360px" 
    TextMode="MultiLine" 
    Width="600px">
    This system is intended to be used by [authorized VA network users] for viewing and retrieving information only except as otherwise explicitly authorized. VA information resides on and transmits through computer systems and networks funded by VA; all use is considered to be understanding and acceptance that there is no reasonable expectation of privacy for any data or transmissions on Government Intranet or Extranet (non-public) networks or systems. All transactions that occur on this system and all data transmitted through this system are subject to review and action including (but not limited to) monitoring, recording, retrieving, copying, auditing, inspecting, investigating, restricting access, blocking, tracking, disclosing to authorized personnel, or any other authorized actions by all authorized VA and law enforcement personnel. All use of this system constitutes understanding and unconditional acceptance of these terms.
    
    Unauthorized attempts or acts to either (1) access, upload, change, or delete information on this system, (2) modify this system, (3) deny access to this system, or (4) accrue resources for unauthorized use on this system are strictly prohibited. Such attempts or acts are subject to action that may result in criminal, civil, or administrative penalties.
</asp:TextBox>
<div class="app_horizontal_spacer">
        </div>
<asp:Button ID="btnOK" runat="server" Text="I accept the above terms and conditions" />

<asp:Button ID="btnCancel" runat="server" Text="Cancel" 
    onclick="btnCancel_Click" />
    <div class="app_horizontal_spacer">
        </div>
