<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="sp_single_patient.aspx.cs" Inherits="sp_single_patient" %>
    
<%@ MasterType VirtualPath="MasterPage.master" %>
<%@ PreviousPageType VirtualPath="~/pl_patient_lookup.aspx" %>
<%@ Register TagPrefix="uc" TagName="PatientChecklist" Src="~/sp_ucPatientChecklist.ascx" %>

<asp:Content ID="ctHead" runat="Server"
    ContentPlaceHolderID="cphHead">
</asp:Content>
<asp:Content ID="ctBody" runat="Server"
    ContentPlaceHolderID="cphBody">
    <asp:UpdatePanel ID="upSinglePatient" runat="server"
        RenderMode="Block">
        <ContentTemplate>
            <uc:PatientChecklist ID="ucPatientChecklist" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
