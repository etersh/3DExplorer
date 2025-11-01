using System;

namespace Game;
class Program
{
    static void Main(string[] args)
    {
        using var win = new GameWindowEx(1280, 720, "Mini 3D Explorer");
        win.Run();
    }
}
