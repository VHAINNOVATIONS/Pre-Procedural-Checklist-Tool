<%@ Control Language="C#" AutoEventWireup="true" CodeFile="app_ucTimer.ascx.cs" Inherits="app_ucTimer" %>
<asp:UpdatePanel ID="upTimer" runat="server"
    RenderMode="Block"
    UpdateMode="Conditional">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="timRefreshTimer" EventName="Tick" />
        <asp:AsyncPostBackTrigger ControlID="ddlTimeInterval" EventName="SelectedIndexChanged" />
        <asp:AsyncPostBackTrigger ControlID="btnRefresh" EventName="Click" />
    </Triggers>
    <ContentTemplate>
        <div style="float: left; padding-right: 5px;">
            <asp:Label ID="lblLastUpdatedLabel" runat="server"
                CssClass="app_label1"
                Text="Last:"
                AssociatedControlID="lblLastUpdated" />
            <asp:Label ID="lblLastUpdated" runat="server"
                CssClass="app_label4" />
        </div>
        <div style="float: right;">
            <asp:Label ID="lblNextUpdateLabel" runat="server"
                CssClass="app_label1"
                Text="Next:"
                AssociatedControlID="lblNextUpdate" />
            <asp:Label ID="lblNextUpdate" runat="server"
                CssClass="app_label4" />
        </div>
        <div class="app_horizontal_spacer">
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
<asp:Timer ID="timRefreshTimer" runat="server"
    OnTick="OnTickRefresh" />
<asp:UpdatePanel ID="upInterval" runat="server"
    RenderMode="Block"
    UpdateMode="Conditional">
    <ContentTemplate>
        <div style="float: right;">
            <asp:Label ID="lblTimeInterval" runat="server"
                Text="Interval"
                CssClass="app_label4"
                AccessKey="F"
                AssociatedControlID="ddlTimeInterval" />
            <asp:DropDownList ID="ddlTimeInterval" runat="server"
                AutoPostBack="true"
                OnSelectedIndexChanged="OnSelChangeInterval">
                <asp:ListItem>5</asp:ListItem>
                <asp:ListItem>6</asp:ListItem>
                <asp:ListItem>7</asp:ListItem>
                <asp:ListItem>8</asp:ListItem>
                <asp:ListItem>9</asp:ListItem>
                <asp:ListItem Selected="True">10</asp:ListItem>
                <asp:ListItem>15</asp:ListItem>
                <asp:ListItem>30</asp:ListItem>
            </asp:DropDownList>
            <asp:Label ID="lblTimeIntervalUnits" runat="server"
                Text="Min(s)"
                CssClass="app_label4"
                AssociatedControlID="ddlTimeInterval" />
            <asp:Button ID="btnRefresh" runat="server"
                Text="Refresh Now"
                OnClick="OnClickRefresh" />
        </div>
        <div class="app_horizontal_spacer">
        </div>
    </ContentTemplate>
</asp:UpdatePanel>
