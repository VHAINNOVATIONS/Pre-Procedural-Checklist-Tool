<%@ Control Language="C#" AutoEventWireup="true" CodeFile="sp_ucPatItemEditor.ascx.cs"
    Inherits="sp_ucPatItemEditor" %>
    
<%@ Reference Control="~/app_ucTimer.ascx" %>
<%@ Register TagPrefix="uc" TagName="TimePicker" Src="~/app_ucTimePicker.ascx" %>
<%@ Register TagPrefix="uc" TagName="Collection" Src="~/sp_ucCollection.ascx" %>
<%@ Register TagPrefix="uc" TagName="NoteTitle" Src="~/sp_ucNoteTitle.ascx" %>

<asp:Panel ID="pnlCollection" runat="server">
    <asp:Label ID="lblForCollection" runat="server"
        AssociatedControlID="lblColl"
        CssClass="app_label2"
        Text="Collection:">
    </asp:Label>
    <div class="app_horizontal_spacer">
    </div>
    <asp:Label ID="lblColl" runat="server"
        CssClass="app_label4">
    </asp:Label>
    <div class="app_horizontal_spacer">
    </div>
    <asp:Label ID="lblForCollDesc" runat="server"
        AssociatedControlID="lblCollDesc"
        CssClass="app_label2"
        Text="Description:">
    </asp:Label>
    <div class="app_horizontal_spacer">
    </div>
    <asp:Label ID="lblCollDesc" runat="server"
        CssClass="app_label4">
    </asp:Label>
    <div class="app_horizontal_spacer">
    </div>
    <div class="patient_item_fields">
        <div class="pif_left_padding_top">
            <asp:Label ID="lblColItems" runat="server"
                AssociatedControlID="ddlColItems"
                CssClass="app_label4"
                Text="Collection Item(s)">
            </asp:Label>
        </div>
        <div class="pif_right">
            <asp:DropDownList ID="ddlColItems" runat="server"
                AutoPostBack="true" 
                OnSelectedIndexChanged="OnSelIndexChangedColItem">
            </asp:DropDownList>
        </div>
        <div class="app_horizontal_spacer">
        </div>
    </div>
</asp:Panel>
<div class="patient_item_fields">
    <div class="pif_left">
        <asp:Label ID="lblForItem" runat="server"
            AssociatedControlID="lblItem"
            CssClass="app_label2"
            Text="Item:">
        </asp:Label>
    </div>
    <div class="patient_item_fields_right">
        <asp:Label ID="lblItem" runat="server"
            CssClass="app_label4">
        </asp:Label>
    </div>
    <div class="app_horizontal_spacer">
    </div>
    
    <div class="pif_left">
        <asp:Label ID="lblForItemDescription" runat="server"
            AssociatedControlID="lblItemDescription"
            CssClass="app_label2"
            Text="Description:">
        </asp:Label>
    </div>
    <div class="patient_item_fields_right">
        <asp:Label ID="lblItemDescription" runat="server"
            CssClass="app_label4">
        </asp:Label>
    </div>
    <div class="app_horizontal_spacer">
    </div>
    
    <div class="pif_left_padding_top">
        <asp:Label ID="lblResults" runat="server"
            AccessKey="R"
            AssociatedControlID="ddlItems"
            CssClass="app_label4"
            Text="<span span class=access_key>R</span>esult(s)">
        </asp:Label>
    </div>
    <div class="patient_item_fields_right">
        <asp:DropDownList ID="ddlItems" runat="server"
            AutoPostBack="true"
            OnSelectedIndexChanged="OnSelIndexChangedItem">
        </asp:DropDownList>
    </div>
    <div class="app_horizontal_spacer">
    </div>
    
    <asp:Panel ID="pnlMapped" Visible="false" runat="server"
    CssClass="app_panel"
    Width="715"
    Height="305"
    ScrollBars="None">
    This is a mapped item and results cannot be entered or edited. 
    Please choose from the list of historical result(s) above or 
    enter new values in the source system.    
    </asp:Panel>
    
    <div class="pif_left_padding_top">
        <asp:Label ID="lblDate" runat="server"
            AccessKey="D"
            AssociatedControlID="txtEntryDate"
            CssClass="app_label4"
            Text="Result&nbsp;<span class=access_key>D</span>ate">
        </asp:Label>
    </div>
    <div class="patient_item_fields_right">
        <asp:TextBox ID="txtEntryDate" runat="server"
            AutoPostBack="true"
            OnTextChanged="OnEntryDateChanged"
            Width="70">
        </asp:TextBox>
        <asp:CalendarExtender ID="calEntryDate" runat="server"
            DefaultView="Days"
            Format="MM/dd/yyyy" 
            TargetControlID="txtEntryDate">
        </asp:CalendarExtender>
        <uc:TimePicker ID="ucTimePicker" runat="server" />
    </div>
    <div class="app_horizontal_spacer">
    </div>
