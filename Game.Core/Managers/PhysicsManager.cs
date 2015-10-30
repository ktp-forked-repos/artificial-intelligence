using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Common.Extensions;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Game.Core.Interfaces;
using log4net;
using Microsoft.Xna.Framework;

namespace Game.Core.Managers
{
  /// <summary>
  ///   Standard Farseer physics manager implementation.
  /// </summary>
  /// <remarks>
  ///   No unit testing here because of the difficulty of mocking the Farseer
  ///   World.
  /// </remarks>
  [ExcludeFromCodeCoverage]
  public class PhysicsManager
    : IPhysicsManager
  {
    private static readonly ILog Log = LogManager.GetLogger(
      MethodBase.GetCurrentMethod().DeclaringType);

    // drive the physics world at a fixed 60 fps
    public const float UpdateInterval = 1f / 60f;

    private float m_timeSinceStep = 0f;

    public PhysicsManager()
    {
      CanPause = true;
    }

    #region IManager

    public bool CanPause { get; private set; }

    public bool Paused { get; set; }

    public bool Initialize()
    {
      Log.Verbose("PhysicsManager Initializing");

      // Farseer configuration
      Settings.AllowSleep = true;
      Settings.UseFPECollisionCategories = true;
      Settings.VelocityIterations = 10;
      Settings.PositionIterations = 8;

      Log.DebugFmt("UpdateInterval: {0:F4} ({1:F2} fps)", 
        UpdateInterval, 1 / UpdateInterval);
      World = new World(Vector2.Zero);
      return true;
    }

    public bool PostInitialize()
    {
      Log.Verbose("PhysicsManager Post-Initializing");

      return true;
    }

    public void Update(float deltaTime)
    {
      if (Paused)
      {
        return;
      }

      m_timeSinceStep += deltaTime;
      while (m_timeSinceStep >= UpdateInterval)
      {
        OnPreStep(UpdateInterval);
        World.Step(UpdateInterval);
        World.ClearForces();
        OnPostStep(UpdateInterval);
        m_timeSinceStep -= UpdateInterval;
      }
    }

    public void Shutdown()
    {
      Log.Verbose("PhysicsManager Shutting Down");

      PreStep = null;
      PostStep = null;
      World = null;
    }

    #endregion
    #region IPhysicsManager

    public event Action<float> PreStep;

    public event Action<float> PostStep;

    public World World { get; private set; }

    #endregion
    #region Event Invokers

    private void OnPreStep(float deltaTime)
    {
      if (PreStep != null)
      {
        PreStep(deltaTime);
      }
    }

    private void OnPostStep(float deltaTime)
    {
      if (PostStep != null)
      {
        PostStep(deltaTime);
      }
    }

    #endregion
  }
}
