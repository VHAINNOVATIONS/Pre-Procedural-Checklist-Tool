<%@ Page Title="Patient Lookup" Language="C#" MasterPageFile="~/MasterPage.master"
    AutoEventWireup="true" CodeFile="pl_patient_lookup.aspx.cs" Inherits="pl_patient_lookup" %>

<%@ MasterType VirtualPath="MasterPage.master" %>
<%@ Register TagPrefix="uc" TagName="PatientLookup" Src="~/app_ucPatientLookup.ascx" %>

<asp:Content ID="ctHead" runat="Server"
    ContentPlaceHolderID="cphHead">
</asp:Content>
<asp:Content ID="ctBody" runat="Server"
    ContentPlaceHolderID="cphBody">
    <asp:UpdatePanel ID="upPatientLookup" runat="server"
        RenderMode="Block"
        UpdateMode="Always">
        <ContentTemplate>
            <!--patient lookup user control-->
            <uc:PatientLookup ID="ucPatientLookup" runat="server"
                OnCollapse="OnFiltersCollapse"
                OnSearch="OnPatientSearch" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <h1 class="app_label1">
        Patient(s)
    </h1>
    <asp:UpdatePanel ID="upPatientGrid" runat="server"
        UpdateMode="Conditional">
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ucPatientLookup" EventName="Search" />
            <asp:AsyncPostBackTrigger ControlID="ucPatientLookup" EventName="Collapse" />
        </Triggers>
        <ContentTemplate>
            <asp:Panel ID="pnlPatients" runat="server"
                CssClass="app_panel"
                Height="198"
                ScrollBars="Vertical">
                <asp:GridView ID="gvPatients" runat="server"
                    AllowSorting="true"
                    DataKeyNames="patient_id"
                    EmptyDataText="Please search with one or more filters."
                    OnRowDataBound="OnRowDataBoundPat"
                    OnSelectedIndexChanged="OnSelIndexChangedPat"
                    OnSorting="OnSortingPat">
                    <Columns>
                        <asp:TemplateField AccessibleHeaderText="Last Name"
                            HeaderText="Last Name"
                            HeaderStyle-CssClass="gv_pleasewait"
                            ItemStyle-CssClass="gv_pleasewait"
                            SortExpression="LAST_NAME">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkSelect" runat="server"
                                    CommandName="Select"
                                    ForeColor="Blue"
                                    Width="389"
                                    CssClass="gv_truncated">
                                </asp:LinkButton>
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
            <div class="app_horizontal_spacer">
            </div>
            <asp:Button ID="btnLookup" runat="server"
                Enabled="false"
                PostBackUrl="~/sp_single_patient.aspx"
                Text="Lookup Patient" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
