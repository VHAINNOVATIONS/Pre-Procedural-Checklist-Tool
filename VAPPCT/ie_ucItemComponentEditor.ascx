<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ie_ucItemComponentEditor.ascx.cs"
    Inherits="ie_ucItemComponentEditor" %>
    
<asp:UpdatePanel ID="upItemComponent" runat="server"
    RenderMode="Block">
    <ContentTemplate>
        <h2 class="app_label2">
            Item Component(s)
            <asp:Button ID="btnAdd" runat="server"
                OnClick="OnClickAdd"
                Text="Add Component" />
        </h2>
        <div class="app_horizontal_spacer">
        </div>
        <asp:Panel ID="pnlGridView" runat="server"
            Height="185"
            CssClass="app_panel"
            ScrollBars="Vertical">
            <asp:GridView ID="gvItemComponent" runat="server"
                DataKeyNames="ITEM_COMPONENT_ID, IC_RANGE_ID, IC_STATE_ID"
                OnRowDataBound="OnRowDataBoundComp"
                ShowHeader="false">
                <RowStyle CssClass="gv_rowstyle_bw" />
                <Columns>
                    <asp:TemplateField ItemStyle-Width="685">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkActive" runat="server"
                                Text="Active"
                                Width="75" />
                            <asp:Label ID="lblComponentLabel" runat="server"
                                Text="Component Label"
                                AssociatedControlID="tbComponentLabel">
                            </asp:Label>
                            <asp:TextBox ID="tbComponentLabel" runat="server"
                                Width="200">
                            </asp:TextBox>
                            <asp:Label ID="lblSortOrder" runat="server"
                                Text="Sort Order"
                                AssociatedControlID="tbSortOrder">
                            </asp:Label>
                            <asp:TextBox ID="tbSortOrder" runat="server"
                                MaxLength="2"
                                Width="20">
                            </asp:TextBox>
                            <asp:FilteredTextBoxExtender ID="ftbSortOrder" runat="server"
                                FilterType="Numbers"
                                TargetControlID="tbSortOrder" />
                            <asp:Label ID="lblSelection" runat="server"
                                Visible="false">
                                <asp:Label ID="lblState" runat="server"
                                    AssociatedControlID="ddlState"
                                    style="margin-right: 5px;"
                                    Text="State">
                                </asp:Label>
                                <asp:DropDownList ID="ddlState" runat="server">
                                </asp:DropDownList>
                            </asp:Label>
                            <asp:Label ID="lblRange" runat="server"
                                Visible="false">
                                <asp:Label ID="lblUnits" runat="server"
                                    AssociatedControlID="tbUnits"
                                    style="margin-right: 5px;"
                                    Text="Units">
                                </asp:Label>
                                <asp:TextBox ID="tbUnits" runat="server"
                                    Width="125">
                                </asp:TextBox>
                                <div class="app_horizontal_spacer">
                                </div>
                                
                                <div style="margin-left: 30px; margin-bottom: 2px;">
                                    <asp:Label ID="lblLegalMin" runat="server"
                                        AssociatedControlID="tbLegalMin"
                                        style="display: inline-block; margin-left: 73px;"
                                        Text="Legal Min"
                                        Width="75">
                                    </asp:Label>
                                    <asp:Label ID="lblCriticalLow" runat="server"
                                        AssociatedControlID="tbCriticalLow"
                                        style="display: inline-block; margin-left: 23px;"
                                        Text="Critical Low"
                                        Width="75">
                                    </asp:Label>
                                    <asp:Label ID="lblLow" runat="server"
                                        AssociatedControlID="tbLow"
                                        style="display: inline-block;"
                                        Text="Low"
                                        Width="75">
                                    </asp:Label>
                                    <asp:Label ID="lblHigh" runat="server"
                                        AssociatedControlID="tbHigh"
                                        style="display: inline-block;"
                                        Text="High"
                                        Width="75">
                                    </asp:Label>
                                    <asp:Label ID="lblCriticalHigh" runat="server"
                                        AssociatedControlID="tbCriticalHigh"
                                        style="display: inline-block;"
                                        Text="Critical High"
                                        Width="75">
                                    </asp:Label>
                                    <asp:Label ID="lblLegalMax" runat="server"
                                        AssociatedControlID="tbLegalMax"
                                        style="display: inline-block; margin-left: 25px;"
                                        Text="Legal Max"
                                        Width="75">
                                    </asp:Label>
                                    <div class="app_horizontal_spacer">
                                    </div>
                                    
                                    <asp:TextBox ID="tbLegalMin" runat="server"
                                        style="margin-left: 73px;"
                                        Width="50">
                                    </asp:TextBox>
                                    <asp:FilteredTextBoxExtender ID="ftbLegalMin" runat="server"
                                        TargetControlID="tbLegalMin"
                                        FilterType="Custom, Numbers"
                                        ValidChars="." />
                                    <asp:TextBox ID="tbCriticalLow" runat="server"
                                        style="margin-left: 42px;"
                                        Width="50">
                                    </asp:TextBox>
                                    <asp:FilteredTextBoxExtender ID="ftbCriticalLow" runat="server"
                                        TargetControlID="tbCriticalLow"
                                        FilterType="Custom, Numbers"
                                        ValidChars="." />
                                    <asp:TextBox ID="tbLow" runat="server"
                                        style="margin-left: 19px;"
                                        Width="50">
                                    </asp:TextBox>
                                    <asp:FilteredTextBoxExtender ID="ftbLow" runat="server"
                                        TargetControlID="tbLow"
                                        FilterType="Custom, Numbers"
                                        ValidChars="." />
                                    <asp:TextBox ID="tbHigh" runat="server"
                                        style="margin-left: 19px;"
                                        Width="50">
                                    </asp:TextBox>
                                    <asp:FilteredTextBoxExtender ID="ftbHigh" runat="server"
                                        TargetControlID="tbHigh"
                                        FilterType="Custom, Numbers"
                                        ValidChars="." />
                                    <asp:TextBox ID="tbCriticalHigh" runat="server"
                                        style="margin-left: 19px;"
                                        Width="50">
                                    </asp:TextBox>
                                    <asp:FilteredTextBoxExtender ID="ftbCriticalHigh" runat="server"
                                        TargetControlID="tbCriticalHigh"
                                        FilterType="Custom, Numbers"
                                        ValidChars="." />
                                    <asp:TextBox ID="tbLegalMax" runat="server"
                                        style="margin-left: 44px;"
                                        Width="50">
                                    </asp:TextBox>
                                    <asp:FilteredTextBoxExtender ID="ftbLegalMax" runat="server"
                                        TargetControlID="tbLegalMax"
                                        FilterType="Custom, Numbers"
                                        ValidChars="." />
                                    <div class="app_horizontal_spacer">
                                    </div>
                                    
                                    <!-- range labels -->
                                    <asp:Label ID="lblScaleMin" runat="server"
                                        BackColor="Gray"
                                        Text="Invalid"
                                        Width="100"
                                        style="text-align:center;">
                                    </asp:Label>
                                    <asp:Label ID="lblScaleCritLow" runat="server"
                                        BackColor="DarkRed"
                                        Text="Critical Low"
                                        Width="100"
                                        style="text-align:center;">
                                    </asp:Label>
                                    <asp:Label ID="lblScaleLow" runat="server"
                                        BackColor="Red"
                                        Text="Low"
                                        Width="75"
                                        style="text-align:center;">
                                    </asp:Label>
                                    <asp:Label ID="lblScaleNorm" runat="server"
                                        BackColor="Green"
                                        Text="Normal"
                                        Width="75"
                                        style="text-align:center;">
                                    </asp:Label>
                                    <asp:Label ID="lblScaleHigh" runat="server"
                                        BackColor="Red"
                                        Text="High"
                                        Width="75"
                                        style="text-align:center;">
                                    </asp:Label>
                                    <asp:Label ID="lblScaleCritHigh" runat="server"
                                        BackColor="DarkRed"
                                        Text="Critical High"
                                        Width="100"
                                        style="text-align:center;">
                                    </asp:Label>
                                    <asp:Label ID="lblScaleMax" runat="server"
                                        BackColor="Gray"
                                        Text="Invalid"
                                        Width="100"
                                        style="text-align:center;">
                                    </asp:Label>
                                </div>
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>