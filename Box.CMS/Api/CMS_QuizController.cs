using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Box.CMS.Services;
using Newtonsoft.Json.Linq;
using System.Web.Http;
using Box.Core.Services;

namespace Box.CMS.Api {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CMS_QuizController : ApiController{

        [Import]
        private CMSService cms { get; set; }

        [Import]
        private LogService log { get; set; }

        [Box.Core.Web.WebApiAntiForgery]
        public ContentHead Post(string id, [FromBody] int selectedAlternative) {

            ContentHead contentHead = cms.GetContent(id);
            cms.ParseContentData(contentHead);
            
            if(selectedAlternative == 1)
                contentHead.CONTENT.TotalVotes1 += 1;
            
            if(selectedAlternative == 2)
                contentHead.CONTENT.TotalVotes2 += 1;
            
            if(selectedAlternative == 3)
                contentHead.CONTENT.TotalVotes3 += 1;

            if(selectedAlternative == 4)
                contentHead.CONTENT.TotalVotes4 += 1;

            if(selectedAlternative == 5)
                contentHead.CONTENT.TotalVotes5 += 1;

            decimal total = SumOfAlternatives(contentHead.CONTENT);

            //Calculates percent for all alternatives
            contentHead.CONTENT.Percent1 = (contentHead.CONTENT.TotalVotes1 / total) * 100;
            contentHead.CONTENT.Percent2 = (contentHead.CONTENT.TotalVotes2 / total) * 100;
            contentHead.CONTENT.Percent3 = (contentHead.CONTENT.TotalVotes3 / total) * 100;
            contentHead.CONTENT.Percent4 = (contentHead.CONTENT.TotalVotes4 / total) * 100;
            contentHead.CONTENT.Percent5 = (contentHead.CONTENT.TotalVotes5 / total) * 100;

            contentHead.Data.JSON = contentHead.CONTENT.ToString(); 
          
            cms.SaveContent(contentHead);

            //PageviewCount means number of votes
            SiteService site = new Box.CMS.Services.SiteService();
            site.LogPageView(id);

            return contentHead;      
        }

        protected decimal SumOfAlternatives(dynamic json){
            return (decimal)json.TotalVotes1 + (decimal)json.TotalVotes2 + (decimal)json.TotalVotes3 + (decimal)json.TotalVotes4 + (decimal)json.TotalVotes5;
        }
    }
}
