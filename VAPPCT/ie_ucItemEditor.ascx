<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ie_ucItemEditor.ascx.cs"
    Inherits="ie_ucItemEditor" %>
    
<%@ Register TagPrefix="uc" TagName="MapNoteTitle" Src="~/ie_ucMapNoteTitle.ascx" %>
<%@ Register TagPrefix="uc" TagName="MapLabTest" Src="~/ie_ucMapLabTest.ascx" %>
<%@ Register TagPrefix="uc" TagName="ItemComponentEditor" Src="~/ie_ucItemComponentEditor.ascx" %>
<%@ Register TagPrefix="uc" TagName="ItemCollectionEditor" Src="~/ie_ucItemCollectionEditor.ascx" %>

<h1 class="app_label1">
    <asp:Button ID="btnCollapseFields" runat="server"
        OnClick="OnClickCollapseFields"
        Text="-"
        Width="30" />
    Item Fields
</h1>
<asp:UpdatePanel ID="upComponents" runat="server"
    RenderMode="Block">
    <ContentTemplate>
        <asp:Panel ID="pnlFields" runat="server"
            CssClass="item_fields">
            <!-- active -->
            <div class="item_fields_left">
            </div>
            <div class="item_fields_right">
                <asp:CheckBox ID="chkActive" runat="server"
                    AccessKey="A"
                    CssClass="app_label4"
                    Text="<span class=access_key>A</span>ctive" />
            </div>
            <div class="app_horizontal_spacer">
            </div>
            
            <!-- type -->
            <div class="item_fields_left">
                <asp:Label ID="lblType" runat="server"
                    AccessKey="T"
                    AssociatedControlID="ddlType"
                    CssClass="app_label4"
                    Text="<span class=access_key>T</span>ype">
                </asp:Label>
            </div>
            <div class="item_fields_right">
                <asp:DropDownList ID="ddlType" runat="server"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="OnSelChangeType">
                </asp:DropDownList>
            </div>
            <div class="app_horizontal_spacer">
            </div>
            
            <!-- map -->
            <div class="item_fields_left">
                <asp:Label ID="lblMapID" runat="server"
                    AccessKey="M"
                    AssociatedControlID="tbMapID"
                    CssClass="app_label4"
                    Text="<span class=access_key>M</span>ap ID (optional)">
                </asp:Label>
            </div>
            <div class="item_fields_right">
                <asp:TextBox ID="tbMapID" runat="server"
                    ReadOnly="true"
                    Width="200">
                </asp:TextBox>
                <asp:Button ID="btnMap" runat="server"
                    Enabled="false"
                    OnClick="OnClickMap"
                    Text="Map" />
            </div>
            <div class="app_horizontal_spacer">
            </div>
            
            <!-- group -->
            <div class="item_fields_left">
                <asp:Label ID="lblGroup" runat="server"
                    AccessKey="G"
                    AssociatedControlID="ddlGroup"
                    CssClass="app_label4"
                    Text="<span class=access_key>G</span>roup">
                </asp:Label>
            </div>
            <div class="item_fields_right">
                <asp:DropDownList ID="ddlGroup" runat="server">
                </asp:DropDownList>
            </div>
            <div class="app_horizontal_spacer">
            </div>
            
            <!-- label -->
            <div class="item_fields_left">
                <asp:Label ID="lblLabel" runat="server"
                    AccessKey="L"
                    AssociatedControlID="tbLabel"
                    CssClass="app_label4"
                    Text="<span class=access_key>L</span>abel">
                </asp:Label>
            </div>
            <div class="item_fields_right">
                <asp:TextBox ID="tbLabel" runat="server"
                    Width="400">
                </asp:TextBox>
            </div>
            <div class="app_horizontal_spacer">
            </div>

            <!-- description -->
            <div class="item_fields_left">
                <asp:Label ID="lblDescription" runat="server"
                    AccessKey="D"
                    AssociatedControlID="tbDescription"
                    CssClass="app_label4"
                    Text="<span class=access_key>D</span>escription">
                </asp:Label>
            </div>
            <div class="item_fields_right">
                <asp:TextBox ID="tbDescription" runat="server"
                    Rows="3" 
                    Style="font-family: Verdana, Arial;"
                    TextMode="MultiLine"
                    Width="400">
                </asp:TextBox>
            </div>
            <div class="app_horizontal_spacer">
            </div>
            
            <!-- lookback time -->
            <div class="item_fields_left">
                <asp:Label ID="lblLookback" runat="server"
                    AccessKey="B"
                    AssociatedControlID="tbLookback"
                    CssClass="app_label4"
                    Text="Look<span class=access_key>b</span>ack Time">
                </asp:Label>
            </div>
            <div class="item_fields_right">
                <asp:TextBox ID="tbLookback" runat="server"
                    MaxLength="4"
                    Width="40">
                </asp:TextBox>
                <asp:FilteredTextBoxExtender ID="ftbLookback" runat="server"
                    FilterType="Numbers"
                    TargetControlID="tbLookback">
                </asp:FilteredTextBoxExtender>
                <asp:Label ID="lblLookbackUnits" runat="server"
                    AssociatedControlID="tbLookback"
                    CssClass="app_label4"
                    Text="Day(s)">
                </asp:Label>
            </div>
        </asp:Panel>
        <div class="app_horizontal_spacer">
        </div>
                    
        <uc:ItemComponentEditor ID="ucItemComponentEditor" runat="server" />
        <uc:ItemCollectionEditor ID="ucItemCollectionEditor" runat="server"
            Visible="false" />
        <div class="app_horizontal_spacer">
        </div>
        
        <asp:Button ID="btnOK" runat="server"
            Text="Save"
            OnClick="OnClickSave" />
        <asp:Button ID="btnCancel" runat="server"
            Text="Cancel"
            OnClick="OnClickCancel" />
            
        <!-- mapping popup -->
        <asp:HiddenField ID="hfMapID" runat="server">
        </asp:HiddenField>
        <asp:ModalPopupExtender ID="mpeMap" runat="server"
            PopupControlID="pnlMap"
            TargetControlID="hfMapID">
        </asp:ModalPopupExtender>
        <asp:Panel ID="pnlMap" runat="server"
            CssClass="map_popup">
            <uc:MapNoteTitle ID="ucMapNoteTitle" runat="server"
                OnSelectNoteTitle="OnSelectNoteTitle"
                Visible="false" />
             <uc:MapLabTest ID="ucMapLabTest" runat="server"
                OnSelectLabTest="OnSelectLabTest"
                Visible="false" />
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
