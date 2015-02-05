<%@ Page Title="Checklist Editor" Language="C#" MasterPageFile="~/MasterPage.master"
    AutoEventWireup="true" CodeFile="ce_checklist_editor.aspx.cs" Inherits="ce_checklist_editor" %>
    
<%@ MasterType VirtualPath="MasterPage.master" %>
<%@ Register TagPrefix="uc" TagName="ChecklistSelector" Src="~/ce_ucChecklistSelector.ascx" %>
<%@ Register TagPrefix="uc" TagName="ChecklistEntry" Src="~/ce_ucChecklistEntry.ascx" %>
<%@ Register TagPrefix="uc" TagName="SaveAs" Src="~/ce_ucSaveAs.ascx" %>

<asp:Content ID="ctHead" runat="Server"
    ContentPlaceHolderID="cphHead">
</asp:Content>
<asp:Content ID="ctBody" runat="Server"
    ContentPlaceHolderID="cphBody">
    <h1 class="app_label1">
        Checklist Options
    </h1>
    <asp:UpdatePanel ID="upChecklist" runat="server"
        UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button ID="btnCLNew" runat="server"
                Text="New"
                OnClick="OnClickNew" />
            <asp:Button ID="btnCLLoad" runat="server"
                Text="Load"
                OnClick="OnClickLoad" />
            <asp:Button ID="btnCLSave" runat="server"
                Text="Save"
                OnClick="OnClickSave"
                Enabled="false" />
            <asp:Button ID="btnCLSaveAs" runat="server"
                Text="Copy Checklist"
                OnClick="OnClickSaveAs"
                Enabled="false" />
                
            <!-- checklist selector popup -->
            <asp:HiddenField ID="hfTargetOne" runat="server" />
            <asp:ModalPopupExtender ID="mpeChecklistSelect" runat="server"
                TargetControlID="hfTargetOne"
                PopupControlID="pnlChecklistSelect">
            </asp:ModalPopupExtender>
            <asp:Panel ID="pnlChecklistSelect" runat="server"
                CssClass="checklist_selector">
                <uc:ChecklistSelector ID="ucChecklistSelector" runat="server"
                    OnSelect="OnChecklistSelect"
                    Visible="false" />
            </asp:Panel>
                
            <!-- save as popup -->
            <asp:HiddenField ID="hfTargetTwo" runat="server" />
            <asp:ModalPopupExtender ID="mpeSaveAs" runat="server"
                TargetControlID="hfTargetTwo"
                PopupControlID="pnlSaveAs">
            </asp:ModalPopupExtender>
            <asp:Panel ID="pnlSaveAs" runat="server"
                CssClass="save_as">
                <uc:SaveAs ID="ucSaveAs" runat="server"
                    OnSaveAs="OnSaveAsChecklist"
                    Visible="false" />
            </asp:Panel>
                
            <uc:ChecklistEntry ID="ucChecklistEntry" runat="server" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
