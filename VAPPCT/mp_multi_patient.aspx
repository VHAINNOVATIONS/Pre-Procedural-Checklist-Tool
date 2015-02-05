<%@ Page Title="Multi Patient" Language="C#" MasterPageFile="~/MasterPage.master"
    AutoEventWireup="true" CodeFile="mp_multi_patient.aspx.cs" Inherits="mp_multi_patient" %>

<%@ MasterType VirtualPath="MasterPage.master" %>
<%@ Register TagPrefix="uc" TagName="ChecklistSelector" Src="~/ce_ucChecklistSelector.ascx" %>
<%@ Register TagPrefix="uc" TagName="Timer" Src="~/app_ucTimer.ascx" %>
<%@ Register TagPrefix="uc" TagName="SinglePatientPopup" Src="~/sp_ucSinglePatientPopup.ascx" %>
<%@ Register TagPrefix="uc" TagName="CancelProcessing" Src="~/app_ucCancelProcessing.ascx" %>
<%@ Register TagPrefix="uc" TagName="UpdateChecklistVersion" Src="~/mp_ucUpdateChecklistVersion.ascx" %>
    
<asp:Content ID="ctHead" runat="server"
    ContentPlaceHolderID="cphHead">
</asp:Content>
<asp:Content ID="ctBody" runat="server"
    ContentPlaceHolderID="cphBody">
    <div style="float: left;">
        <h1 class="app_label1">
            <asp:Button ID="btnCollapseFilters" runat="server"
                OnClick="OnClickCollapseFilters"
                Text="-"
                Width="30" />
            Patient Checklist Filters
        </h1>
        <asp:UpdatePanel ID="upLookup" runat="server"
            UpdateMode="Conditional">
            <ContentTemplate>
                <asp:Panel ID="pnlFilters" runat="server"
                    DefaultButton="btnSearch">
                    <div style="float: left;">
                        <div class="app_horizontal_spacer">
                        </div>
                        <!-- procedure date -->
                        <asp:CheckBox ID="chkFilterByEvent" runat="server"
                            Text="Enable <span class=access_key>P</span>rocedure Date Filter"
                            OnCheckedChanged="OnCheckChangedEvent"
                            AutoPostBack="true"
                            CssClass="app_label4"
                            AccessKey="P" />
                        <div class="app_horizontal_spacer">
                        </div>
                        <asp:Label ID="lblFromDate" runat="server"
                            Text="<span class=access_key>F</span>rom Date"
                            CssClass="app_label4"
                            AccessKey="F"
                            AssociatedControlID="txtFromDate">
                        </asp:Label>
                        <asp:TextBox ID="txtFromDate" runat="server"
                            Width="70"
                            Enabled="false">
                        </asp:TextBox>
                        <asp:CalendarExtender ID="calFromDate" runat="server"
                            TargetControlID="txtFromDate"
                            DefaultView="Days"
                            Format="MM/dd/yyyy" >
                        </asp:CalendarExtender>
                        <asp:Label ID="lblToDate" runat="server"
                            Text="<span class=access_key>T</span>o Date"
                            CssClass="app_label4"
                            AccessKey="T"
                            AssociatedControlID="txtToDate">
                        </asp:Label>
                        <asp:TextBox ID="txtToDate" runat="server"
                            Width="70"
                            Enabled="false">
                        </asp:TextBox>
                        <asp:CalendarExtender ID="calToDate" runat="server"
                            TargetControlID="txtToDate"
                            DefaultView="Days"
                            Format="MM/dd/yyyy">
                        </asp:CalendarExtender>
                        <div class="app_horizontal_spacer">
                        </div>
                        
                        <!--checklist-->
                        <asp:CheckBox ID="chkChecklist" runat="server"
                            CssClass="app_label4"
                            Text="Enable <span class=access_key>C</span>hecklist Filter"
                            OnCheckedChanged="OnCheckChangedChecklist"
                            AutoPostBack="true"
                            AccessKey="C" />
                        <div class="app_horizontal_spacer">
                        </div>
                        <asp:Label ID="lblChecklist" runat="server"
                            CssClass="app_label4"
                            AssociatedControlID="txtChecklist"
                            AccessKey="K"
                            Text="Chec<span class=access_key>k</span>list">
                        </asp:Label>
                        <asp:TextBox ID="txtChecklist" runat="server"
                            ReadOnly="true"
                            Width="200"
                            Enabled="false">
                        </asp:TextBox>
                        <asp:Button ID="btnSelectChecklist" runat="server"
                            Text="Select"
                            OnClick="OnClickChecklist"
                            Enabled="false" />
                        <div class="app_horizontal_spacer">
                        </div>
                        
                        <!--checklist status-->
                        <asp:CheckBox ID="chkChecklistStatus" runat="server"
                            Text="Enable C<span class=access_key>h</span>ecklist Status Filter" 
                            CssClass="app_label4"
                            OnCheckedChanged="OnCheckChangedCLStatus"
                            AutoPostBack="true"
                            AccessKey="H" />
                        <div class="app_horizontal_spacer">
                        </div>
                        <asp:Label ID="lblChecklistStatus" runat="server"
                            CssClass="app_label4"
                            AssociatedControlID="ddlChecklistStatus"
                            AccessKey="U"
                            Text="Checklist Stat<span class=access_key>u</span>s">
                        </asp:Label>
                        <asp:DropDownList ID="ddlChecklistStatus" runat="server"
                            Enabled="false">
                        </asp:DropDownList>
                        <div class="app_horizontal_spacer">
                        </div>
                        
    <div class="app_horizontal_spacer">
    </div>            
    <!-- service -->
    <asp:CheckBox ID="chkFilterByCLService" runat="server"
        AccessKey="V"
        AutoPostBack="true"
        CssClass="app_label4"
        OnCheckedChanged="OnCheckedChangedCLService"
        Text="Enable Ser<span class=access_key>v</span>ice Filter" />
    <div class="app_horizontal_spacer">
    </div>
    <asp:Label ID="lblService" runat="server"
        AccessKey="I"
        AssociatedControlID="ddlFilterByService"
        CssClass="app_label4"
        Text="Serv<span class=access_key>i</span>ce">
    </asp:Label>
    <asp:DropDownList Enabled="false" ID="ddlFilterByService" runat="server">
    </asp:DropDownList>
    <div class="app_horizontal_spacer">
    </div>

                        <asp:Button ID="btnSearch" runat="server"
                            Text="Search"
                            OnClick="OnClickSearch" />&nbsp;
                            
            <asp:Button ID="btnUpdateCLVersion" runat="server"
            Enabled="false"
            OnClick="OnUpdateCLVersion"
            Text="Apply New Version" />
            
                    </div>
                </asp:Panel>
                
                <!--checklist selector popup-->
                <asp:HiddenField ID="hfTargetOne" runat="server" />
                <asp:ModalPopupExtender ID="mpeChecklistSelector" runat="server"
                    TargetControlID="hfTargetOne"
                    PopupControlID="pnlChecklistSelect">
                </asp:ModalPopupExtender>
                <asp:Panel ID="pnlChecklistSelect" runat="server"
                    CssClass="checklist_selector">
                    <uc:ChecklistSelector ID="ucChecklistSelector" runat="server"
                        OnSelect="OnChecklistSelect"
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
            
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div style="float: right;">
        <uc:Timer ID="ucTimer" runat="server"
            OnRefresh="OnRefresh" />
    </div>
    <h1 class="app_label1"
        style="clear:both;">
        Patient(s)&nbsp;
        
    </h1>
    <asp:UpdatePanel ID="upMultiPatient" runat="server"
        UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ucTimer" EventName="Refresh" />
            <asp:AsyncPostBackTrigger ControlID="btnProcess" EventName="Click" />
        </Triggers>
        <ContentTemplate>
        <asp:Panel ID="pnlMultiPatientView" runat="server"
            CssClass="app_panel"
            Height="295"
            ScrollBars="Both"
            Width="960">
                <asp:UpdatePanel ID="upGridView" runat="server"
                    UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:GridView ID="gvMultiPatientView" runat="server"
                            AllowSorting="true"
                            AutoGenerateColumns="false"
                            DataKeyNames="patient_id,checklist_id,checklist_label,pat_cl_id"
                            EmptyDataText="Please search with zero or more filters."
                            OnPreRender="OnPreRenderPatients"
                            OnRowDataBound="OnRowDataBoundPatient"
                            OnSelectedIndexChanged="OnSelIndexChangedPat"
                            OnSorting="OnSortingPatients">
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="app_horizontal_spacer">
    </div>
    <asp:UpdatePanel ID="upProcess" runat="server"
        UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ucTimer" EventName="Refresh" />
        </Triggers>
        <ContentTemplate>
            <asp:Button ID="btnProcess" runat="server"
                OnClick="OnClickProcess"
                style="display:none;" />
                
            <!-- cancel processing popup -->
            <asp:HiddenField ID="hfTargetCancel" runat="server" />
            <asp:ModalPopupExtender ID="mpeCancelProcessing" runat="server"
                TargetControlID="hfTargetCancel"
                PopupControlID="pnlCancelProcessing">
            </asp:ModalPopupExtender>
            <asp:Panel ID="pnlCancelProcessing" runat="server"
                CssClass="cancel_processing">
                <uc:CancelProcessing ID="ucCancelProcessing" runat="server"
                    OnCancel="OnCancelProcessing"
                    Visible="false" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upSPEdit" runat="server"
        UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button ID="btnSPEdit" runat="server"
                Text="Edit"
                OnClick="OnClickEdit" />
                
            <!--Single Patient Editor popup-->
            <asp:HiddenField ID="hfTargetSP" runat="server" />
            <asp:ModalPopupExtender ID="mpeSinglePatientEditor" runat="server"
                PopupControlID="pnlSinglePatientSelector"
                TargetControlID="hfTargetSP">
            </asp:ModalPopupExtender>
            <asp:Panel ID="pnlSinglePatientSelector" runat="server"
                CssClass="single_patient_popup">
                <uc:SinglePatientPopup ID="ucSinglePatientPopup" runat="server"
                    Visible="false" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
