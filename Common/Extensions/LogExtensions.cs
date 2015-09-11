using System;
using System.Reflection;
using log4net;
using log4net.Core;

namespace Common.Extensions
{
  /// <summary>
  ///   Extensions to help with log4net logging.
  /// </summary>
  public static class LogExtensions
  {
    /// <summary>
    ///   Logs a message at the Verbose level.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="message"></param>
    public static void Verbose(this ILog log, string message)
    {
      log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType,
        Level.Verbose, message, null);
    }

    /// <summary>
    ///   Logs an exception at the Verbose level.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="exception"></param>
    public static void Verbose(this ILog log, Exception exception)
    {
      log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType,
        Level.Verbose, null, exception);
    }

    /// <summary>
    ///   Logs a formatted message at the Verbose level.  Formatting is not 
    ///   performed if the level is disabled.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="format"></param>
    /// <param name="args"></param>
    public static void VerboseFmt(this ILog log, string format, 
      params object[] args)
    {
      if (!log.Logger.IsEnabledFor(Level.Verbose))
      {
        return;
      }

      var msg = string.Format(format, args);
      log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType,
        Level.Verbose, msg, null);
    }

    /// <summary>
    ///   Logs a formatted message at the Verbose level if the condition is 
    ///   true.  Formatting is not performed if the level is disabled.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="condition"></param>
    /// <param name="format"></param>
    /// <param name="args"></param>
    public static void VerboseFmtIf(this ILog log, bool condition,
      string format, params object[] args)
    {
      if (condition)
      {
        log.VerboseFmt(format, args);
      }
    }

    /// <summary>
    ///   Logs a formatted message at the Debug level.  Formatting is not 
    ///   performed if the level is disabled.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="format"></param>
    /// <param name="args"></param>
    public static void DebugFmt(this ILog log, string format,
      params object[] args)
    {
      if (!log.Logger.IsEnabledFor(Level.Debug))
      {
        return;
      }

      var msg = string.Format(format, args);
      log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType,
        Level.Debug, msg, null);
    }

    /// <summary>
    ///   Logs a formatted message at the Debug level if the condition is true.
    ///   Formatting is not performed if the level is disabled.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="condition"></param>
    /// <param name="format"></param>
    /// <param name="args"></param>
    public static void DebugFmtIf(this ILog log, bool condition,
      string format, params object[] args)
    {
      if (condition)
      {
        log.DebugFmt(format, args);
      }
    }

    /// <summary>
    ///   Logs a formatted at the Info level.  Formatting is not performed if 
    ///   the level is disabled.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="format"></param>
    /// <param name="args"></param>
    public static void InfoFmt(this ILog log, string format,
      params object[] args)
    {
      if (!log.Logger.IsEnabledFor(Level.Info))
      {
        return;
      }

      var msg = string.Format(format, args);
      log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType,
        Level.Info, msg, null);
    }

    /// <summary>
    ///   Logs a formatted message at the Info level if the condition is true.
    ///   Formatting is not performed if the level is disabled.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="condition"></param>
    /// <param name="format"></param>
    /// <param name="args"></param>
    public static void InfoFmtIf(this ILog log, bool condition,
      string format, params object[] args)
    {
      if (condition)
      {
        log.InfoFmt(format, args);
      }
    }

    /// <summary>
    ///   Logs a formatted message at the Warn level.  Formatting is not 
    ///   performed if the level is disabled.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="format"></param>
    /// <param name="args"></param>
    public static void WarnFmt(this ILog log, string format,
      params object[] args)
    {
      if (!log.Logger.IsEnabledFor(Level.Warn))
      {
        return;
      }

      var msg = string.Format(format, args);
      log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType,
        Level.Warn, msg, null);
    }

    /// <summary>
    ///   Logs a formatted message at the Warn level if the condition is true.
    ///   Formatting is not performed if the level is disabled.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="condition"></param>
    /// <param name="format"></param>
    /// <param name="args"></param>
    public static void WarnFmtIf(this ILog log, bool condition,
      string format, params object[] args)
    {
      if (condition)
      {
        log.WarnFmt(format, args);
      }
    }

    /// <summary>
    ///   Logs a formatted message at the Error level.  Formatting is not 
    ///   performed if the level is disabled.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="format"></param>
    /// <param name="args"></param>
    public static void ErrorFmt(this ILog log, string format,
      params object[] args)
    {
      if (!log.Logger.IsEnabledFor(Level.Error))
      {
        return;
      }

      var msg = string.Format(format, args);
      log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType,
        Level.Error, msg, null);
    }

    /// <summary>
    ///   Logs a formatted message at the Error level if the condition is true.
    ///   Formatting is not performed if the level is disabled.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="condition"></param>
    /// <param name="format"></param>
    /// <param name="args"></param>
    public static void ErrorFmtIf(this ILog log, bool condition,
      string format, params object[] args)
    {
      if (condition)
      {
        log.ErrorFmt(format, args);
      }
    }

    /// <summary>
    ///   Logs a formatted message at the Fatal level.  Formatting is not 
    ///   performed if the level is disabled.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="format"></param>
    /// <param name="args"></param>
    public static void FatalFmt(this ILog log, string format,
      params object[] args)
    {
      if (!log.Logger.IsEnabledFor(Level.Fatal))
      {
        return;
      }

      var msg = string.Format(format, args);
      log.Logger.Log(MethodBase.GetCurrentMethod().DeclaringType,
        Level.Fatal, msg, null);
    }

    /// <summary>
    ///   Logs a formatted message at the Fatal level if the condition is true.
    ///   Formatting is not performed if the level is disabled.
    /// </summary>
    /// <param name="log"></param>
    /// <param name="condition"></param>
    /// <param name="format"></param>
    /// <param name="args"></param>
    public static void FatalFmtIf(this ILog log, bool condition,
      string format, params object[] args)
    {
      if (condition)
      {
        log.FatalFmt(format, args);
      }
    }
  }
}
