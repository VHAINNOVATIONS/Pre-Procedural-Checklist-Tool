<%@ Page Title="Help" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="app_help.aspx.cs" Inherits="app_help" %>

<%@ MasterType VirtualPath="MasterPage.master" %>

<asp:Content ID="cHead" runat="Server"
    ContentPlaceHolderID="cphHead">
</asp:Content>
<asp:Content ID="cBody" runat="Server"
    ContentPlaceHolderID="cphBody">
    <iframe src="vappctusermanual.pdf" width="100%" height="600px" />
</asp:Content>
