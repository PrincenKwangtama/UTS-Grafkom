using OpenTK.Windowing.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tes
{
    internal class Program
    {
        static void Main(String[] args)
        {
            var nativeWindowSttings = new NativeWindowSettings()
            {
                Size = new OpenTK.Mathematics.Vector2i(800, 800),
                Title = "Pertemuan 1"
            };
            using (var window = new Window(GameWindowSettings.Default,
                nativeWindowSttings))
            {
                window.Run();
            }
        }
    }
}