<%@ Page Title="Assign Checklist" Language="C#" MasterPageFile="~/MasterPage.master"
    AutoEventWireup="true" CodeFile="ac_assign_checklist.aspx.cs" Inherits="ac_assign_checklist" %>

<%@ MasterType VirtualPath="MasterPage.master" %>
<%@ Register TagPrefix="uc" TagName="PatientLookup" Src="~/app_ucPatientLookup.ascx" %>
<%@ Register TagPrefix="uc" TagName="ChecklistSelector" Src="~/ce_ucChecklistSelector.ascx" %>
<%@ Register TagPrefix="uc" TagName="ExistingChecklist" Src="~/ac_ucExistingChecklist.ascx" %>
<%@ Register TagPrefix="uc" TagName="CancelProcessing" Src="~/app_ucCancelProcessing.ascx" %>

<asp:Content ID="ctHead" runat="Server"
    ContentPlaceHolderID="cphHead">
</asp:Content>
<asp:Content ID="ctBody" runat="Server"
    ContentPlaceHolderID="cphBody">
    <h1 class="app_label1">
        Checklist
    </h1>
    <asp:UpdatePanel ID="upAssignChecklist" runat="server"
        RenderMode="Block"
        UpdateMode="always">
        <ContentTemplate>
            <asp:Label ID="lblChecklistLabel" runat="server"
                CssClass="app_label4"
                AssociatedControlID="tbChecklist"
                AccessKey="A"
                Text="Checklist To <span class=access_key>A</span>ssign">
            </asp:Label>
            <asp:TextBox ID="tbChecklist" runat="server"
                Width="200"
                ReadOnly="true" />
            <asp:Button ID="btnSelectChecklist" runat="server"
                OnClick="OnSelectChecklist"
                Text="Select" />
            <!-- checklist selector popup -->
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
            <uc:PatientLookup ID="ucPatientLookup" runat="server"
                OnCollapse="OnFiltersCollapse"
                OnSearch="OnPatientSearch" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:UpdatePanel ID="upPatientGrid" runat="server"
        RenderMode="Block"
        UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ucPatientLookup" EventName="Search" />
            <asp:AsyncPostBackTrigger ControlID="ucPatientLookup" EventName="Collapse" />
            <asp:AsyncPostBackTrigger ControlID="btnProcess" EventName="Click" />
        </Triggers>
        <ContentTemplate>
            <h1 class="app_label1">
                Patient(s)
                <asp:Button ID="btnCheckAll" runat="server"
                    OnClick="OnClickCheckAll"
                    Text="Check All" />
            </h1>
            <asp:Panel ID="pnlPatients" runat="server"
                CssClass="app_panel"
                Height="135"
                ScrollBars="Vertical">
                <asp:GridView ID="gvPatients" runat="server"
                    AllowSorting="true"
                    DataKeyNames="PATIENT_ID"
                    EmptyDataText="Please search with one or more filters."
                    OnRowDataBound="OnRowDataBoundPat"
                    OnSorting="OnSortingPat">
                    <Columns>
                        <asp:TemplateField AccessibleHeaderText="Last Name"
                            HeaderText="Last Name"
                            HeaderStyle-CssClass="gv_pleasewait"
                            ItemStyle-CssClass="gv_pleasewait"
                            SortExpression="LAST_NAME">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelect" runat="server"
                                    Width="389"
                                    CssClass="gv_truncated" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField AccessibleHeaderText="First Name"
                            HeaderText="First Name"
                            HeaderStyle-CssClass="gv_pleasewait"
                            SortExpression="FIRST_NAME">
                            <ItemTemplate>
                                <asp:Label ID="lblFirstName" runat="server"
                                    Width="389"
                                    CssClass="gv_truncated">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField AccessibleHeaderText="MI"
                            HeaderText="MI"
                            HeaderStyle-CssClass="gv_pleasewait"
                            SortExpression="MIDDLE_INITIAL">
                            <ItemTemplate>
                                <asp:Label ID="lblMI" runat="server"
                                    Width="20"
                                    CssClass="gv_truncated">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField AccessibleHeaderText="Last 4"
                            HeaderText="Last 4"
                            HeaderStyle-CssClass="gv_pleasewait"
                            SortExpression="SSN_LAST_4">
                            <ItemTemplate>
                                <asp:Label ID="lblLastFour" runat="server"
                                    Width="55"
                                    CssClass="gv_truncated">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField AccessibleHeaderText="Age"
                            HeaderText="Age"
                            HeaderStyle-CssClass="gv_pleasewait"
                            SortExpression="PATIENT_AGE">
                            <ItemTemplate>
                                <asp:Label ID="lblAge" runat="server"
                                    Width="40"
                                    CssClass="gv_truncated">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField AccessibleHeaderText="Sex"
                            HeaderText="Sex"
                            HeaderStyle-CssClass="gv_pleasewait"
                            SortExpression="SEX_LABEL">
                            <ItemTemplate>
                                <asp:Label ID="lblSex" runat="server"
                                    Width="29"
                                    CssClass="gv_truncated">
                                </asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
    <div class="app_horizontal_spacer">
    </div>
    <asp:UpdatePanel ID="upProcess" runat="server"
        UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnAssignChecklist" EventName="Click" />
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
    <asp:UpdatePanel ID="upAssign" runat="server"
        UpdateMode="Conditional">
        <ContentTemplate>
            <asp:Button ID="btnAssignChecklist" runat="server"
                OnClick="OnAssignChecklist" 
                Text="Assign Checklist" />
                
            <!-- existing checklist popup -->
            <asp:HiddenField ID="hfTargetExisting" runat="server" />
            <asp:ModalPopupExtender ID="mpeExistingChecklist" runat="server"
                TargetControlID="hfTargetExisting"
                PopupControlID="pnlExistingChecklist">
            </asp:ModalPopupExtender>
            <asp:Panel ID="pnlExistingChecklist" runat="server"
                CssClass="existing_multi">
                <uc:ExistingChecklist ID="ucExistingChecklist" runat="server"
                    OnContinue="OnContinueExisting"
                    Visible="false" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
