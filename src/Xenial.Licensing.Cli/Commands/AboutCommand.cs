using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SkiaSharp;

using TrueColorConsole;

using static TrueColorConsole.VTConsole;

namespace Xenial.Licensing.Cli.Commands
{
    public class AboutCommand : XenialDefaultCommand
    {
    }

    [XenialCommandHandler("about")]
    public class AboutCommandHandler : XenialCommandHandler<AboutCommand>
    {
        protected override Task<int> ExecuteCommand(AboutCommand arguments)
        {
            if (VTConsole.IsSupported && !VTConsole.IsEnabled)
            {
                VTConsole.Enable();
            }
            var assembly = typeof(AboutCommandHandler).Assembly;

            using var pngStream = assembly.GetManifestResourceStream(
                $"{typeof(AboutCommandHandler).Assembly.GetName().Name}.Images.icon-64x64.png"
            );

            VTConsole.SetConsoleWidth80();

            using var bitmap = SKBitmap.Decode(pngStream);

            var builder = new StringBuilder(bitmap.Width * bitmap.Height * 22);

            for (var j = 0; j < bitmap.Height; j = j + 2)
            {
                for (var i = 0; i < bitmap.Width; i++)
                {
                    var pixel = bitmap.GetPixel(i, j);
                    var str = VTConsole.GetColorBackgroundString(pixel.Red, pixel.Green, pixel.Blue);
                    builder.Append(str);
                    builder.Append(' ');
                }
                var backStr = VTConsole.GetColorBackgroundString(0, 0, 0);
                builder.AppendLine(backStr);
            }

            var bytes = Encoding.ASCII.GetBytes(builder.ToString());
            VTConsole.WriteFast(bytes);

            return Task.FromResult(0);
        }
    }
}
