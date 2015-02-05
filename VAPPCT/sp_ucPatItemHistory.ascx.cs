using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.Web.UI.DataVisualization.Charting;
using System.Drawing;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class sp_ucPatItemHistory : CAppUserControlPopup
{
    const int kBackColorAlpha = 200;

    public long ItemID
    {
        get
        {
            object obj = ViewState[ClientID + "ItemID"];
            return (obj != null) ? Convert.ToInt64(obj) : -1;
        }
        set { ViewState[ClientID + "ItemID"] = value; }
    }

    public string PatientID
    {
        get
        {
            object obj = ViewState[ClientID + "PatientID"];
            return (obj != null) ? Convert.ToString(obj) : string.Empty;
        }
        set { ViewState[ClientID + "PatientID"] = value; }
    }

    protected DataTable PatientItemComponents
    {
        get
        {
            object obj = ViewState[ClientID + "PatientItemComponents"];
            return (obj != null) ? obj as DataTable : null;
        }
        set { ViewState[ClientID + "PatientItemComponents"] = value; }
    }

    protected DataTable ItemComponents
    {
        get
        {
            object obj = ViewState[ClientID + "ItemComponents"];
            return (obj != null) ? obj as DataTable : null;
        }
        set { ViewState[ClientID + "ItemComponents"] = value; }
    }

    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        throw new NotImplementedException();
    }

    public override CStatus SaveControl()
    {
        throw new NotImplementedException();
    }

    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        Title = "Item History";

        CItemData id = new CItemData(BaseMstr.BaseData);
        CItemDataItem diItem = null;
        CStatus status = id.GetItemDI(ItemID, out diItem);
        if (!status.Status)
        {
            return status;
        }
        hItem.InnerText = diItem.ItemLabel;

        CPatientItemData pid = new CPatientItemData(BaseMstr.BaseData);
        DataSet dsPIC = null;
        status = pid.GetPatItemCompDS(PatientID, ItemID, out dsPIC);
        if (!status.Status)
        {
            return status;
        }
        PatientItemComponents = dsPIC.Tables[0];

        CItemComponentData icd = new CItemComponentData(BaseMstr.BaseData);
        DataSet dsIC = null;
        status = icd.GetItemComponentOJDS(ItemID, k_ACTIVE_ID.Active, out dsIC);
        if (!status.Status)
        {
            return status;
        }

        ItemComponents = dsIC.Tables[0];
        if (ItemComponents.Rows.Count > 1)
        {
            pnlComponents.Visible = true;
            rblComponents.DataSource = ItemComponents;
            rblComponents.DataBind();
            DataRow rdItemComponent = ItemComponents.Rows[0];
            rblComponents.SelectedValue = rdItemComponent["item_component_id"].ToString();
            GraphItemComponent(rdItemComponent);
        }
        else
        {
            pnlComponents.Visible = false;
            GraphItemComponent(ItemComponents.Rows[0]);
        }

        return new CStatus();
    }

    protected void OnSelIndexChangedComp(object sender, EventArgs e)
    {
        DataRow[] drIC = ItemComponents.Select("item_component_id = " + rblComponents.SelectedValue);
        GraphItemComponent(drIC[0]);
        ShowMPE();
    }

    protected void GraphItemComponent(DataRow drItemComponent)
    {
        AddStripLines(drItemComponent);

        Series srsPatItems = chrtPatItems.Series["srsPatItems"];
        if (srsPatItems == null)
        {
            return;
        }
        srsPatItems.ToolTip = "Value = #VALY{n}\nDate = #VALX{g}";

        DataRow[] drPIC = PatientItemComponents.Select(
            "item_component_id = " + drItemComponent["item_component_id"].ToString(),
            "entry_date");
        chrtPatItems.DataSource = drPIC;
        chrtPatItems.DataBind();

    }

    protected void AddStripLines(DataRow drItemComponent)
    {
        // strip lines
        ChartArea caPatItems = chrtPatItems.ChartAreas["caPatItems"];
        Legend lgdPatItems = chrtPatItems.Legends["lgdPatItems"];
        if (caPatItems == null
            || lgdPatItems == null)
        {
            return;
        }

        // legend items
        lgdPatItems.CustomItems.Add(Color.FromArgb(kBackColorAlpha, Color.DarkRed), "Critical High/Low");
        lgdPatItems.CustomItems.Add(Color.FromArgb(kBackColorAlpha, Color.Red), "High/Low");
        lgdPatItems.CustomItems.Add(Color.FromArgb(kBackColorAlpha, Color.Green), "Normal");

        // legal min to critical low
        double dLegalMin = Convert.ToDouble(drItemComponent["legal_min"]);
        double dCriticalLow = Convert.ToDouble(drItemComponent["critical_low"]);
        caPatItems.AxisY.Minimum = dLegalMin;

        StripLine slCriticalLow = new StripLine();
        slCriticalLow.IntervalOffset = dLegalMin;
        slCriticalLow.StripWidth = dCriticalLow - dLegalMin;
        slCriticalLow.BackColor = Color.FromArgb(kBackColorAlpha, Color.DarkRed);
        caPatItems.AxisY.StripLines.Add(slCriticalLow);

        // critical low to low
        double dLow = Convert.ToDouble(drItemComponent["low"]);

        StripLine slLow = new StripLine();
        slLow.IntervalOffset = dCriticalLow;
        slLow.StripWidth = dLow - dCriticalLow;
        slLow.BackColor = Color.FromArgb(kBackColorAlpha, Color.Red);
        caPatItems.AxisY.StripLines.Add(slLow);

        // low to high
        double dHigh = Convert.ToDouble(drItemComponent["high"]);

        StripLine slNormal = new StripLine();
        slNormal.IntervalOffset = dLow;
        slNormal.StripWidth = dHigh - dLow;
        slNormal.BackColor = Color.FromArgb(kBackColorAlpha, Color.Green);
        caPatItems.AxisY.StripLines.Add(slNormal);

        // high to critical high
        double dCriticalHigh = Convert.ToDouble(drItemComponent["critical_high"]);

        StripLine slHigh = new StripLine();
        slHigh.IntervalOffset = dHigh;
        slHigh.StripWidth = dCriticalHigh - dHigh;
        slHigh.BackColor = Color.FromArgb(kBackColorAlpha, Color.Red);
        caPatItems.AxisY.StripLines.Add(slHigh);

        // critical high to legal max
        double dLegalMax = Convert.ToDouble(drItemComponent["legal_max"]);
        caPatItems.AxisY.Maximum = dLegalMax;

        StripLine slCriticalHigh = new StripLine();
        slCriticalHigh.IntervalOffset = dCriticalHigh;
        slCriticalHigh.StripWidth = dLegalMax - dCriticalHigh;
        slCriticalHigh.BackColor = Color.FromArgb(kBackColorAlpha, Color.DarkRed);
        caPatItems.AxisY.StripLines.Add(slCriticalHigh);
    }
}
