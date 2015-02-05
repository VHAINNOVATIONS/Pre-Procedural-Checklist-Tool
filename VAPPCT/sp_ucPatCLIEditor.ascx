<%@ Control Language="C#" AutoEventWireup="true" CodeFile="sp_ucPatCLIEditor.ascx.cs"
    Inherits="sp_ucPatCLIEditor" %>
    
<%@ Reference Control="~/app_ucTimer.ascx" %>
<%@ Register TagPrefix="uc" TagName="TimePicker" Src="~/app_ucTimePicker.ascx" %>
<%@ Register TagPrefix="uc" TagName="Collection" Src="~/sp_ucCollection.ascx" %>
<%@ Register TagPrefix="uc" TagName="NoteTitle" Src="~/sp_ucNoteTitle.ascx" %>

<asp:Label ID="lblItemlbl" runat="server"
    AssociatedControlID="lblItem"
    CssClass="app_label2"
    Text="Item:">
</asp:Label>
<div class="app_horizontal_spacer">
</div>
<asp:Label ID="lblItem" runat="server"
    CssClass="app_label4">
</asp:Label>
<div class="app_horizontal_spacer">
</div>
<asp:Label ID="lblItemDesclbl" runat="server"
    AssociatedControlID="lblItemDescription"
    CssClass="app_label2"
    Text="Item Description:">
</asp:Label>
<div class="app_horizontal_spacer">
</div>
<asp:Label ID="lblItemDescription" runat="server"
    CssClass="app_label4">
</asp:Label>
<div class="app_horizontal_spacer">
</div>

<h2 class="app_label2">
    Last Result
</h2>
<asp:Panel ID="pnlLastResult" runat="server"
    CssClass="app_panel"
    ScrollBars="Vertical"
    Height="120"
    Width="610"
    Visible="false">
    <asp:Literal ID="litLastResult" runat="server">
    </asp:Literal>
</asp:Panel>
<uc:Collection ID="ucCollection" runat="server"
    Height="120"
    Width="615"
    Visible="false" />
<uc:NoteTitle ID="ucNoteTitle" runat="server"
    Height="120"
    Width="615"
    Visible="false" />
<div class="app_horizontal_spacer">
</div>

<asp:Label ID="lblTemporalState" runat="server"
    CssClass="app_label4"
    Text="Temporal State:">
</asp:Label>
<asp:Label ID="lblCurrentTS" runat="server"
    CssClass="app_label4"
    Text="Label">
</asp:Label>
<div class="app_horizontal_spacer">
</div>

<asp:Label ID="lblOutcomeState" runat="server"
    CssClass="app_label4"
    Text="Outcome State:">
</asp:Label>
<asp:Label ID="lblCurrentOS" runat="server"
    CssClass="app_label4"
    Text="Label">
</asp:Label>
<div class="app_horizontal_spacer">
</div>

<asp:Label ID="lblDecisionState" runat="server"
    CssClass="app_label4"
    Text="Decision State:">
</asp:Label>
<asp:DropDownList ID="ddlDS" runat="server">
</asp:DropDownList>
<div class="app_horizontal_spacer">
</div>

<h2 class="app_label2">
    Required Comment
</h2>
<asp:TextBox ID="txtComment" runat="server"
    MaxLength="4000"
    TextMode="MultiLine"
    Rows="3"
    Width="615">
</asp:TextBox>
<div class="app_horizontal_spacer">
</div>

<h2 class="app_label2">
    Comment History
</h2>
<asp:Panel ID="pnlComments" runat="server"
    CssClass="app_panel"
    Width="615"
    Height="100px"
    ScrollBars="Vertical">
    <asp:GridView ID="gvComments" runat="server"
        DataKeyNames="pat_cl_id">
        <Columns>
            <asp:BoundField HeaderStyle-HorizontalAlign="Left"
                HeaderText="Date"
                ItemStyle-Width="140"
                DataField="override_date" />
            <asp:BoundField HeaderStyle-HorizontalAlign="Left"
                HeaderText="DS"
                ItemStyle-Width="100"
                DataField="ds_definition_label" />
            <asp:BoundField HeaderStyle-HorizontalAlign="Left"
                HeaderText="Comment"
                ItemStyle-Width="235"
                DataField="override_comment" />
            <asp:BoundField HeaderStyle-HorizontalAlign="Left"
                HeaderText="User"
                ItemStyle-Width="140"
                DataField="user_name" />
        </Columns>
    </asp:GridView>
</asp:Panel>
<div class="app_horizontal_spacer">
</div>

<asp:Button ID="btnOK" runat="server"
    Text="Save"
    OnClick="btnOK_Click" />
<asp:Button ID="btnCancel" runat="server"
    Text="Cancel"
    OnClick="btnCancel_Click" />