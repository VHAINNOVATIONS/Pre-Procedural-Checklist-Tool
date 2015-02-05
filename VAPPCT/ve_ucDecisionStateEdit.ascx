<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ve_ucDecisionStateEdit.ascx.cs"
    Inherits="ve_ucDecisionStateEdit" %>

<div class="decision_state_fields">
    <div class="decision_state_fields_left">
    </div>
    <div class="decision_state_fields_right">
        <asp:CheckBox ID="chkDSActive" runat="server"
            Text="<span class=access_key>A</span>ctive"
            AccessKey="A" />
    </div>
    <div class="app_horizontal_spacer">
    </div>
    <div class="decision_state_fields_left">
        <asp:Label ID="lblDecisionStateLabel" runat="server"
            Text="<span class=access_key>D</span>ecision State Label"
            AccessKey="D"
            AssociatedControlID="txtDSLabel">
        </asp:Label>
    </div>
    <div class="decision_state_fields_right">
        <asp:TextBox ID="txtDSLabel" runat="server"
            MaxLength="50"
            Width="225">
        </asp:TextBox>
    </div>
    <div class="app_horizontal_spacer">
    </div>
    <div class="decision_state_fields_left">
        <asp:Label ID="lblDefinition" runat="server"
            Text="De<span class=access_key>f</span>inition"
            AccessKey="F"
            AssociatedControlID="ddlDSDefinition">
        </asp:Label>
    </div>
    <div class="decision_state_fields_right">
        <asp:DropDownList ID="ddlDSDefinition" runat="server">
            </asp:DropDownList>
    </div>
</div>
<div class="app_horizontal_spacer">
</div>
<asp:Button ID="btnucDSESave" runat="server"
    Text="Save"
    OnClick="OnClickSave" />
<asp:Button ID="btnucDSECancel" runat="server"
    Text="Cancel"
    OnClick="OnClickCancel" />