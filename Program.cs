using SkiaSharp;
using Microsoft.AspNetCore.Diagnostics;
using System.Text.RegularExpressions;

var builder = WebApplication.CreateSlimBuilder(args);
var app = builder.Build();

var random = new Random();
var opaqueImageInfo = new SKImageInfo(width:1920,height:1080,alphaType: SKAlphaType.Opaque, colorType: SKColorType.Rgb888x);
var hexColorPattern = "([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$";
var hexColorRegex = new Regex(hexColorPattern);

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        if (exception is BadHttpRequestException badHttpRequestException)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync(badHttpRequestException.Message);
        }
    });
});
app.MapGet("/placeholder", GetHandler);

app.Run();



SKColor GetComplementaryColor (SKColor color) {
    color.ToHsl(out var h, out var s, out var l);
    h = (h + 180) % 360;
    l = l > 50 ? l - 50 : l + 50;
    return SKColor.FromHsl(h, s, l);
}

SKColor getRandomColor(){
    return SKColor.FromHsl(random.Next(0, 360), 50, 80);
}


bool IsValidHexColor(string? colorStr) {
    if(colorStr is null)
    {
        return true;
    }
    return hexColorRegex.IsMatch(colorStr);
};

async Task<IResult> GetHandler (string? b,string? f,string? t,string? e,int? d,int q=100,int h=1080, int w=1920)
{
    if(!IsValidHexColor(b) || !IsValidHexColor(f))
    {
        return Results.StatusCode(400);
    }

    if (w <= 0 || h <= 0 || q<0 || q>100 || d<0 || d>10000)
    {
        return Results.StatusCode(400);
    }

    var trueImageInfo = opaqueImageInfo.WithSize(w, h);

    using (var surface = SKSurface.Create(trueImageInfo))
    {
        var canvas = surface.Canvas;
        var paint = new SKPaint();
        var bgColor = string.IsNullOrEmpty(b) ? getRandomColor() : SKColor.Parse(b);
        var textColor = string.IsNullOrEmpty(f) ? GetComplementaryColor(bgColor) : SKColor.Parse(f);
        var trueText =t ?? $"{w}x{h}";
        canvas.Clear(bgColor);

        paint.Color = textColor;
        paint.TextSize = w/trueText.Length;

        var textBounds = new SKRect();
        paint.MeasureText(trueText, ref textBounds);
        var x = (w - textBounds.Width) / 2;
        var y = (h + textBounds.Height) / 2;
        canvas.DrawText(trueText, x, y, paint);

        var encode = e is null? SKEncodedImageFormat.Png : Enum.TryParse<SKEncodedImageFormat>(e, true, out var format) ? format : SKEncodedImageFormat.Png;
        
        using (var image = surface.Snapshot())
        using (var data = image.Encode(encode, q))
        {   
            if(d.HasValue)
            {
                await Task.Delay(d.Value);
            }
            return Results.File(data.ToArray(), $"image/{encode.ToString().ToLower()}");
        }
    }
}
