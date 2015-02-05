<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ce_ucStateLogicEditor.ascx.cs"
    Inherits="ce_ucStateLogicEditor" %>
    
<%@ Register TagPrefix="uc" TagName="StateLogicSelector" Src="~/ce_ucStateLogicSelector.ascx" %>

<h1 id="hItem" runat="server"
    class="app_label1"
    style="width: 860px;">
</h1>
<asp:UpdatePanel ID="upStateSelector" runat="server"
    RenderMode="Block"
    UpdateMode="Conditional">
    <ContentTemplate>
        <uc:StateLogicSelector ID="ucStateLogicSelector" runat="server" />
        <div class="app_horizontal_spacer">
        </div>
        <h2 class="app_label2">
            Item Logic
            <asp:Button ID="btnValidate" runat="server"
                Text="Validate"
                OnClick="OnClickValidate" />
            <asp:Button ID="btnRestore" runat="server"
                Text="Restore Default"
                OnClick="OnClickRestore" />
        </h2>
        <asp:TextBox ID="txtItemLogic" runat="server"
            TextMode="MultiLine"
            MaxLength="4000" 
            Rows="10"
            Width="905">
        </asp:TextBox>
    </ContentTemplate>
</asp:UpdatePanel>
<div class="app_horizontal_spacer">
</div>
<!--save cancel-->
<asp:Button ID="btnucSave" runat="server"
    Text="Save"
    OnClick="btnucSave_Click" />
<asp:Button ID="btnucCancel" runat="server"
    Text="Cancel"
    OnClick="btnucCancel_Click" />
