using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class sp_ucCollection : CAppUserControl
{
    /// <summary>
    /// US:1883
    /// property
    /// stores the height of the collection text box
    /// </summary>
    public Unit Height { get; set; }

    /// <summary>
    /// US:1883
    /// property
    /// stores the width of the collection text box
    /// </summary>
    public Unit Width { get; set; }

    /// <summary>
    /// US:1883
    /// property
    /// stores a collection item for the user control
    /// </summary>
    public long CollectionItemID
    {
        get
        {
            object obj = ViewState[ClientID + "CollectionItemID"];
            return (obj == null) ? -1 : Convert.ToInt64(obj);
        }
        set { ViewState[ClientID + "CollectionItemID"] = value; }
    }

    /// <summary>
    /// US:1883
    /// property
    /// stores a patient id for the user control
    /// </summary>
    public string PatientID
    {
        get
        {
            object obj = ViewState[ClientID + "PatientID"];
            return (obj == null) ? string.Empty : obj.ToString();
        }
        set { ViewState[ClientID + "PatientID"] = value; }
    }

    /// <summary>
    /// US:1883
    /// event
    /// sets the height and width of the collection text box
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        tbCollection.Height = Height;
        tbCollection.Width = Width;
    }

    /// <summary>
    /// US:1883
    /// override
    /// loads the control with the values of the patient item specified
    /// by the properties CollectionItemID and PatientID
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;

        tbCollection.Text = string.Empty;

        CItemCollectionData ItemCollection = new CItemCollectionData(BaseMstr.BaseData);
        DataSet dsItems = null;
        CStatus status = ItemCollection.GetItemColMostRecentPatItemDS(CollectionItemID, PatientID, out dsItems);
        if (!status.Status)
        {
            return status;
        }

        DataSet dsItemComps = null;
        status = ItemCollection.GetItemColMostRecentPatICDS(CollectionItemID, PatientID, out dsItemComps);
        if (!status.Status)
        {
            return status;
        }

        foreach (DataRow drItem in dsItems.Tables[0].Rows)
        {
            tbCollection.Text += drItem["ITEM_LABEL"].ToString() + "\r\n";
            DataRow[] draComponents = dsItemComps.Tables[0].Select("ITEM_ID = " + drItem["ITEM_ID"].ToString());
            foreach (DataRow drComponent in draComponents)
            {
                if (Convert.ToInt64(drItem["ITEM_TYPE_ID"]) == (long)k_ITEM_TYPE_ID.QuestionSelection)
                {
                    // skip non selected items
                    if (drComponent["COMPONENT_VALUE"].ToString() == Convert.ToInt64(k_TRUE_FALSE_ID.False).ToString())
                    {
                        continue;
                    }

                    tbCollection.Text += drComponent["ITEM_COMPONENT_LABEL"].ToString() + "\r\n";
                }
                else
                {
                    tbCollection.Text += drComponent["ITEM_COMPONENT_LABEL"].ToString() + "\r\n";
                    tbCollection.Text += drComponent["COMPONENT_VALUE"].ToString() + "\r\n";
                }
            }
            tbCollection.Text += "\r\n";
        }

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
        tbCollection.Text = string.Empty;
    }
}
