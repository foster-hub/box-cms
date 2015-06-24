using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Box.Composition;
using System.ComponentModel.Composition;
using Box.CMS.Services;

namespace Box.CMS.Api {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CMS_CommentsController : ApiController {

        [Import]
        private CMSService cms { get; set; }

        [Box.Core.Web.WebApiAntiForgery]
        public IEnumerable<ContentComment> Get(string id, int afterPosition = 0, int beforePosition = 0, int top = 0, string order = "DESC", bool includeCensored = false) {            
            return cms.GetComments(id, afterPosition, beforePosition, top, order, includeCensored);
        }

        [Box.Core.Web.WebApiAntiForgery]
        public IEnumerable<ContentComment> Post([FromBody] ContentComment comment, bool returnNewPosts = false, int lastCommentPosition = 0, string order = "DESC") {

            comment.CommentUId = Guid.NewGuid().ToString();
            comment.CommentDate = DateTime.Now.ToUniversalTime();
            
            cms.SaveComment(comment);

            if(!returnNewPosts)
                return new ContentComment[] { comment };
            
            return cms.GetComments(comment.ContentUId, lastCommentPosition, 0, 0, order,  false);
            
        }


    }
}
