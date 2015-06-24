using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.Composition;
using Box.Composition;
using Box.Composition.Web;
using System.Drawing;
using System.Drawing.Imaging;

namespace Box.CMS.Controllers {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CMS_CaptchaController : Controller {

        [Import]
        private Services.CaptchaService captcha { get; set; }

        public CaptchaResult Index() {

            captcha.GenerateNewText(6);

            return new CaptchaResult(captcha.CurrentText, captcha.ImageWidth, captcha.ImageHeight, captcha.FontSize, captcha.BackgroundColorName, captcha.TextColorName);
        }

    }

    public class CaptchaResult : ActionResult {
        private string _text;

        private Color backgroundColor = Color.Navy;
        private Color textColor = Color.WhiteSmoke;
        private int Width = 120;
        private int Height = 30;
        private int FontSize = 18;

        public CaptchaResult(string text, int? width, int? height, int? fontSize, string backgroundColorName = null, string textColorName = null) {
            _text = text;

            var rand = new Random((int)DateTime.Now.Ticks);

            if (width.HasValue)
                Width = width.Value;

            if (height.HasValue)
                Height = height.Value;

            if (fontSize.HasValue)
                FontSize = fontSize.Value;

            if (!String.IsNullOrEmpty(backgroundColorName))
                backgroundColor = Color.FromName(backgroundColorName);
            else
                backgroundColor = Color.FromArgb((rand.Next(0, 255)), (rand.Next(0, 255)), (rand.Next(0, 255)));

            if (!String.IsNullOrEmpty(textColorName))
                textColor = Color.FromName(textColorName);
            else
                textColor = Color.FromArgb((rand.Next(0, 255)), (rand.Next(0, 255)), (rand.Next(0, 255)));
        }

        public override void ExecuteResult(ControllerContext context) {



            Bitmap bmp = new Bitmap(Width, Height);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(backgroundColor);

            //add noise , if dont want any noisy , then make it false.
            bool noisy = true;
            var rand = new Random((int)DateTime.Now.Ticks);
            if (noisy) {                
                int i, r, x, y;
                var pen = new Pen(Color.Yellow);
                for (i = 1; i < Width*Height/360; i++) {
                    pen.Color = Color.FromArgb(
                    (rand.Next(0, 255)),
                    (rand.Next(0, 255)),
                    (rand.Next(0, 255)));

                    r = rand.Next(0, (Width / 3));
                    x = rand.Next(0, Width);
                    y = rand.Next(0, Height);

                    int m = x - r;
                    int n = y - r;
                    pen.Width = 3;
                    g.DrawEllipse(pen, m, n, r, r);
                }
            }
            //end noise

            var font = new Font("Arial", FontSize);
            var brush = new SolidBrush(textColor);
            var top = 3;
            var left = 5;

            
            for (int i = 0; i < _text.Length; i++) {

                int angle = rand.Next(30);
                g.TranslateTransform(left, top);
                g.RotateTransform(angle);
                g.TranslateTransform(-left, -top);
                g.DrawString(_text.Substring(0 + i, 1), font, brush, left, top);
                g.ResetTransform();

                left += 17;

                if (i % 2 == 0)
                    top = 7;
                else
                    top = 3;
            }


            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "image/jpeg";
            bmp.Save(response.OutputStream, ImageFormat.Jpeg);
            bmp.Dispose();

        }
    }

}
