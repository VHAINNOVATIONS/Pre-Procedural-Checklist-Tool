<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ce_ucLogicSelector.ascx.cs"
    Inherits="ce_ucLogicSelector" %>

<div style="float: left; text-align: right; width: 115px;">
    <asp:Label ID="lblLogic" runat="server"
        AssociatedControlID="rblLogic"
        CssClass="app_label4"
        Text="<span class=access_key>L</span>ogic Category">
    </asp:Label>
</div>
<div style="float: right; width: 790px;">
    <asp:RadioButtonList ID="rblLogic" runat="server"
        AccessKey="L"
        AutoPostBack="true"
        CssClass="app_label4"
        OnSelectedIndexChanged="OnSelIndexChangedCats"
        RepeatDirection="Horizontal"
        RepeatLayout="Flow">
    </asp:RadioButtonList>
</div>
<div class="app_horizontal_spacer" style="height: 2px;">
</div>
<asp:MultiView ID="mvLogic" runat="server">
    <!-- operator -->
    <asp:View ID="vOperator" runat="server">
        <div style="width: 605px;">
            <div style="float: left;">
                <asp:Label ID="lblOperator" runat="server"
                    AccessKey="C"
                    AssociatedControlID="lbOperator"
                    CssClass="app_label4"
                    Text="Operator <span class=access_key>C</span>ategories">
                </asp:Label>
                <div class="app_horizontal_spacer">
                </div>
                <asp:ListBox ID="lbOperator" runat="server"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="OnSelIndexChangedOperator"
                    SkinID="FullLogicSelector">
                </asp:ListBox>
            </div>
            <div style="float: right;">
                <asp:Label ID="lblOperatorSubGroup" runat="server"
                    AccessKey="P"
                    AssociatedControlID="lbOperatorSubGroup"
                    CssClass="app_label4"
                    Text="Operator <span class=access_key>P</span>lace Holder(s)">
                </asp:Label>
                <div class="app_horizontal_spacer">
                </div>
                <asp:ListBox ID="lbOperatorSubGroup" runat="server"
                    SkinID="FullLogicSelector"
                    Visible="false">
                </asp:ListBox>
            </div>
        </div>
    </asp:View>
    
    <!-- patient -->
    <asp:View ID="vPatient" runat="server">
        <asp:Label ID="lblPatient" runat="server"
            AccessKey="P"
            AssociatedControlID="lbPatient"
            CssClass="app_label4"
            Text="Patient <span class=access_key>P</span>lace Holder(s)">
        </asp:Label>
        <div class="app_horizontal_spacer">
        </div>
        <asp:ListBox ID="lbPatient" runat="server"
            SkinID="FullLogicSelector">
        </asp:ListBox>
    </asp:View>
    
    <!-- item -->
    <asp:View ID="vItem" runat="server">
        <div style="float: left;">
            <asp:Label ID="lblItem" runat="server"
                AccessKey="I"
                AssociatedControlID="lbItem"
                CssClass="app_label4"
                Text="<span class=access_key>I</span>tem(s)">
            </asp:Label>
            <div class="app_horizontal_spacer">
            </div>
            <asp:ListBox ID="lbItem" runat="server"
                AutoPostBack="true"
                OnSelectedIndexChanged="OnSelIndexChangedItem"
                SkinID="FullLogicSelector">
            </asp:ListBox>
        </div>
        <div style="float: right; width: 605px;">
            <div style="float: left;">
                <asp:Label ID="lblItemPH" runat="server"
                    AccessKey="T"
                    AssociatedControlID="lbItem"
                    CssClass="app_label4"
                    Text="I<span class=access_key>t</span>em Place Holder(s)">
                </asp:Label>
                <div class="app_horizontal_spacer">
                </div>
                <asp:ListBox ID="lbItemPH" runat="server"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="OnSelIndexChangedItemPH"
                    SkinID="HalfLogicSelector"
                    Visible="false">
                </asp:ListBox>
                <div class="app_horizontal_spacer">
                </div>
                <asp:Label ID="lblItemComponent" runat="server"
                    AccessKey="C"
                    AssociatedControlID="lbItem"
                    CssClass="app_label4"
                    Text="Item <span class=access_key>C</span>omponent">
                </asp:Label>
                <div class="app_horizontal_spacer">
                </div>
                <asp:ListBox ID="lbItemComponent" runat="server"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="OnSelIndexChangedItemComp"
                    SkinID="HalfLogicSelector"
                    Visible="false">
                </asp:ListBox>
            </div>
            <div style="float: right;">
                <asp:Label ID="lblItemCompPH" runat="server"
                    AccessKey="O"
                    AssociatedControlID="lbItem"
                    CssClass="app_label4"
                    Text="Item C<span class=access_key>o</span>mponent Place Holder(s)">
                </asp:Label>
                <div class="app_horizontal_spacer">
                </div>
                <asp:ListBox ID="lbItemCompPH" runat="server"
                    AppendDataBoundItems="true"
                    SkinID="FullLogicSelector"
                    Visible="false">
                </asp:ListBox>
            </div>
        </div>
    </asp:View>
    
    <!-- static -->
    <asp:View ID="vStatic" runat="server">
        <div style="width: 605px;">
            <div style="float: left;">
                <asp:Label ID="lblStatic" runat="server"
                    AccessKey="C"
                    AssociatedControlID="lbStatic"
                    CssClass="app_label4"
                    Text="Static <span class=access_key>C</span>ategories">
                </asp:Label>
                <div class="app_horizontal_spacer">
                </div>
                <asp:ListBox ID="lbStatic" runat="server"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="OnSelIndexChangedStatic"
                    SkinID="FullLogicSelector">
                </asp:ListBox>
            </div>
            <div style="float: right;">
                <asp:Label ID="lblStaticSpecifier" runat="server"
                    AccessKey="P"
                    AssociatedControlID="lbStaticSpecifier"
                    CssClass="app_label4"
                    Text="Static <span class=access_key>P</span>lace Holder(s)">
                </asp:Label>
                <div class="app_horizontal_spacer">
                </div>
                <asp:ListBox ID="lbStaticSpecifier" runat="server"
                    SkinID="FullLogicSelector"
                    Visible="false">
                </asp:ListBox>
            </div>
        </div>
    </asp:View>
    
    <!-- action -->
    <asp:View ID="vAction" runat="server">
        <div style="float: left;">
            <asp:Label ID="lblAction" runat="server"
                AccessKey="A"
                AssociatedControlID="lbAction"
                CssClass="app_label4"
                Text="<span class=access_key>A</span>ctions">
            </asp:Label>
            <div class="app_horizontal_spacer">
            </div>
            <asp:ListBox ID="lbAction" runat="server"
                AutoPostBack="true"
                OnSelectedIndexChanged="OnSelIndexChangedAction"
                SkinID="FullLogicSelector">
            </asp:ListBox>
        </div>
        <div style="float: right; width: 605px;">
            <div style="float: left;">
                <asp:Label ID="lblActionSpecifier" runat="server"
                    AssociatedControlID="lbActionSpecifier"
                    CssClass="app_label4">
                </asp:Label>
                <div class="app_horizontal_spacer">
                </div>
                <asp:ListBox ID="lbActionSpecifier" runat="server"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="OnSelIndexChangedActionSpecifier"
                    SkinID="FullLogicSelector"
                    Visible="false">
                </asp:ListBox>
            </div>
            <div style="float: right;">
                <asp:Label ID="lblActionValue" runat="server"
                    AccessKey="P"
                    AssociatedControlID="lbActionValue"
                    CssClass="app_label4"
                    Text="Action <span class=access_key>P</span>lace Holder(s)">
                </asp:Label>
                <div class="app_horizontal_spacer">
                </div>
                <asp:ListBox ID="lbActionValue" runat="server"
                    SkinID="FullLogicSelector"
                    Visible="false">
                </asp:ListBox>
            </div>
        </div>
    </asp:View>
</asp:MultiView>
<div class="app_horizontal_spacer" style="height: 2px;">
</div>
<asp:Button ID="btnAdd" runat="server"
    Text="Add"
    OnClick="OnClickAdd" />