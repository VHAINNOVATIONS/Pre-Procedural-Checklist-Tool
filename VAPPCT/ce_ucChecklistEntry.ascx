<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ce_ucChecklistEntry.ascx.cs"
    Inherits="ce_ucChecklistEntry" %>

<%@ Register TagPrefix="uc" TagName="ItemEntry" Src="~/ce_ucItemEntry.ascx" %>
<%@ Register TagPrefix="uc" TagName="MapNoteTitle" Src="~/ie_ucMapNoteTitle.ascx" %>

<asp:UpdatePanel ID="upChecklist" runat="server"
    UpdateMode="Conditional">
    <ContentTemplate>
        <h1 class="app_label1"
            style="clear:both;">
            <asp:Button ID="btnCollapseFields" runat="server"
                OnClick="OnClickCollapseFields"
                Text="-"
                Width="30" />
            Checklist Fields
        </h1>
        <asp:Panel ID="pnlChecklistFields" runat="server"
            CssClass="checklist_fields">
            <!-- active -->
            <div class="checklist_fields_left">
            </div>
            <div class="checklist_fields_right">
                <asp:CheckBox ID="chkActive" runat="server"
                    Text="<span class=access_key>A</span>ctive"
                    CssClass="app_label4"
                    AccessKey="A"
                    Enabled="false" />
            </div>
            <div class="app_horizontal_spacer">
            </div>
            
            <!-- label -->
            <div class="checklist_fields_left">
                <asp:Label ID="lblCLLabel" runat="server"
                    Text="Chec<span class=access_key>k</span>List Label"
                    CssClass="app_label4"
                    AccessKey="K"
                    AssociatedControlID="txtCLLabel">
                </asp:Label>
            </div>
            <div class="checklist_fields_right">
                <asp:TextBox ID="txtCLLabel" runat="server"
                    Enabled="false"
                    MaxLength="255"
                    Width="200">
                </asp:TextBox>
            </div>
            <div class="app_horizontal_spacer">
            </div>
            
            <!-- service -->
            <div class="checklist_fields_left">
                <asp:Label ID="lblCLService" runat="server"
                    Text="Serv<span class=access_key>i</span>ce"
                    CssClass="app_label4"
                    AccessKey="I"
                    AssociatedControlID="ddlCLService">
                </asp:Label>
            </div>
            <div class="checklist_fields_right">
                <asp:DropDownList ID="ddlCLService" runat="server"
                    Enabled="false">
                </asp:DropDownList>
            </div>
            <div class="app_horizontal_spacer">
            </div>
            
            <!-- description -->
            <div class="checklist_fields_left">
                <asp:Label ID="lblCLDesc" runat="server"
                    Text="<span class=access_key>D</span>escription"
                    CssClass="app_label4"
                    AccessKey="D" 
                    AssociatedControlID="txtCLDesc">
                </asp:Label>
            </div>
            <div class="checklist_fields_right">
                <div style="float:left;">
                    <asp:TextBox ID="txtCLDesc" runat="server"
                        TextMode="MultiLine"
                        Width="450"
                        Rows="3"
                        Style="font-family: Verdana, Arial;"
                        Enabled="false">
                    </asp:TextBox>
                </div>
            </div>
            <div class="app_horizontal_spacer">
            </div>
            
            <!-- note title -->
            <div class="checklist_fields_left">
                <asp:Label ID="lblNoteTitle" runat="server"
                    CssClass="app_label4"
                    Text="Note <span class=access_key>T</span>itle"
                    AccessKey="T"
                    AssociatedControlID="txtCLNoteTitle">
                </asp:Label>
            </div>
            <div class="checklist_fields_right">
                <asp:TextBox ID="txtCLNoteTitle" runat="server"
                    Width="200"
                    ReadOnly="true"
                    Enabled="false">
                </asp:TextBox>
                <asp:Button ID="btnMap" runat="server"
                    Text="Map To Note Title"
                    OnClick="OnClickMap"
                    Enabled="false" />
            </div>
            <div class="app_horizontal_spacer">
            </div>
            
            <!-- note title mapping popup -->
            <asp:HiddenField ID="hfMapTarget" runat="server">
            </asp:HiddenField>
            <asp:ModalPopupExtender ID="mpeMap" runat="server"
                TargetControlID="hfMapTarget"
                PopupControlID="pnlMap">
            </asp:ModalPopupExtender>
            <asp:Panel ID="pnlMap" runat="server"
                CssClass="map_popup">
                <uc:MapNoteTitle ID="ucMapNoteTitle" runat="server"
                    OnSelectNoteTitle="OnSelectNoteTitle"
                    Visible="false" />
            </asp:Panel>
            
            <!-- clinic -->
            <div class="checklist_fields_left">
                <asp:Label ID="lblSelectClinic" runat="server"
                    Text="Note <span class=access_key>C</span>linic"
                    CssClass="app_label4"
                    AccessKey="C"
                    AssociatedControlID="ddlClinics">
                </asp:Label>
            </div>
            <div class="checklist_fields_right">
                <asp:DropDownList ID="ddlClinics" runat="server"
                    Enabled="false">
                </asp:DropDownList>
            </div>
            <div class="app_horizontal_spacer">
            </div>
            
            <!-- viewable by -->
            <div class="checklist_fields_left">
                <asp:Label ID="lblViewable" runat="server"
                    Text="Vie<span class=access_key>w</span>able By"
                    CssClass="app_label4"
                    AssociatedControlID="cblViewable">
                </asp:Label>
            </div>
            <div class="checklist_fields_right">
                <div style="float:left;">
                    <asp:CheckBoxList ID="cblViewable" runat="server"
                        CssClass="app_label4"
                        AccessKey="W"
                        DataValueField="USER_ROLE_ID"
                        DataTextField="USER_ROLE_LABEL"
                        RepeatDirection="Horizontal"
                        AutoPostBack="true" 
                        Enabled="false" onselectedindexchanged="cblViewable_SelectedIndexChanged">
                    </asp:CheckBoxList>
                </div>
            </div>
            <div class="app_horizontal_spacer">
            </div>
            
            <!-- read only by -->
            <div class="checklist_fields_left">
                <asp:Label ID="lblReadOnly" runat="server"
                    Text="R<span class=access_key>e</span>ad Only For"
                    CssClass="app_label4"
                    AssociatedControlID="cblReadOnly">
                </asp:Label>
            </div>
            <div class="checklist_fields_right">
                <div style="float:left;">
                    <asp:CheckBoxList ID="cblReadOnly" runat="server"
                        CssClass="app_label4"
                        AccessKey="E"
                        DataValueField="USER_ROLE_ID"
                        DataTextField="USER_ROLE_LABEL"
                        RepeatDirection="Horizontal"
                        Enabled="false" 
                        AutoPostBack="true" 
                        onselectedindexchanged="cblReadOnly_SelectedIndexChanged">
                    </asp:CheckBoxList>
                </div>
            </div>
            <div class="app_horizontal_spacer">
            </div>
            
            <!-- closeable by -->
            <div class="checklist_fields_left">
                <asp:Label ID="lblCloseable" runat="server"
                    Text="Cl<span class=access_key>o</span>seable By"
                    CssClass="app_label4"
                    AssociatedControlID="cblCloseable">
                </asp:Label>
            </div>
            <div class="checklist_fields_right">
                <div style="float:left;">
                    <asp:CheckBoxList ID="cblCloseable" runat="server"
                        CssClass="app_label4"
                        AccessKey="O"
                        DataValueField="USER_ROLE_ID"
                        DataTextField="USER_ROLE_LABEL"
                        RepeatDirection="Horizontal" 
                        AutoPostBack="true" 
                        Enabled="false" onselectedindexchanged="cblCloseable_SelectedIndexChanged">
                    </asp:CheckBoxList>
                </div>
            </div>
            <div class="app_horizontal_spacer">
            </div>
            
            <!-- TIU Note -->
            <div class="checklist_fields_left">
                <asp:Label ID="Label1" runat="server"
                    Text="TI<span class=access_key>U</span> Note"
                    CssClass="app_label4"
                    AssociatedControlID="cblTIUNote">
                </asp:Label>
            </div>
            <div class="checklist_fields_right">
                <div style="float:left;">
                    <asp:CheckBoxList ID="cblTIUNote" runat="server"
                        CssClass="app_label4"
                        AccessKey="O"
                        DataValueField="USER_ROLE_ID"
                        DataTextField="USER_ROLE_LABEL"
                        RepeatDirection="Horizontal" 
                        AutoPostBack="true" 
                        Enabled="false" onselectedindexchanged="cblTIUNote_SelectedIndexChanged">
                    </asp:CheckBoxList>
                </div>
            </div>
        </asp:Panel>
        <div class="app_horizontal_spacer">
        </div>
        <uc:ItemEntry ID="ucItemEntry" runat="server" />
    </ContentTemplate>
</asp:UpdatePanel>
