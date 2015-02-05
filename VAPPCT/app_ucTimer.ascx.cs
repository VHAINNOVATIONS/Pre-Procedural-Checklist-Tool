using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using VAPPCT.DA;
using VAPPCT.UI;

public partial class app_ucTimer : CAppUserControl
{
    const int k_nINTERVAL_MULTIPLIER = 60000;

    protected event EventHandler<EventArgs> _Refresh;
    /// <summary>
    /// property
    /// adds/removes event handlers to the refresh event
    /// </summary>
    public event EventHandler<EventArgs> Refresh
    {
        add { _Refresh += new EventHandler<EventArgs>(value); }
        remove { _Refresh -= value; }
    }

    /// <summary>
    /// event
    /// initializes the user control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            lblLastUpdated.Text = Convert.ToString(DateTime.Now);
            OnSelChangeInterval(sender, e);
        }
    }

    /// <summary>
    /// method
    /// US:879
    /// updates the time labels and fires a refresh event
    /// </summary>
    protected void UpdateLabels()
    {
        lblLastUpdated.Text = Convert.ToString(DateTime.Now);
        lblNextUpdate.Text = Convert.ToString(DateTime.Now + TimeSpan.FromMinutes(Convert.ToDouble(ddlTimeInterval.SelectedValue)));
    }

    /// <summary>
    /// event
    /// US:879
    /// updates last/next labels and fires the refresh event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnTickRefresh(object sender, EventArgs e)
    {
        UpdateLabels();

        if (_Refresh != null)
            _Refresh(this, new EventArgs());
    }

    /// <summary>
    /// event
    /// US:879
    /// updates last/next labels and fires the refresh event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnClickRefresh(object sender, EventArgs e)
    {
        UpdateLabels();

        if (_Refresh != null)
            _Refresh(this, new EventArgs());
    }

    /// <summary>
    /// event
    /// US:879
    /// sets the interval to the selected value
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void OnSelChangeInterval(object sender, EventArgs e)
    {
        timRefreshTimer.Interval = Convert.ToInt32(ddlTimeInterval.SelectedValue) * k_nINTERVAL_MULTIPLIER;
        lblNextUpdate.Text = Convert.ToString(DateTime.Now + TimeSpan.FromMinutes(Convert.ToInt32(ddlTimeInterval.SelectedValue)));
    }

    /// <summary>
    /// method
    /// US:879
    /// stops the timer
    /// </summary>
    public void StopRefresh()
    {
        timRefreshTimer.Enabled = false;
    }

    public void StartRefresh()
    {
        StartRefresh(true);
    }
    
    public void StartRefresh(bool bRefresh)
    {
        timRefreshTimer.Enabled = true;
        if (bRefresh)
        {
            UpdateLabels();

            if (_Refresh != null)
                _Refresh(this, new EventArgs());
        }
    }

    /// <summary>
    /// override
    /// does nothing
    /// </summary>
    /// <param name="lEditMode"></param>
    /// <returns></returns>
    public override CStatus LoadControl(k_EDIT_MODE lEditMode)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// override
    /// does nothing
    /// </summary>
    /// <returns></returns>
    public override CStatus SaveControl()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// override
    /// does nothing
    /// </summary>
    /// <param name="plistStatus"></param>
    /// <returns></returns>
    public override CStatus ValidateUserInput(out CParameterList plistStatus)
    {
        throw new NotImplementedException();
    }
}
