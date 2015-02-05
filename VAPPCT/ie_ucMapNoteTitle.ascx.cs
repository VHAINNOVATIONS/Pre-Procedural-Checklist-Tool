using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;

//our data and ui libraries
using VAPPCT.DA;
using VAPPCT.UI;

/// <summary>
/// user control used to map to an item to a note title
/// </summary>
public partial class ie_ucMapNoteTitle : CAppUserControlPopup
{
    protected event EventHandler<CAppUserControlArgs> _SelectNoteTitle;
    /// <summary>
    /// US:1945 US:1880 property
    /// adds/removes event handlers to the select event
    /// </summary>
    public event EventHandler<CAppUserControlArgs> SelectNoteTitle
    {
        add { _SelectNoteTitle += new EventHandler<CAppUserControlArgs>(value); }
        remove { _SelectNoteTitle -= value; }
    }

    /// <summary>
    /// US:1945 US:1880 returns the text of the item selected in the note title list box
    /// </summary>
    public string SelectedText
    {
        get { return lbNoteTitles.SelectedItem.Text; }
    }

    /// <summary>
    /// US:1945 US:1880 returns the value of the item selected in the note title list box
    /// </summary>
    public string SelectedValue
    {
        get { return lbNoteTitles.SelectedValue; }
    }

    /// <summary>
    /// US:1945 US:1880 page load for the user control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            //Set the title
            Title = "Map Note Title";
        }
    }

    /// <summary>
    /// override
    /// does nothing
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// override
    /// does nothing
    /// </summary>
    /// <param name="lStatusCode"></param>
    /// <param name="strStatusComment"></param>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// US:1945 US:1880 override
    /// Loads the note titles listbox with all note tiles
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        CStatus status = new CStatus();

        if (lbNoteTitles.Items.Count < 1)
        {
            CNoteTitleData ntd = new CNoteTitleData(BaseMstr.BaseData);
            DataSet dsNoteTitles = null;
            status = ntd.GetNoteTitleDS(out dsNoteTitles);
            if (status.Status)
            {
                lbNoteTitles.DataTextField = "note_title_label";
                //note: the tag is not unique so it cannot be used
                //as the datavaluefield, will map incorrectly...
                //lbNoteTitles.DataValueField = "note_title_tag";
                lbNoteTitles.DataValueField = "note_title_label";
                lbNoteTitles.DataSource = dsNoteTitles;
                lbNoteTitles.DataBind();
            }
            else
            {
                ShowStatusInfo(status);
            }
        }

        txtSearchOptions.Focus();
        return status;
    }

    /// <summary>
    /// US:1945 US:1880 search the list of note titles
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSearchOptions_Click(object sender, EventArgs e)
    {
        //re-load the note titles
        lbNoteTitles.Items.Clear();

        CNoteTitleData ntd = new CNoteTitleData(BaseMstr.BaseData);
        DataSet dsNoteTitles = null;
        CStatus status = ntd.GetNoteTitleDS(out dsNoteTitles);
        if (!status.Status)
        {
            ShowStatusInfo(status);
            return;
        }

        lbNoteTitles.DataTextField = "note_title_label";
        //note: the tag is not unique so it cannot be used
        //as the datavaluefield, will map incorrectly...
        //lbNoteTitles.DataValueField = "note_title_tag";
        lbNoteTitles.DataValueField = "note_title_label";
        lbNoteTitles.DataSource = dsNoteTitles;
        lbNoteTitles.DataBind();

        //filter the listbox using helper
        CListBox clb = new CListBox();
        clb.FilterListBox(lbNoteTitles, txtSearchOptions.Text);

        //show the parents MPE and then this controls MPE
        ShowMPE();
    }

    /// <summary>
    /// US:1945 US:1880 save the selected note title to the note title property and fire the 
    /// SelectNoteTitle property.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSelect_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(lbNoteTitles.SelectedValue))
        {
            ShowMPE();
            return;
        }

        //fire the SelectNoteTitle event for the parent to handle
        if (_SelectNoteTitle != null)
        {
            CAppUserControlArgs args = new CAppUserControlArgs(
                k_EVENT.SELECT,
                k_STATUS_CODE.Success,
                string.Empty,
                lbNoteTitles.SelectedValue);
            _SelectNoteTitle(this, args);
        }

        ShowParentMPE();
    }
    
    /// <summary>
    /// US:1945 US:1880 Cancel the note title dialog
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        ShowParentMPE();
    }
}