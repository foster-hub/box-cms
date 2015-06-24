using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Box.CMS.Web {

    public class ContentPageModel : Box.Composition.Web.IPageModel {

        public ContentPageModel(Box.Composition.Web.IPageModel pageModel, ContentKind kind, CrossLinkArea[] crossLinkAreas) {
            this.SettingsMenuItems = pageModel.SettingsMenuItems;
            this.TopMenuItems = pageModel.TopMenuItems;
            this.UserDisplayName = pageModel.UserDisplayName;
            this.UserEmail = pageModel.UserEmail;
            this.ContentKind = kind;
            this.CrossLinkAreas = crossLinkAreas;
        }

        public CrossLinkArea[] CrossLinkAreas {
            get;
            private set;
        }

        public ContentKind ContentKind {
            get;
            private set;
        }

        public ICollection<Composition.IMenuActionLink> SettingsMenuItems {
            get;
            private set;
        }

        public ICollection<Composition.IMenuActionLink> TopMenuItems {
            get;
            private set;
        }

        public string UserDisplayName {
            get;
            private set;
        }

        public string UserEmail { get; private set; }
    }
}
