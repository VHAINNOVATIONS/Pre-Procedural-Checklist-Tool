<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ve_ucServiceEdit.ascx.cs"
    Inherits="ve_ucServiceEdit" %>

<div class="service_fields">
    <div class="service_fields_left">
    </div>
    <div class="service_fields_right">
        <asp:CheckBox ID="chkActive" runat="server"
            Text="<span class=access_key>A</span>ctive"
            AccessKey="A"
            CssClass="app_label4" />
    </div>
    <div class="app_horizontal_spacer">
    </div>
    <div class="service_fields_left">
        <asp:Label ID="lblServiceLabel" runat="server"
            Text="<span class=access_key>S</span>ervice Label"
            AccessKey="S"
            AssociatedControlID="txtService"
            CssClass="app_label4">
        </asp:Label>
    </div>
    <div class="service_fields_right">
        <asp:TextBox ID="txtService" runat="server"
            MaxLength="50"
            Width="225">
        </asp:TextBox>
    </div>
</div>
<div class="app_horizontal_spacer">
</div>
<asp:Button ID="btnSave" runat="server"
    Text="Save"
    OnClick="OnClickSave" />
<asp:Button ID="btnCancel" runat="server"
    Text="Cancel" />