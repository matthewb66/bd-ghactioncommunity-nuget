using System;
using System.Diagnostics;
using System.IO;
using SkiaSharp;

namespace Microsoft.Maui.Resizetizer
{
	internal abstract class SkiaSharpTools
	{
		public static SkiaSharpTools Create(bool isVector, string filename, SKSize? baseSize, SKColor? tintColor, ILogger logger)
			=> isVector
				? new SkiaSharpSvgTools(filename, baseSize, tintColor, logger) as SkiaSharpTools
				: new SkiaSharpBitmapTools(filename, baseSize, tintColor, logger);

		public SkiaSharpTools(ResizeImageInfo info, ILogger logger)
			: this(info.Filename, info.BaseSize, info.TintColor, logger)
		{
		}

		public SkiaSharpTools(string filename, SKSize? baseSize, SKColor? tintColor, ILogger logger)
		{
			Logger = logger;
			Filename = filename;
			BaseSize = baseSize;

			if (tintColor is SKColor tint)
			{
				Logger?.Log($"Detected a tint color of {tint}");

				Paint = new SKPaint
				{
					ColorFilter = SKColorFilter.CreateBlendMode(tint, SKBlendMode.SrcIn)
				};
			}
		}

		public string Filename { get; }

		public SKSize? BaseSize { get; }

		public ILogger Logger { get; }

		public SKPaint Paint { get; }

		public void Resize(DpiPath dpi, string destination)
		{
			var originalSize = GetOriginalSize();
			var (scaledSize, scale) = GetScaledSize(originalSize, dpi.Scale);

			var sw = new Stopwatch();
			sw.Start();

			// Allocate
			using (var tempBitmap = new SKBitmap(scaledSize.Width, scaledSize.Height))
			{
				// Draw (copy)
				using (var canvas = new SKCanvas(tempBitmap))
				{
					canvas.Clear(SKColors.Transparent);
					canvas.Save();
					canvas.Scale(scale, scale);
					DrawUnscaled(canvas, scale);
				}

				// Save (encode)
				using var stream = File.Create(destination);
				tempBitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
			}

			sw.Stop();
			Logger?.Log($"Save Image took {sw.ElapsedMilliseconds}ms ({destination})");
		}

		public abstract SKSize GetOriginalSize();

		public abstract void DrawUnscaled(SKCanvas canvas, float scale);

		public (SKSizeI, float) GetScaledSize(SKSize originalSize, DpiPath dpi)
		{
			if (dpi.Size.HasValue)
				return GetScaledSize(originalSize, dpi.Scale, dpi.Size.Value);
			else
				return GetScaledSize(originalSize, dpi.Scale);
		}

		(SKSizeI, float) GetScaledSize(SKSize originalSize, decimal scale, SKSize absoluteSize)
		{
			var ratio = Math.Min(
				(decimal)absoluteSize.Width / (decimal)originalSize.Width,
				(decimal)absoluteSize.Height / (decimal)originalSize.Height);

			return GetScaledSize(originalSize, ratio * scale);
		}

		public (SKSizeI, float) GetScaledSize(SKSize originalSize, decimal resizeRatio)
		{
			var sourceNominalWidth = (int)(BaseSize?.Width ?? originalSize.Width);
			var sourceNominalHeight = (int)(BaseSize?.Height ?? originalSize.Height);

			// Find the actual size of the image
			var sourceActualWidth = (double)originalSize.Width;
			var sourceActualHeight = (double)originalSize.Height;

			// Figure out what the ratio to convert the actual image size to the nominal size is
			var nominalRatio = Math.Max(sourceNominalWidth / sourceActualWidth, sourceNominalHeight / sourceActualHeight);

			// Multiply nominal ratio by the resize ratio to get our final ratio we actually adjust by
			var adjustRatio = nominalRatio * (double)resizeRatio;

			// Figure out our scaled width and height to make a new canvas for
			var scaledWidth = sourceActualWidth * adjustRatio;
			var scaledHeight = sourceActualHeight * adjustRatio;
			var scaledSize = new SKSizeI((int)Math.Round(scaledWidth), (int)Math.Round(scaledHeight));

			return (scaledSize, (float)adjustRatio);
		}
	}
}
