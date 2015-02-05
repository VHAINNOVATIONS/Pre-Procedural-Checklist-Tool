<%@ Control Language="C#" AutoEventWireup="true" CodeFile="app_ucLogin.ascx.cs" Inherits="app_ucLogin" %>

<asp:UpdatePanel ID="upLogin" runat="server"
    RenderMode="Block">
    <ContentTemplate>
        <asp:Panel ID="pnlLogin" runat="server"
            DefaultButton="btnOKLogin">
            <div id="divLoginStatus" runat="server">
            </div>
            <div class="login_fields">
                <div class="login_fields_left">
                    <asp:Label ID="lblUID" runat="server"
                        AccessKey="U"
                        AssociatedControlID="txtUID"
                        CssClass="app_label4"
                        Text="<span class=access_key>U</span>ser Name">
                    </asp:Label>
                </div>
                <div class="login_fields_right">
                    <asp:TextBox ID="txtUID" runat="server"
                        Width="150">
                    </asp:TextBox>
                </div>
                <div class="app_horizontal_spacer">
                </div>
                <div class="login_fields_left">
                    <asp:Label ID="lblPWD" runat="server"
                        AccessKey="P"
                        AssociatedControlID="txtPWD"
                        CssClass="app_label4"
                        Text="<span class=access_key>P</span>assword">
                    </asp:Label>
                </div>
                <div class="login_fields_right">
                    <asp:TextBox ID="txtPWD" runat="server"
                        TextMode="Password"
                        Width="150">
                    </asp:TextBox>
                </div>
                <div class="app_horizontal_spacer">
                </div>
                <div class="login_fields_left">
                    <asp:Label ID="lblRegion" runat="server"
                        AccessKey="R"
                        AssociatedControlID="ddlRegion"
                        CssClass="app_label4"
                        Text="<span class=access_key>R</span>egion">
                    </asp:Label>
                </div>
                <div class="login_fields_right">
                   <asp:DropDownList ID="ddlRegion" runat="server" 
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlRegion_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>
                <div class="app_horizontal_spacer">
                </div>
                <div class="login_fields_left">
                    <asp:Label ID="lblSite" runat="server"
                        AccessKey="S"
                        AssociatedControlID="ddlSite"
                        CssClass="app_label4"
                        Text="<span class=access_key>S</span>ite">
                    </asp:Label>
                </div>
                <div class="login_fields_right">
                    <asp:DropDownList ID="ddlSite" runat="server">
                    </asp:DropDownList>
                </div>
                <div class="app_horizontal_spacer">
                </div>
                
            </div>
            
        </asp:Panel>
        <div class="app_horizontal_spacer">
        </div>
        <asp:Button ID="btnOKLogin" runat="server"
            Text="Login"
            OnClick="btnOKLogin_Click" />
        <asp:Button ID="btnCancelLogin" runat="server"
            Text="Cancel" />
    </ContentTemplate>
</asp:UpdatePanel>
