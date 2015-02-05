<%@ Control Language="C#" AutoEventWireup="true" CodeFile="sp_ucPatItemHistory.ascx.cs" Inherits="sp_ucPatItemHistory" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
    Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<h1 id="hItem" runat="server"
    class="app_label1">
</h1>
<div class="app_horizontal_spacer">
</div>
<asp:Panel ID="pnlComponents" runat="server"
    Visible="false"
    Width="700">
    <div style="float:left; padding-top:5px; width:110px;">
        <asp:Label ID="lblComponents" runat="server"
            AssociatedControlID="rblComponents"
            CssClass="app_label4"
            Text="Item Components">
        </asp:Label>
    </div>
    <div style="float:left; width:585px;">
        <asp:RadioButtonList ID="rblComponents" runat="server"
            AutoPostBack="true"
            DataTextField="item_component_label"
            DataValueField="item_component_id"
            OnSelectedIndexChanged="OnSelIndexChangedComp"
            RepeatDirection="Horizontal">
        </asp:RadioButtonList>
    </div>
    <div class="app_horizontal_spacer">
    </div>
</asp:Panel>
<div style="border: solid 1px black;">
    <asp:Chart ID="chrtPatItems" runat="server"
        Width="700">
        <ChartAreas>
            <asp:ChartArea Name="caPatItems">
                <AxisX>
                    <LabelStyle Font="Arial"
                        Format="MM/dd/yyyy"
                        IntervalType="Auto" />
                </AxisX>
                <AxisY>
                    <LabelStyle Font="Arial" />
                </AxisY>
            </asp:ChartArea>
        </ChartAreas>
        <Legends>
            <asp:Legend Name="lgdPatItems"
                Alignment="Center"
                BorderColor="Black"
                BorderDashStyle="Solid"
                BorderWidth="1"
                Docking="Bottom"
                Font="Arial"
                Title="Legend">
            </asp:Legend>
        </Legends>
        <Series>
            <asp:Series Name="srsPatItems"
                BorderWidth="2"
                Color="Black"
                ChartType="Line"
                IsVisibleInLegend="false"
                MarkerBorderColor="Black"
                MarkerBorderWidth="1"
                MarkerColor="Black"
                MarkerSize="10"
                MarkerStyle="Circle"
                XValueMember="entry_date"
                XValueType="DateTime"
                YValueMembers="component_value">
            </asp:Series>
        </Series>
    </asp:Chart>
</div>
<div class="app_horizontal_spacer">
</div>
<asp:Button ID="btnClose" runat="server"
    Text="Close" />
