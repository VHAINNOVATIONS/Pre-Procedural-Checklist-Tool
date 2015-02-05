<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ie_ucMapLabTest.ascx.cs"
    Inherits="ie_ucMapLabTest" %>
    
<asp:UpdatePanel ID="upucMapLabTest" runat="server"
    RenderMode="Block">
    <ContentTemplate>
        <!-- search fields -->
        <asp:Panel ID="pnlSearch" runat="server"
            DefaultButton="btnSearchOptions">
            <asp:Label ID="lblSearchOptions" runat="server"
                AccessKey="S"
                AssociatedControlID="txtSearchOptions"
                CssClass="app_label4"
                Text="<span class=access_key>S</span>earch Laboratory Test(s)">
            </asp:Label>
            <asp:TextBox ID="txtSearchOptions" runat="server"
                Width="300">
            </asp:TextBox>
            <asp:Button ID="btnSearchOptions" runat="server"
                OnClick="OnClickSearch"
                Text="Search" />
        </asp:Panel>
        <div class="app_horizontal_spacer">
        </div>
        
        <!-- search results -->
        <asp:Label ID="lblLabTests" runat="server"
            AccessKey="L"
            AssociatedControlID="gvLabTests"
            CssClass="app_label4"
            Text="<span class=access_key>L</span>aboratory Test(s)">
        </asp:Label>
        <div class="app_horizontal_spacer">
        </div>
        <asp:Panel ID="pnlLabTests" runat="server"
            CssClass="app_panel"
            Height="300"
            ScrollBars="Vertical">
            <asp:GridView ID="gvLabTests" runat="server"
                AllowSorting="true"
                DataKeyNames="lab_test_id"
                OnSelectedIndexChanged="OnSelIndexChangedLab"
                OnRowDataBound="OnRowDataBoundLab"
                OnSorting="OnSortingLab"
                Width="736">
                <Columns>
                    <asp:TemplateField AccessibleHeaderText="Laboratory"
                        HeaderStyle-CssClass="gv_pleasewait"
                        HeaderText="Laboratory"
                        ItemStyle-CssClass="gv_pleasewait"
                        SortExpression="lab_test_name">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkSelect" runat="server"
                                CommandName="Select"
                                ForeColor="Blue"
                                Width="200"
                                CssClass="gv_truncated">
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="Lo Ref"
                        HeaderStyle-CssClass="gv_pleasewait"
                        HeaderText="Lo Ref"
                        SortExpression="lab_test_loref">
                        <ItemTemplate>
                            <asp:Label ID="lblLoRef" runat="server"
                                CssClass="gv_truncated"
                                Width="50">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="Hi Ref"
                        HeaderStyle-CssClass="gv_pleasewait"
                        HeaderText="Hi Ref"
                        SortExpression="lab_test_hiref">
                        <ItemTemplate>
                            <asp:Label ID="lblHiRef" runat="server"
                                CssClass="gv_truncated"
                                Width="50">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="Ref Range"
                        HeaderStyle-CssClass="gv_pleasewait"
                        HeaderText="Ref Range"
                        SortExpression="lab_test_refrange">
                        <ItemTemplate>
                            <asp:Label ID="lblRefRange" runat="server"
                                CssClass="gv_truncated"
                                Width="50">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="Units"
                        HeaderStyle-CssClass="gv_pleasewait"
                        HeaderText="Units"
                        SortExpression="lab_test_units">
                        <ItemTemplate>
                            <asp:Label ID="lblUnits" runat="server"
                                CssClass="gv_truncated"
                                Width="50">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField AccessibleHeaderText="Description"
                        HeaderStyle-CssClass="gv_pleasewait"
                        HeaderText="Description"
                        SortExpression="lab_test_description">
                        <ItemTemplate>
                            <asp:Label ID="lblDescription" runat="server"
                                CssClass="gv_truncated"
                                Width="336">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </asp:Panel>
        <div class="app_horizontal_spacer">
        </div>
        <asp:Button ID="btnSelect" runat="server"
            OnClick="OnClickSelect"
            Text="Select" />
        <asp:Button ID="btnCancel" runat="server"
            OnClick="OnClickCancel"
            Text="Cancel" />
    </ContentTemplate>
</asp:UpdatePanel>
