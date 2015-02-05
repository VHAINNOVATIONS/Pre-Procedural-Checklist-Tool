<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ve_ucTemporalStateEdit.ascx.cs"
    Inherits="ve_ucTemporalStateEdit" %>

<div class="temporal_state_fields">
    <div class="temporal_state_fields_left">
    </div>
    <div class="temporal_state_fields_right">
        <asp:CheckBox ID="chkTSActive" runat="server" 
            Text="<span class=access_key>A</span>ctive"
            AccessKey="A" />
    </div>
    <div class="app_horizontal_spacer">
    </div>
    <div class="temporal_state_fields_left">
        <asp:Label ID="lblTemporalStateLabel" runat="server"
            Text="<span class=access_key>T</span>emporal State Label"
            AccessKey="T"
            AssociatedControlID="txtTSLabel">
        </asp:Label>
    </div>
    <div class="temporal_state_fields_right">
        <asp:TextBox ID="txtTSLabel" runat="server"
            Width="225"
            MaxLength="50">
        </asp:TextBox>
    </div>
    <div class="app_horizontal_spacer">
    </div>
    <div class="temporal_state_fields_left">
        <asp:Label ID="Label1" runat="server"
            Text="<span class=access_key>D</span>efinition"
            AccessKey="D"
            AssociatedControlID="ddlTSDefinition">
        </asp:Label>
    </div>
    <div class="temporal_state_fields_right">
        <asp:DropDownList ID="ddlTSDefinition" runat="server">
            </asp:DropDownList>
    </div>
</div>
<div class="app_horizontal_spacer">
</div>
<asp:Button ID="btnucTSESave" runat="server"
    Text="Save"
    OnClick="OnClickSave" />
<asp:Button ID="btnucTSECancel" runat="server"
    Text="Cancel"
    OnClick="OnClickCancel" />
