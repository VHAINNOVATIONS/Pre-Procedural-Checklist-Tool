<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ce_ucStateLogicSelector.ascx.cs"
    Inherits="ce_ucStateLogicSelector" %>
    
<%@ Register TagPrefix="uc" TagName="StateSelector" Src="~/ce_ucStateSelector.ascx" %>
<%@ Register TagPrefix="uc" TagName="LogicSelector" Src="~/ce_ucLogicSelector.ascx" %>

<div style="float: left; text-align: right; width: 115px;">
    <asp:Label ID="lblSelector" runat="server"
        AssociatedControlID="rblSelector"
        CssClass="app_label4"
        Text="<span class=access_key>S</span>elector Category">
    </asp:Label>
</div>
<div style="float: right; width: 790px;">
    <asp:RadioButtonList ID="rblSelector" runat="server"
        AccessKey="S"
        AutoPostBack="true"
        CssClass="app_label4"
        OnSelectedIndexChanged="OnSelIndexChangedSelector"
        RepeatDirection="Horizontal"
        RepeatLayout="Flow">
        <asp:ListItem Text="State"
             Value="1">
        </asp:ListItem>
        <asp:ListItem Text="Logic"
            Value="2">
        </asp:ListItem>
    </asp:RadioButtonList>
</div>
<div class="app_horizontal_spacer">
</div>
<asp:MultiView ID="mvStateLogicSelector" runat="server">
    <asp:View ID="vStateSelector" runat="server">
        <uc:StateSelector ID="ucStateSelector" runat="server" />
    </asp:View>
    <asp:View ID="vLogicSelector" runat="server">
        <uc:LogicSelector ID="ucLogicSelector" runat="server" />
    </asp:View>
</asp:MultiView>