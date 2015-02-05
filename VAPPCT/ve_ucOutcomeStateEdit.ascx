<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ve_ucOutcomeStateEdit.ascx.cs"
    Inherits="ve_ucOutcomeStateEdit" %>

<div class="outcome_state_fields">
    <div class="outcome_state_fields_left">
    </div>
    <div class="outcome_state_fields_right">
        <asp:CheckBox ID="chkOSActive" runat="server"
            Text="<span class=access_key>A</span>ctive"
            AccessKey="A" />
    </div>
    <div class="app_horizontal_spacer">
    </div>
    <div class="outcome_state_fields_left">
        <asp:Label ID="lblOutcomeStateLabel" runat="server"
            Text="<span class=access_key>O</span>utcome State Label"
            AccessKey="O"
            AssociatedControlID="txtOSLabel">
        </asp:Label>
    </div>
    <div class="outcome_state_fields_right">
        <asp:TextBox ID="txtOSLabel" runat="server"
             MaxLength="50"
             Width="225">
        </asp:TextBox>
    </div>
    <div class="app_horizontal_spacer">
    </div>
    <div class="outcome_state_fields_left">
        <asp:Label ID="lblDefinition" runat="server"
            Text="<span class=access_key>D</span>efinition"
            AccessKey="D"
            AssociatedControlID="ddlOSDefinition">
        </asp:Label>
    </div>
    <div class="outcome_state_fields_right">
        <asp:DropDownList ID="ddlOSDefinition" runat="server">
        </asp:DropDownList>
    </div>
</div>
<div class="app_horizontal_spacer">
</div>
<asp:Button ID="btnucOSESave" runat="server"
    Text="Save"
    OnClick="OnClickSave" />
<asp:Button ID="btnucOSECancel" runat="server"
    Text="Cancel"
    OnClick="OnClickCancel" />
