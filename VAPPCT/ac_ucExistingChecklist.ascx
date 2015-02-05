<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ac_ucExistingChecklist.ascx.cs"
    Inherits="ac_ucExistingChecklist" %>
    
<div class="app_label4">
    The patient(s) listed below currently have the selected checklist open. Do you want to assign the checklist to the patient(s) again?
</div>
<h1 class="app_label1">
    Patient(s)
</h1>
<asp:UpdatePanel ID="upExistingCL" runat="server"
    RenderMode="Block">
    <ContentTemplate>
        <asp:Panel ID="pnlExistingCLgv" runat="server"
            CssClass="app_panel"
            Height="300"
            ScrollBars="Vertical">
            <asp:GridView ID="gvExistingCL" runat="server"
                AllowSorting="true"
                DataKeyNames="patient_id"
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
                                Width="235"
                                CssClass="gv_truncated" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="First Name"
                        HeaderText="First Name"
                        HeaderStyle-CssClass="gv_pleasewait"
                        SortExpression="FIRST_NAME">
                        <ItemTemplate>
                            <asp:Label ID="lblFirstName" runat="server"
                                Width="235"
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
                    <asp:TemplateField AccessibleHeaderText="Assignment Date"
                        HeaderText="Assignment Date"
                        HeaderStyle-CssClass="gv_pleasewait"
                        SortExpression="assignment_date">
                        <ItemTemplate>
                            <asp:Label ID="lblAssignmentDate" runat="server"
                                Width="145"
                                CssClass="gv_truncated">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="Procedure Date"
                        HeaderText="Procedure Date"
                        HeaderStyle-CssClass="gv_pleasewait"
                        SortExpression="procedure_date">
                        <ItemTemplate>
                            <asp:Label ID="lblProcedureDate" runat="server"
                                Width="145"
                                CssClass="gv_truncated">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
<!--spacer-->
<div class="app_horizontal_spacer">
</div>
<asp:Button ID="btnExistCLContinue" runat="server"
    Text="Yes"
    OnClick="btnExistCLContinue_Click" />
<asp:Button ID="btnExistCLCancel" runat="server"
    Text="No"
    OnClick="btnExistCLCancel_Click" />