<%@ Page Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true"
    CodeFile="VAPPCTHome.aspx.cs" Inherits="VAPPCTHome" %>

<%@ MasterType VirtualPath="MasterPage.master" %>
<%@ Register TagPrefix="uc" TagName="Login" Src="~/app_ucLogin.ascx" %>
<%@ Register TagPrefix="uc" TagName="WarningBanner" Src="~/app_ucWarningBanner.ascx" %>

<asp:Content ID="ctHead" runat="Server"
    ContentPlaceHolderID="cphHead">
</asp:Content>
<asp:Content ID="ctBody" runat="Server"
    ContentPlaceHolderID="cphBody">
    <asp:UpdatePanel ID="upTLogin" runat="server"
        RenderMode="Block">
        <ContentTemplate>
            <asp:Button ID="btnLogin" runat="server"
                OnClick="OnClickLogin"
                Text="Login" />
            <asp:HiddenField ID="hfTarget" runat="server" />
            <asp:ModalPopupExtender ID="mpeLogin" runat="server"
                PopupControlID="pnlLogin"
                TargetControlID="hfTarget">
            </asp:ModalPopupExtender>
            <asp:Panel ID="pnlLogin" runat="server"
                CssClass="login">
                <uc:Login ID="app_ucLogin" runat="server"
                    Visible="false" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <asp:UpdatePanel ID="upWarningBanner" runat="server"
        RenderMode="Block">
        <ContentTemplate>
            <asp:HiddenField ID="hfWarningBanner" runat="server" />
            <asp:ModalPopupExtender ID="mpeWarningBanner" runat="server"
                PopupControlID="pnlWarningBanner"
                TargetControlID="hfWarningBanner">
            </asp:ModalPopupExtender>
            <asp:Panel ID="pnlWarningBanner" runat="server">
                <uc:WarningBanner ID="app_ucWarningBanner" runat="server"
                    Visible="false" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    
    <div class="app_horizontal_spacer">
    </div>
    Welcome to the Pre-Procedure Checklist Tool!
</asp:Content>
