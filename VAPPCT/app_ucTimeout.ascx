<%@ Control Language="C#" AutoEventWireup="true" CodeFile="app_ucTimeout.ascx.cs" Inherits="app_ucTimeout" %>
<asp:Timer 
    ID="tmrucTimeout" 
    runat="server"
    Interval="100000"
    OnTick="tmrucTimeout_Tick">
</asp:Timer>
<asp:Label Width="251px" ID="lblWarning" runat="server" Text=""></asp:Label>
<div class="app_horizontal_spacer">
</div>
<asp:Button ID="btnStayLoggedIn" runat="server" Text="Stay Logged In" 
    onclick="btnStayLoggedIn_Click" />
<asp:Button ID="btnLogoff" runat="server" Text="Logoff" 
    onclick="btnLogoff_Click" />   
       