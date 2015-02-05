using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class sp_ucViewValuePopup : CAppUserControlPopup
{
    public string PatientID
    {
        get { return ucCollection.PatientID; }
        set
        {
            ucCollection.PatientID = value;
            ucNoteTitle.PatientID = value;
        }
    }

    public long ItemID
    {
        get { return ucCollection.CollectionItemID; }
        set
        {
            ucCollection.CollectionItemID = value;
            ucNoteTitle.ItemID = value;
        }
    }

    public long ItemTypeID
    {
        get
        {
            object obj = ViewState[ClientID + "ItemTypeID"];
            return (obj == null) ? -1 : Convert.ToInt64(obj);
        }
        set { ViewState[ClientID + "ItemTypeID"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        ucCollection.BaseMstr = BaseMstr;
        ucNoteTitle.BaseMstr = BaseMstr;

        if (!IsPostBack)
        {
            Title = "View Value(s)";
        }
    }

    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        EditMode = lEditMode;

        ucNoteTitle.Visible = false;
        ucCollection.Visible = false;

        CStatus status = null;
        switch (ItemTypeID)
        {
            case (long)k_ITEM_TYPE_ID.NoteTitle:
                ucNoteTitle.Visible = true;
                status = ucNoteTitle.LoadControl(EditMode);
                if (!status.Status)
                {
                    return status;
                }
                break;
            case (long)k_ITEM_TYPE_ID.Collection:
                ucCollection.Visible = true;
                status = ucCollection.LoadControl(EditMode);
                if (!status.Status)
                {
                    return status;
                }
                break;
            default:
                return new CStatus(false, k_STATUS_CODE.Failed, "TODO");
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
}
