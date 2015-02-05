using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class sp_ucNoteTitle : CAppUserControl
{
    public Unit Height { get; set; }
    public Unit Width { get; set; }

    /// <summary>
    /// US:1945 US:1880 item id 
    /// </summary>
    public long ItemID
    {
        get
        {
            object obj = ViewState[ClientID.ToString() + "ItemID"];
            return (obj == null) ? -1 : Convert.ToInt64(obj);
        }
        set { ViewState[ClientID.ToString() + "ItemID"] = value; }
    }

    /// <summary>
    /// patient item id
    /// </summary>
    public long PatientItemID
    {
        get
        {
            object obj = ViewState[ClientID.ToString() + "PatientItemID"];
            return (obj == null) ? -1 : Convert.ToInt64(obj);
        }
        set { ViewState[ClientID.ToString() + "PatientItemID"] = value; }
    }

    /// <summary>
    /// US:1945 US:1880 patient id
    /// </summary>
    public string PatientID
    {
        get
        {
            object obj = ViewState[ClientID.ToString() + "PatientID"];
            return (obj == null) ? string.Empty : obj.ToString();
        }
        set { ViewState[ClientID.ToString() + "PatientID"] = value; }
    }

    /// <summary>
    /// US:1945 US:1880 page load
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        tbNoteTitle.Height = Height;
        tbNoteTitle.Width = Width;
    }

    /// <summary>
    /// US:1945 US:1880 load control
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;

        tbNoteTitle.Text = string.Empty;

        CPatientItemData PatientItemData = new CPatientItemData(BaseMstr.BaseData);
        CPatientItemDataItem di = null;

        CStatus status = new CStatus();

        //if no pat item id then load the most recent
        if (PatientItemID < 1)
        {
            status = PatientItemData.GetMostRecentPatientItemDI(PatientID, 
                                                                ItemID, 
                                                                out di);
            if (!status.Status)
            {
                return status;
            }
        }
        else
        {
            status = PatientItemData.GetPatientItemDI(PatientID, 
                                                      ItemID,
                                                      PatientItemID, 
                                                      out di);
            if (!status.Status)
            {
                return status;
            }
        }

        if(string.IsNullOrEmpty(di.PatientID))
        {
            return new CStatus();
        }

        DataSet ds = null;
        status = PatientItemData.GetPatientItemComponentDS(di.PatientID, di.PatItemID, di.ItemID, out ds);
        if (!status.Status)
        {
            return status;
        }

        tbNoteTitle.Text = di.ItemLabel + "\r\n";
        foreach (DataRow dr in ds.Tables[0].Rows)
        {
            tbNoteTitle.Text += dr["ITEM_COMPONENT_LABEL"].ToString() + "\r\n";
            tbNoteTitle.Text += dr["COMPONENT_VALUE"].ToString() + "\r\n";
        }

        tbNoteTitle.Text += "\r\n";

        return new CStatus();
    }

    public override CStatus SaveControl()
    {
        throw new NotImplementedException();
    }

    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        tbNoteTitle.Text = string.Empty;
    }
}
