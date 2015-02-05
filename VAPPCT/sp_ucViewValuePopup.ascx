<%@ Control Language="C#" AutoEventWireup="true" CodeFile="sp_ucViewValuePopup.ascx.cs"
    Inherits="sp_ucViewValuePopup" %>
    
<%@ Register TagPrefix="uc" TagName="Collection" Src="~/sp_ucCollection.ascx" %>
<%@ Register TagPrefix="uc" TagName="NoteTitle" Src="~/sp_ucNoteTitle.ascx" %>

<uc:Collection ID="ucCollection" runat="server"
    Height="500"
    Width="640"
    Visible="false" />
<uc:NoteTitle ID="ucNoteTitle" runat="server"
    Height="500"
    Width="640"
    Visible="false" />
<div class="app_horizontal_spacer">
</div>
<asp:Button ID="btnClose" runat="server"
    Text="Close" />