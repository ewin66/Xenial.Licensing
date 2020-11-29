using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Colorful;

using static Colorful.Console;

namespace Xenial.Licensing.Cli.Services.Default
{
    internal class Consts
    {
        internal const string Header = @"__   __          _       _ 
\ \ / /         (_)     | |
 \ V / ___ _ __  _  __ _| |
  > < / _ \ '_ \| |/ _` | |
 / . \  __/ | | | | (_| | |
/_/ \_\___|_| |_|_|\__,_|_|
                           ";

        internal static Color DarkBlue { get; } = Color.FromArgb(0, 80, 106);
        internal static Color LightBlue { get; } = Color.FromArgb(56, 188, 216);
        internal static Color Orange { get; } = Color.FromArgb(243, 160, 39);
        internal static Color White { get; } = Color.White;

        public static void WriteHeader()
        {
            using var fontStream = typeof(Consts).Assembly.GetManifestResourceStream(
                $"{typeof(Consts).Assembly.GetName().Name}.Fonts.big.flf"
            );

            var font = FigletFont.Load(fontStream);
            var figlet = new Figlet(font);

            void WriteSegments((string str, Color color)[] columns)
            {
                var cols = columns.Select(c => (figlet.ToAscii(c.str).ToString(), c.color));
                var rows = cols.Select(c => (c.Item1.Split(Environment.NewLine), c.color)).ToArray();

                IEnumerable<(string str, Color color)> Iter()
                {
                    var segCount = rows.Length;
                    var maxRowCount = rows.Select(r => r.Item1.Length).Max();

                    for (var i = 0; i < maxRowCount; i++)
                    {
                        for (var j = 0; j < segCount; j++)
                        {
                            var row = rows[j].Item1[i];
                            var color = rows[j].color;
                            yield return (row, color);
                        }
                        yield return (Environment.NewLine, DarkBlue);
                    }

                    yield break;
                }

                foreach (var row in Iter())
                {
                    Write(row.str, row.color);
                }
            }

            WriteSegments(new[] { ("Xenial", LightBlue), (".", White), ("io", Orange) });

            WriteLine();
        }
    }
}
