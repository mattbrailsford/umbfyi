using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text.RegularExpressions;
using Umb.Fyi.Models;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Extensions;

using Image = SixLabors.ImageSharp.Image;

namespace Umb.Fyi.Web.Controllers
{
    public class OgApiController : UmbracoApiController
    {
        private readonly IUmbracoContextFactory _umbracoContextFactory;
        private readonly IWebHostEnvironment _env;

        public OgApiController(
            IUmbracoContextFactory umbracoContextFactory,
            IWebHostEnvironment webHostEnvironment)
        {
            _umbracoContextFactory = umbracoContextFactory;
            _env = webHostEnvironment;
        }

        public Task<IActionResult> Image1080(string id)
            => GenImage(id, 1080, 1080, 100, 75, 50);

        public Task<IActionResult> Image1200x630(string id)
            => GenImage(id, 1200, 630, 60, 60, 40);

        public Task<IActionResult> Image1200x627(string id)
            => GenImage(id, 1200, 627, 60, 60, 40);

        public Task<IActionResult> Image1200x600(string id)
            => GenImage(id, 1200, 600, 60, 60, 40);

        private async Task<IActionResult> GenImage(string id, int imgWidth, int imgHeight, int padding, int titleFontSize, int subtitleFontSize)
        {
            using (var umbracoContext = _umbracoContextFactory.EnsureUmbracoContext())
            {
                var newsletter = umbracoContext.UmbracoContext.Content.GetByRoute($"/{Constants.Newsletter.ContainerAlias}/{id}/") as Newsletter;

                var image = new Image<Rgba32>(imgWidth, imgHeight);

                // BG
                var bg = await Image.LoadAsync(_env.MapPathWebRoot("/assets/bg_og.png"));
                image.Mutate(ctx => ctx.DrawImage(bg, 1));

                // Logo
                var logoImage = await Image.LoadAsync(_env.MapPathWebRoot("/assets/umbfyi_white_450.png"));
                image.Mutate(ctx => ctx.DrawImage(logoImage, new Point(padding, padding), 1));

                // Icon
                var iconImage = await Image.LoadAsync(_env.MapPathWebRoot("/assets/rocket_90.png"));
                image.Mutate(ctx => ctx.DrawImage(iconImage, new Point(imgWidth - padding - 90, padding), 1));

                var latoFontCollection = new FontCollection();
                latoFontCollection.Add(_env.MapPathWebRoot("/fonts/lato/Lato-Black.ttf"));
                latoFontCollection.Add(_env.MapPathWebRoot("/fonts/lato/Lato-Bold.ttf"));
                latoFontCollection.Add(_env.MapPathWebRoot("/fonts/lato/Lato-Regular.ttf"));

                if (latoFontCollection.TryGet("Lato", out FontFamily latoFont))
                {
                    // Title
                    var titleFont = latoFont.CreateFont(titleFontSize, FontStyle.Bold);

                    var titleTextOpts = new TextOptions(titleFont)
                    {
                        Origin = new System.Numerics.Vector2(padding, (int)Math.Floor(imgHeight - (padding * 1.5) - subtitleFontSize)),
                        WrappingLength = imgWidth - (padding * 2),
                        LineSpacing = 1.15f,
                        VerticalAlignment = VerticalAlignment.Bottom
                    };

                    var titleGlyphs = TextBuilder.GenerateGlyphs(Regex.Replace(newsletter.Preheader, @"\p{Cs}", ""), titleTextOpts);

                    image.Mutate(ctx => ctx.Fill(Color.White, titleGlyphs));

                    // Url
                    var urlFont = latoFont.CreateFont(subtitleFontSize, FontStyle.Bold);

                    var urlTextOpts = new TextOptions(urlFont)
                    {
                        Origin = new System.Numerics.Vector2(padding, imgHeight - padding),
                        LineSpacing = 1.2f,
                        VerticalAlignment = VerticalAlignment.Bottom
                    };

                    var urlGlyphs = TextBuilder.GenerateGlyphs(newsletter.Url(mode: Umbraco.Cms.Core.Models.PublishedContent.UrlMode.Absolute).TrimEnd('/'), urlTextOpts);

                    image.Mutate(ctx => ctx.Fill(Color.FromRgb(245, 193, 188), urlGlyphs));
                }

                // Generate stream
                var ms = new MemoryStream();
                image.SaveAsPng(ms);
                ms.Position = 0;

                return File(ms, "image/png");

            }
        }
    }
}
