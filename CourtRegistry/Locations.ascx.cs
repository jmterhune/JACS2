/*
' Copyright (c) 2025  Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Services.Exceptions;
using System;
using System.Linq;
using System.Web.UI.WebControls;
using tjc.Modules.CourtRegistry.Components;

namespace tjc.Modules.CourtRegistry
{
    public partial class Locations : CourtRegistryModuleBase
    {
        private void BindList()
        {
            var ctl = new LocationController();
            rptLocations.DataSource = ctl.GetLocations().OrderBy(l => l.LocationName);
            rptLocations.DataBind();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                    BindList();
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void rptLocations_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int.TryParse(e.CommandArgument.ToString(), out int locationId);
            var ctl = new LocationController();
            if (e.CommandName == "delete" && locationId > 0)
            {
                ctl.DeleteLocation(locationId);
                BindList();
            }
            else if (e.CommandName == "edit" && locationId > 0)
            {
                var loc = ctl.GetLocation(locationId);
                if (loc != null)
                {
                    hdLocationID.Value = loc.LocationID.ToString();
                    txtLocationName.Text = loc.LocationName;
                    txtAbbreviation.Text = loc.Abbreviation;
                    txtCountyNumber.Text = loc.CountyNumber.ToString();
                    ltModalScript.Text = "<script>(function(){function s(){if(typeof bootstrap!=='undefined'&&bootstrap.Modal){bootstrap.Modal.getOrCreateInstance(document.getElementById('locationModal')).show();}else if(typeof jQuery!=='undefined'){jQuery('#locationModal').modal('show');}}if(document.readyState!=='loading'){s();}else{document.addEventListener('DOMContentLoaded',s);}})();</script>";
                }
            }
        }

        protected void cmdSave_Click(object sender, EventArgs e)
        {
            var ctl = new LocationController();
            int.TryParse(txtCountyNumber.Text, out int countyNumber);
            if (int.TryParse(hdLocationID.Value, out int locationId) && locationId > 0)
            {
                var loc = ctl.GetLocation(locationId);
                loc.LocationName = txtLocationName.Text.Trim();
                loc.Abbreviation = txtAbbreviation.Text.Trim();
                loc.CountyNumber = countyNumber;
                ctl.UpdateLocation(loc);
            }
            else
            {
                ctl.CreateLocation(new Location
                {
                    LocationName = txtLocationName.Text.Trim(),
                    Abbreviation = txtAbbreviation.Text.Trim(),
                    CountyNumber = countyNumber
                });
            }
            BindList();
        }
    }
}
