<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ce_ucStateSelector.ascx.cs"
    Inherits="ce_ucStateSelector" %>
    
<%@ Register TagPrefix="uc" TagName="TemporalStateSelector" Src="~/ce_ucTemporalStateSelector.ascx" %>
<%@ Register TagPrefix="uc" TagName="OutcomeStateSelector" Src="~/ce_ucOutcomeStateSelector.ascx" %>
<%@ Register TagPrefix="uc" TagName="DecisionStateSelector" Src="~/ce_ucDecisionStateSelector.ascx" %>

<div style="width:910px;">
    <div style="float:left; width:605px;">
        <!-- temporal states -->
        <div style="float:left;">
            <uc:TemporalStateSelector ID="ucTemporalStateSelector" runat="server" />
        </div>
        <!-- outcome states -->
        <div style="float:right;">
            <uc:OutcomeStateSelector ID="ucOutcomeStateSelector" runat="server" />
        </div>
    </div>
    <!-- decision states -->
    <div style="float:right;">
        <uc:DecisionStateSelector ID="ucDecisionStateSelector" runat="server" />
    </div>
</div>