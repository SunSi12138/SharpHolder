using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

[Route("api/[controller]")]
public class PlaceholderController: ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromQuery(Name="h")]int height,[FromQuery(Name="w")]int width)
    {
        using(var surface = SKSurface.Create(new SKImageInfo(width, height)))
        {
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);
            canvas.DrawText("Hello, SkiaSharp!", 20, 20, new SKPaint
            {
                Color = SKColors.Black,
                TextSize = 40
            });
            using(var image = surface.Snapshot())
            using(var data = image.Encode(SKEncodedImageFormat.Png, 100))
            {
                var bytes = data.ToArray();
                return new FileContentResult(bytes, "image/png");
            }
        }
    }
}