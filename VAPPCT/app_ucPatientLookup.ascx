<%@ Control Language="C#" AutoEventWireup="true" CodeFile="app_ucPatientLookup.ascx.cs"
    Inherits="app_ucPatientLookup" %>
    
<%@ Register TagPrefix="uc" TagName="ChecklistSelector" Src="~/ce_ucChecklistSelector.ascx" %>

<h1 class="app_label1">
    <asp:Button ID="btnCollapseFilters" runat="server"
        OnClick="OnClickCollapseFilters"
        Text="-"
        Width="30" />
    Patient Filters
</h1>
<asp:UpdatePanel ID="upucPatientLookup" UpdateMode="Conditional"  runat="server"
    RenderMode="Block">
    <ContentTemplate>
        <asp:Panel ID="pnlPatientLookup" runat="server"
            DefaultButton="btnSearch">
            <div style="float: left;">
                <!-- procedure date-->
                <asp:CheckBox ID="chkFilterByEvent" runat="server"
                    Text="Enable <span class=access_key>P</span>rocedure Date Filter"
                    OnCheckedChanged="chkFilterByEvent_CheckedChanged"
                    AutoPostBack="true"
                    CssClass="app_label4"
                    AccessKey="P" />
                <div class="app_horizontal_spacer">
                </div>
                <asp:Label ID="lblFromDate" runat="server"
                    Text="<span class=access_key>F</span>rom Date"
                    AccessKey="F"
                    CssClass="app_label4"
                    AssociatedControlID="txtFromDate">
                </asp:Label>
                <asp:TextBox ID="txtFromDate" runat="server"
                    Width="70"
                    Enabled="false">
                </asp:TextBox>
                <asp:CalendarExtender ID="calFromDate" runat="server"
                    TargetControlID="txtFromDate"
                    EnableViewState="true"
                    DefaultView="Days"
                    Format="MM/dd/yyyy">
                </asp:CalendarExtender>
                <asp:Label ID="lblToDate" runat="server"
                    Text="<span class=access_key>T</span>o Date"
                    AssociatedControlID="txtToDate"
                    AccessKey="T"
                    CssClass="app_label4">
                </asp:Label>
                <asp:TextBox ID="txtToDate" runat="server"
                    Width="70"
                    Enabled="false">
                </asp:TextBox>
                <asp:CalendarExtender ID="calToDate" runat="server"
                    TargetControlID="txtToDate"
                    DefaultView="Days"
                    Format="MM/dd/yyyy" >
                </asp:CalendarExtender>
                <div class="app_horizontal_spacer">
                </div>
                
                <!--last name-->
                <asp:CheckBox ID="chkLastName" runat="server"
                    OnCheckedChanged="chkLastName_CheckedChanged"
                    CssClass="app_label4"
                    Text="Enable <span class=access_key>L</span>ast Name Filter"
                    AutoPostBack="true"
                    AccessKey="L" />
                <div class="app_horizontal_spacer">
                </div>
                <asp:Label ID="lblLastName" runat="server"
                    Text="Last Na<span class=access_key>m</span>e"
                    AssociatedControlID="txtLastName"
                    AccessKey="M"
                    CssClass="app_label4">
                </asp:Label>
                <asp:TextBox ID="txtLastName" runat="server"
                    Enabled="false">
                </asp:TextBox>
                <div class="app_horizontal_spacer">
                </div>
                
                <!--LSSN-->
                <asp:CheckBox ID="chkLSSN" runat="server"
                    CssClass="app_label4"
                    Text="Enable L<span class=access_key>S</span>SN Filter"
                    OnCheckedChanged="chkLSSN_CheckedChanged"
                    AutoPostBack="true"
                    AccessKey="S" />
                <div class="app_horizontal_spacer">
                </div>
                <asp:Label ID="lblLSSN" runat="server"
                    AssociatedControlID="txtLSSN"
                    CssClass="app_label4"
                    AccessKey="N"
                    Text="LSS<span class=access_key>N</span>">
                </asp:Label>
                <asp:TextBox ID="txtLSSN" runat="server"
                    MaxLength="5"
                    Width="50"
                    Enabled="false">
                </asp:TextBox>
                <asp:MaskedEditExtender ID="meeLSSN" runat="server"
                    TargetControlID="txtLSSN"
                    Mask="L9999">
                </asp:MaskedEditExtender>
                <div class="app_horizontal_spacer">
                </div>
                
                <!--checklist-->
                <asp:CheckBox ID="chkChecklist" runat="server"
                    Text="Enable <span class=access_key>C</span>hecklist Filter"
                    OnCheckedChanged="chkChecklist_CheckedChanged"
                    AutoPostBack="true"
                    AccessKey="C"
                    CssClass="app_label4" />
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
                    OnClick="btnSelectChecklist_Click"
                    Enabled="false" />
                <div class="app_horizontal_spacer">
                </div>
                
                <!--checklist status-->
                <asp:CheckBox ID="chkChecklistStatus" runat="server"
                    Text="Enable C<span class=access_key>h</span>ecklist Status Filter"
                    CssClass="app_label4"
                    OnCheckedChanged="chkChecklistStatus_CheckedChanged"
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
    
            </div>
            <div style="float: right;">
                <asp:Label ID="lblAdditionalFilters" runat="server"
                    Text="Additional Filter Categories"
                    CssClass="app_label4"
                    AssociatedControlID="rblOptions">
                </asp:Label>
                <asp:RadioButtonList ID="rblOptions" runat="server"
                    AutoPostBack="true"
                    CssClass="app_label4"
                    OnSelectedIndexChanged="OnSelectedIndexChangedOptions"
                    RepeatDirection="Horizontal"
                    Width="500">
                    <asp:ListItem  Value="0" Selected="True">None</asp:ListItem>
                    <asp:ListItem Value="1">Providers</asp:ListItem>
                    <asp:ListItem Value="2">Teams</asp:ListItem>
                    <asp:ListItem Value="3">Specialties</asp:ListItem>
                    <asp:ListItem Value="4" Enabled="false">Clinics</asp:ListItem>
                    <asp:ListItem Value="5">Wards</asp:ListItem>
                </asp:RadioButtonList>
                <div class="app_horizontal_spacer">
                </div>
                
                <!--text filter-->
                <asp:Label ID="lblSearchOptions" runat="server"
                    Text="Search Category <span class=access_key>R</span>esult(s)"
                    CssClass="app_label4"
                    AccessKey="R"
                    AssociatedControlID="txtSearchOptions">
                </asp:Label>
                <asp:TextBox ID="txtSearchOptions" runat="server"
                    Width="255">
                </asp:TextBox>
                <asp:Button ID="btnSearchOptions" runat="server"
                    Text="Search"
                    OnClick="btnSearchOptions_Click" />
                <!--spacer-->
                <div class="app_horizontal_spacer">
                </div>
                
                <!--appointment dates-->
                <asp:Label ID="lblApptFor" runat="server"
                    Text="APPTs for:"
                    CssClass="app_label4">
                </asp:Label>
                <asp:TextBox ID="txtApptFromDate" runat="server"
                    ReadOnly="false"
                    Width="70">
                </asp:TextBox>
                <asp:CalendarExtender ID="calApptFromDate" runat="server"
                    TargetControlID="txtApptFromDate"
                    DefaultView="Days"
                    Format="MM/dd/yyyy">
                </asp:CalendarExtender>
                <asp:Label ID="lblApptTo" runat="server"
                    Text="To:"
                    CssClass="app_label4">
                </asp:Label>
                <asp:TextBox ID="txtApptToDate" runat="server"
                    ReadOnly="false"
                    Width="70">
                </asp:TextBox>
                <asp:CalendarExtender ID="calApptToDate" runat="server"
                    TargetControlID="txtApptToDate"
                    DefaultView="Days"
                    Format="MM/dd/yyyy">
                </asp:CalendarExtender>
                <asp:Label ID="lblResults" runat="server"
                    Text="Cat<span class=access_key>e</span>gory Result(s)"
                    CssClass="app_label4"
                    AssociatedControlID="lbOptions">
                </asp:Label>
                <!--spacer-->
                <div class="app_horizontal_spacer">
                </div>
                <asp:ListBox ID="lbOptions" runat="server"
                    AccessKey="E"
                    Height="160"
                    Width="500"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="lbOptions_SelectedIndexChanged">
                </asp:ListBox>
            </div>
            <!--spacer-->
            <div class="app_horizontal_spacer">
            </div>
            <asp:Button ID="btnSearch" runat="server"
                Text="Search"
                OnClick="btnSearch_Click" />
            <asp:Button ID="btnMyPatients" runat="server"
                Text="My Patients"
                OnClick="btnMyPatients_Click" />
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
    </ContentTemplate>
</asp:UpdatePanel>
