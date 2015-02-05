<%@ Page Title="Variable Editor" Language="C#" MasterPageFile="~/MasterPage.master"
    AutoEventWireup="true" CodeFile="ve_variable_editor.aspx.cs" Inherits="ve_variable_editor" %>

<%@ MasterType VirtualPath="MasterPage.master" %>
<%@ Register TagPrefix="uc" TagName="TemporalState" Src="~/ve_ucTemporalState.ascx" %>
<%@ Register TagPrefix="uc" TagName="OutcomeState" Src="~/ve_ucOutcomeState.ascx" %>
<%@ Register TagPrefix="uc" TagName="DecisionState" Src="~/ve_ucDecisionState.ascx" %>
<%@ Register TagPrefix="uc" TagName="ItemGroup" Src="~/ve_ucItemGroup.ascx" %>
<%@ Register TagPrefix="uc" TagName="Service" Src="~/ve_ucService.ascx" %>

<asp:Content ID="ctHead" runat="Server"
    ContentPlaceHolderID="cphHead">
</asp:Content>
<asp:Content ID="ctBody" runat="Server"
    ContentPlaceHolderID="cphBody">
    <div style="float: left;">
        <uc:TemporalState ID="ucTemporalState" runat="server" />
    </div>
    <div style="float: right;">
        <uc:OutcomeState ID="ucOutcomeState" runat="server" />
    </div>
    <div class="app_horizontal_spacer">
    </div>
    <div style="float: left;">
        <uc:DecisionState ID="ucDecisionState" runat="server" />
    </div>
    <div style="float: right;">
        <uc:ItemGroup ID="ucItemGroup" runat="server" />
    </div>
    <div class="app_horizontal_spacer">
    </div>
    <div style="float: left;">
        <uc:Service ID="ucService" runat="server" />        
    </div>
</asp:Content>
