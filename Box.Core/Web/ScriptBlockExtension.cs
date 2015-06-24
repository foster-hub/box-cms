using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Web.WebPages;

namespace Box.Core.Web {

    public static class ScriptBlockExtension {

        private const string SCRIPTBLOCK_BUILDER = "ScriptBlockBuilder";

        public static MvcHtmlString ScriptBlock(this WebViewPage webPage, Func<dynamic, HelperResult> template) {
            if (webPage.IsAjax)
                return new MvcHtmlString(template(null).ToHtmlString());
            
            var scriptBuilder = webPage.Context.Items[SCRIPTBLOCK_BUILDER] as StringBuilder ?? new StringBuilder();

            scriptBuilder.Append(template(null).ToHtmlString());
            webPage.Context.Items[SCRIPTBLOCK_BUILDER] = scriptBuilder;

            return new MvcHtmlString(string.Empty);
            
        }

        public static MvcHtmlString RenderScriptBlocks(this WebViewPage webPage) {
            var scriptBuilder = webPage.Context.Items[SCRIPTBLOCK_BUILDER] as StringBuilder ?? new StringBuilder();
            return new MvcHtmlString(scriptBuilder.ToString());
        }
    }
}