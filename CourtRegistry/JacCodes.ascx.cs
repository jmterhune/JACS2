/*
' Copyright (c) 2025  Joe Terhune
'  All rights reserved.
*/

using DotNetNuke.Abstractions.Application;
using DotNetNuke.Services.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using tjc.Modules.CourtRegistry.Components;

namespace tjc.Modules.CourtRegistry
{
    public partial class JacCodes : CourtRegistryModuleBase
    {
        private readonly IHostSettings _hostSettings;

        public JacCodes()
        {
            _hostSettings = DependencyProvider.GetRequiredService<IHostSettings>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    var ctl = new JacCodeController(_hostSettings);
                    rptJacCodes.DataSource = ctl.GetJacCodes().OrderBy(j => j.JacCodeID);
                    rptJacCodes.DataBind();
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }
    }
}
