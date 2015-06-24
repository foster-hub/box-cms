using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Newtonsoft.Json.Linq;

namespace Box.CMS.Services {

    public partial class CMSService {

        private CrossLinkArea[] crossLinkAreaCache = null;

        private string CrossLinkAreasConfigXMLPath {
            get {
                if (System.Web.HttpContext.Current == null)
                    return "";
                return System.Web.HttpContext.Current.Server.MapPath("~/App_Data/CMS/CrossLinks.config");
            }
        }

        private void GetCrossLinkAreas() {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(CrossLinkArea[]));
            using (var reader = System.Xml.XmlReader.Create(CrossLinkAreasConfigXMLPath)) {
                crossLinkAreaCache = serializer.Deserialize(reader) as CrossLinkArea[];
            }
            if (crossLinkAreaCache == null)
                crossLinkAreaCache = new CrossLinkArea[0];
        }

        public CrossLinkArea[] CrossLinkAreas {
            get {
                if (crossLinkAreaCache == null)
                    GetCrossLinkAreas();
                return crossLinkAreaCache;
            }
        }
    }

}
