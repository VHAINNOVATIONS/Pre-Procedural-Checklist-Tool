<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ve_ucItemGroupsEdit.ascx.cs"
    Inherits="ve_ucItemGroupsEdit" %>

<div class="item_group_fields">
    <div class="item_group_fields_left">
    </div>
    <div class="item_group_fields_right">
        <asp:CheckBox ID="chkItemGroupActive" runat="server"
            Text="<span class=access_key>A</span>ctive"
            AccessKey="A" />
    </div>
    <div class="app_horizontal_spacer">
    </div>
    <div class="item_group_fields_left">
        <asp:Label ID="lblItemGroupLabel" runat="server"
            Text="<span class=access_key>I</span>tem Group Label"
            AccessKey="I"
            AssociatedControlID="txtItemGroupLabel">
        </asp:Label>
    </div>
    <div class="item_group_fields_right">
        <asp:TextBox ID="txtItemGroupLabel" runat="server"
            MaxLength="30"
            Width="225">
        </asp:TextBox>
        <asp:FilteredTextBoxExtender ID="ftbItemGroup" runat="server"
            InvalidChars="/"
            FilterMode="InvalidChars"
            FilterType="Custom"
            TargetControlID="txtItemGroupLabel" />
    </div>
</div>
<div class="app_horizontal_spacer">
</div>
<asp:Button ID="btnucIGESave" runat="server"
    Text="Save"
    OnClick="OnClickSave" />
<asp:Button ID="btnucIGECancel" runat="server"
    Text="Cancel"
    OnClick="OnClickCancel" />