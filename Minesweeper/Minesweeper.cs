using System;
using System.Diagnostics;
using System.Windows.Forms;
using Game.Core;
using Minesweeper.UI;
using NLog;

namespace Minesweeper
{
  static class Minesweeper
  {
    private static readonly Logger Log = LogManager.GetCurrentClassLogger();

    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      var pid = Process.GetCurrentProcess().Id;
      Log.Info("Process ID {0}", pid);
      
      var window = new MainWindow(pid);
      var engine = new GameEngine(window)
      {
        ApplicationEventHook = Application.DoEvents,
        LimitCpuUsage = true,
        UseWallTime = true,
        StartPaused = true
      };

      window.Show();
      engine.Run();
    }
  }
}
