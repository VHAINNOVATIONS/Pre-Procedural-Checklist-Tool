<%@ Control Language="C#" AutoEventWireup="true" CodeFile="sp_ucPatientChecklist.ascx.cs"
    Inherits="sp_ucPatientChecklist" %>
    
<%@ Register TagPrefix="uc" TagName="Timer" Src="~/app_ucTimer.ascx" %>
<%@ Register TagPrefix="uc" TagName="PatCLItems" Src="~/sp_ucPatientChecklistItems.ascx" %>
<%@ Register TagPrefix="uc" TagName="TimePicker" Src="~/app_ucTimePicker.ascx" %>
<%@ Register TagPrefix="uc" TagName="ChecklistSelector" Src="~/ce_ucChecklistSelector.ascx" %>
<%@ Register TagPrefix="uc" TagName="TIUNote" Src="~/sp_ucTIUNote.ascx" %>
<%@ Register TagPrefix="uc" TagName="ExistingChecklist" Src="~/sp_ucExistingChecklist.ascx" %>
<%@ Register TagPrefix="uc" TagName="UpdateChecklistVersion" Src="~/sp_ucUpdateChecklistVersion.ascx" %>


<asp:UpdatePanel ID="upSinglePatient" runat="server"
    RenderMode="Block"
    UpdateMode="Conditional">
    <ContentTemplate>
        <div style="float: left;">
            <h1 class="app_label1">
                Patient Checklist Options
            </h1>
            <asp:Button ID="btnSaveCL" runat="server"
                OnClick="OnClickSaveChecklist"
                Text="Save" />
            <asp:Button ID="btnAssignCL" runat="server"
                OnClick="OnClickAssignChecklist"
                Text="Assign New Checklist" />
            <asp:Button ID="btnTIU" runat="server"
                Enabled="false"
                OnClick="OnTIUNote"
                Text="TIU Note" />
        </div>
        <div style="float: right;">
            <uc:Timer ID="ucTimer" runat="server"
                OnRefresh="OnRefresh" />
        </div>
        <div style="clear:both; float:left;">
            <h1 class="app_label1">
                <asp:Button ID="btnCollapseFields" runat="server"
                    OnClick="OnClickCollapseFields"
                    Text="-"
                    Width="30" />
                <span id="sPatientBlurb" runat="server"></span>
            </h1>
            <asp:Panel ID="pnlFields" runat="server"
                CssClass="patient_checklist_fields">
                <div class="patient_checklist_fields_left">
                    <asp:Label ID="lblPatChecklist" runat="server"
                        AccessKey="C"
                        AssociatedControlID="ddlPatChecklist"
                        CssClass="app_label4"
                        Text="Patient&nbsp;<span class=access_key>C</span>hecklist">
                    </asp:Label>
                </div>
                <div class="patient_checklist_fields_right" >
                    <asp:DropDownList ID="ddlPatChecklist" runat="server"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="OnSelChangeChecklist"
                        Width="250" />
                    <asp:Label ID="lblVersion" runat="server" Text="Label">
                    </asp:Label>
                    <asp:Button ID="btnUpdateCLVersion" runat="server"
                        Enabled="false"
                        OnClick="OnUpdateCLVersion"
                        Text="Apply New Version" />
                </div>
                <div class="app_horizontal_spacer">
                </div>
                <div class="patient_checklist_fields_left">
                    <asp:Label ID="lblChecklistState" runat="server"
                        AccessKey="S"
                        AssociatedControlID="ddlChecklistState"
                        CssClass="app_label4"
                        Text="Checklist&nbsp;<span class=access_key>S</span>tate">
                    </asp:Label>
                </div>
                <div class="patient_checklist_fields_right">
                    <asp:DropDownList ID="ddlChecklistState" runat="server"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="OnSelChangeChecklistState" />
                </div>
                <div class="app_horizontal_spacer">
                </div>
                
                <div class="patient_checklist_fields_left">
                    <asp:Label ID="lblProcedureDate" runat="server"
                        AccessKey="D"
                        AssociatedControlID="tbProcedureDate"
                        CssClass="app_label4"
                        Text="Procedure&nbsp;<span class=access_key>D</span>ate" />
                </div>
                <div class="patient_checklist_fields_right">
                    <asp:TextBox ID="tbProcedureDate" runat="server"
                        AutoPostBack="true"
                        OnTextChanged="OnProcedureDateChanged"
                        Width="70" />
                    <uc:TimePicker ID="ucProcedureTime" runat="server" />
                    <asp:CalendarExtender ID="calProcedureDate" runat="server"
                        DefaultView="Days"
                        Format="MM/dd/yyyy"
                        TargetControlID="tbProcedureDate" />
                </div>
            </asp:Panel>
        </div>
        <div class="app_horizontal_spacer">
        </div>
        
        <asp:HiddenField ID="hfTargetOne" runat="server" />
        <asp:ModalPopupExtender ID="mpeChecklistSelector" runat="server"
            PopupControlID="pnlChecklistSelect"
            TargetControlID="hfTargetOne">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlChecklistSelect" runat="server"
            CssClass="checklist_selector">
            <uc:ChecklistSelector ID="ucChecklistSelector" runat="server"
                OnSelect="OnChecklistSelect"
                Visible="false" />
        </asp:Panel>
        
        <asp:HiddenField ID="hfTargetTwo" runat="server" />
        <asp:ModalPopupExtender ID="mpeTIU" runat="server"
            PopupControlID="pnlTIU"
            TargetControlID="hfTargetTwo">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlTIU" runat="server"
            CssClass="tiu_note">
            <uc:TIUNote ID="ucTIUNote" runat="server"
                OnNoteSaved="OnNoteSaved"
                Visible="false" />
        </asp:Panel>
        
        <asp:HiddenField ID="hfUpdateChecklistVersion" runat="server" />
        <asp:ModalPopupExtender ID="mpeUpdateChecklistVersion" runat="server"
            PopupControlID="pnlUpdateChecklistVersion"
            TargetControlID="hfUpdateChecklistVersion">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlUpdateChecklistVersion" runat="server"
            CssClass="tiu_note">
            <uc:UpdateChecklistVersion ID="ucUpdateChecklistVersion" 
                runat="server"
                OnUpdateVersion="OnUpdateCLVersion"
                Visible="false" />
        </asp:Panel>
                
        <asp:HiddenField ID="hfTargetThree" runat="server" />
        <asp:ModalPopupExtender ID="mpeExisting" runat="server"
            PopupControlID="pnlExisting"
            TargetControlID="hfTargetThree">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlExisting" runat="server"
            CssClass="existing_single">
            <uc:ExistingChecklist ID="ucExistingChecklist" runat="server"
                OnContinue="OnContinueExisting"
                Visible="false" />
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>

<asp:UpdatePanel ID="upPatientCLItems" runat="server"
    RenderMode="Block"
    UpdateMode="Conditional">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="ucTimer" EventName="Refresh" />
        <asp:AsyncPostBackTrigger ControlID="ddlChecklistState" EventName="SelectedIndexChanged" />
        <asp:AsyncPostBackTrigger ControlID="ddlPatChecklist" EventName="SelectedIndexChanged" />
        <asp:AsyncPostBackTrigger ControlID="ucChecklistSelector" EventName="Select" />
    </Triggers>
    <ContentTemplate>
        <uc:PatCLItems ID="ucPatCLItems" runat="server" />
    </ContentTemplate>
</asp:UpdatePanel>