</div>
<asp:Label CssClass="app_label2" ID="lblItemComps" runat="server" Text="Item Component(s)"></asp:Label>
<asp:Panel ID="pnlComponents" runat="server"
    CssClass="app_panel"
    Width="715"
    Height="105"
    ScrollBars="Vertical">
    <asp:GridView ID="gvComponents" runat="server"
        DataKeyNames="item_component_id, legal_min, critical_low, low, high, critical_high, legal_max"
        OnRowDataBound="OnRowDataBoundComp"
        Height="50"
        ShowHeader="false">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:RadioButton ID="rbSelComponent" runat="server"
                        GroupName="gnComponent"
                        OnClick="javascript:ToggleGVRadio(this.id,'rbSelComponent');"
                        Width="695" />
                    <asp:Label ID="lblComponent" runat="server"
                        AssociatedControlID="txtValue"
                        Width="200">
                    </asp:Label>
                    <asp:TextBox ID="txtValue" runat="server"
                        MaxLength="4000"
                        Width="200">
                    </asp:TextBox>
                    <asp:Label ID="lblUnits" runat="server"
                        Width="100">
                    </asp:Label>
                    <div class="app_horizontal_spacer">
                    </div>
                    <asp:Label ID="lblRanges" runat="server"
                        Width="695">
                    </asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Panel>
<uc:NoteTitle ID="ucNoteTitle" runat="server"
    Height="100"
    Width="715"
    Visible="false" />
<div class="app_horizontal_spacer">
</div>
<asp:Label CssClass="app_label2" ID="lblNewComment" runat="server" Text="New Comment"></asp:Label>
<asp:TextBox ID="txtComment" runat="server"
    MaxLength="4000"
    Rows="3"
    TextMode="MultiLine"
    Width="715">
</asp:TextBox>
<!--spacer-->
<div class="app_horizontal_spacer">
</div>
<asp:Label CssClass="app_label2" ID="lblCommentHistory" runat="server" Text="Comment History"></asp:Label>
<asp:Panel ID="pnlComments" runat="server"
    CssClass="app_panel"
    Height="100"
    ScrollBars="Vertical"
    Width="715">
    <asp:GridView ID="gvComments" runat="server"
        DataKeyNames="pat_item_id">
        <Columns>
            <asp:BoundField AccessibleHeaderText="Date"
                ControlStyle-CssClass="gv_truncated"
                ControlStyle-Width="150"
                DataField="comment_date" 
                HeaderStyle-Width="150"
                HeaderText="Date" />
            <asp:BoundField AccessibleHeaderText="Comment"
                ControlStyle-CssClass="gv_truncated"
                ControlStyle-Width="415"
                DataField="comment_text" 
                HeaderStyle-Width="420"
                HeaderText="Comment" />
            <asp:BoundField AccessibleHeaderText="User"
                ControlStyle-CssClass="gv_truncated"
                ControlStyle-Width="150"
                DataField="user_name" 
                HeaderStyle-Width="150"
                HeaderText="User" />
        </Columns>
    </asp:GridView>
</asp:Panel>
<!--spacer-->
<div class="app_horizontal_spacer">
</div>
<asp:Button ID="btnOK" runat="server"
    OnClick="OnClickOK"
    Text="Save" />
<asp:Button ID="btnCancel" runat="server"
    OnClick="OnClickCancel"
    Text="Cancel" />