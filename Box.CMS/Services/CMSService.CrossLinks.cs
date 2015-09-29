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

        public CrossLink GetCrossLink(string contentUId, string area) {
            using (var context = new Data.CMSContext()) {
                return context.CrossLinks.SingleOrDefault(c => c.ContentUId == contentUId && c.PageArea == area);
            }
        }

        public void RemoveCrossLink(string contentUId, string area) {
            using (var context = new Data.CMSContext()) {
                CrossLink link = context.CrossLinks.SingleOrDefault(c => c.ContentUId == contentUId && c.PageArea == area);
                if (link == null)
                    return;

                int order = link.DisplayOrder;
                
                context.CrossLinks.Remove(link);
                context.SaveChanges();

                // update orders
                context.Database.ExecuteSqlCommand(
                    "UPDATE CrossLinks SET DisplayOrder = DisplayOrder - 1 WHERE PageArea = {0} AND DisplayOrder > {1}", area, order);
                

            }
        }

        /// <summary>
        /// Adds a news crosslink for the contect at the given area.
        /// </summary>
        /// <param name="contentUId">The content Id</param>
        /// <param name="area">The crosslink area</param>
        /// <param name="changeDisplayOrderBy">Used to change the crosslink display order</param>
        /// <returns></returns>
        public void AddCrossLink(string contentUId, string area, short changeDisplayOrderBy = 0) {

            short oldOrder = 0;
                        
            using (var context = new Data.CMSContext()) {

                // max crosslink order
                short maxOrder = -1;
                if (context.CrossLinks.Any(c => c.PageArea == area)) {
                    maxOrder = context.CrossLinks.Where(c => c.PageArea == area).Select(c => c.DisplayOrder).DefaultIfEmpty().Max();
                }

                CrossLink link = context.CrossLinks.SingleOrDefault(c => c.ContentUId == contentUId && c.PageArea == area);

                if (link==null) {
                    link = new CrossLink() { ContentUId = contentUId, PageArea = area, DisplayOrder = (short)(maxOrder + 1) };
                    context.CrossLinks.Add(link);
                }

                // calcs the new crosslink order
                oldOrder = link.DisplayOrder;
                short order = (short)(link.DisplayOrder + changeDisplayOrderBy);

                // if is a order chage and it its ut of bounds, get out of here
                if (changeDisplayOrderBy < 0 && oldOrder == 0)
                    return;
                if (changeDisplayOrderBy > 0 && oldOrder == maxOrder)
                    return;

                // set the new order
                link.DisplayOrder = order;

                // change the other link display order
                CrossLink link2 = null;
                link2 = context.CrossLinks.SingleOrDefault(c => c.ContentUId != contentUId && c.PageArea == area && c.DisplayOrder == order);
                if (link2!=null)
                    link2.DisplayOrder = oldOrder;
                
                context.SaveChanges();

            }
        }


        internal void ApplyCollectionValuesCrossLinks(ICollection<CrossLink> oldCollection, ICollection<CrossLink> newCollection) {
            if (oldCollection == null)
                oldCollection = new List<CrossLink>();
            if (newCollection == null)
                newCollection = new List<CrossLink>();
            var removed = oldCollection.Where(o => !newCollection.Any(n => n.PageArea == o.PageArea)).ToArray();
            var added = newCollection.Where(n => !oldCollection.Any(o => n.PageArea == o.PageArea)).ToArray();

            foreach (var r in removed)
                RemoveCrossLink(r.ContentUId, r.PageArea);

            foreach (var a in added)
                AddCrossLink(a.ContentUId, a.PageArea);
        }
    }

}
