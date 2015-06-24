using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Newtonsoft.Json.Linq;

namespace Box.CMS.Services {

    public partial class CMSService {

        public IEnumerable<ContentComment> GetComments(string id, int afterPosition, int beforePosition, int top, string order, bool includeCensored) {
            using (var context = new Data.CMSContext()) {
                var comments = context.ContentComments.Where(c => c.ContentUId == id);

                if (includeCensored)
                    comments = comments.Where(c => c.Status == 1);

                if(order!=null && order.ToUpper()=="DESC")
                    comments = comments.OrderByDescending(c => c.Position);
                else
                    comments = comments.OrderBy(c => c.Position);

                if (afterPosition != -1)
                    comments = comments.Where(c => c.Position > afterPosition);
                
                if (beforePosition != -1)
                    comments = comments.Where(c => c.Position < beforePosition);

                if(top!=0)
                    comments = comments.Take(top);

                return comments.ToArray();

            }
        }

        public ContentComment SaveComment(ContentComment comment) {

            using (var context = new Data.CMSContext()) {

                ContentComment oldComment = context.ContentComments.SingleOrDefault(c => c.CommentUId == comment.CommentUId);

                if (oldComment == null) {                    
                    context.ContentComments.Add(comment);
                    int? position = context.ContentComments.Where(c => c.ContentUId == comment.ContentUId).Max(c => (int?) c.Position);
                    if (position == null)
                        comment.Position = 0;
                    else
                        comment.Position = position.Value + 1;
                }
                else {
                    context.ContentComments.Attach(oldComment);
                    context.Entry<ContentComment>(oldComment).CurrentValues.SetValues(comment);
                }

                context.SaveChanges();

                IncreaseCommentCount(comment);

                return comment;
            }
        }

        private void IncreaseCommentCount(ContentComment comment) {
            using (var context = new Data.CMSContext()) {
                ContentCommentCount oldCount = context.ContentCommentCounts.SingleOrDefault(c => c.ContentUId == comment.ContentUId);
                if (oldCount == null)
                    context.ContentCommentCounts.Add(new ContentCommentCount() { ContentUId = comment.ContentUId, Count = 1 });
                else
                    oldCount.Count++;
                context.SaveChanges();
            }

        }

        

    }

}
