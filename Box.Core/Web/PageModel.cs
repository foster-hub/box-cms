﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Box.Composition;
using Box.Composition.Web;
using System.ComponentModel.Composition;

namespace Box.Core.Web {

    [Export(typeof(IPageModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PageModel : IPageModel {


        public ICollection<IMenuActionLink> SettingsMenuItems { get; private set; }
        public ICollection<IMenuActionLink> TopMenuItems { get; private set; }

        public string UserDisplayName { get; private set; }
        public string UserEmail { get; private set; }
        
        [ImportingConstructor]
        public PageModel([Import]Box.Composition.Services.IMenuService menus, [Import] Services.LogService log) {
            TopMenuItems = menus.TopMenuItems;
            SettingsMenuItems = menus.SettingsMenuItems;

            if (System.Threading.Thread.CurrentPrincipal is BoxPrincipal) {
                UserDisplayName = ((BoxPrincipal)System.Threading.Thread.CurrentPrincipal).DisplayName;
                UserEmail = ((BoxPrincipal)System.Threading.Thread.CurrentPrincipal).Email;
            } else {
                UserDisplayName = "";
                UserEmail = "";
                log.Log("Make sure /ADM application Windows Authetication is OFF.\n Only /ADM_NT folder should have Windows Authetication ON.", "Invalid Current Principal: " + System.Threading.Thread.CurrentPrincipal.ToString());
            }
        }
    }
}