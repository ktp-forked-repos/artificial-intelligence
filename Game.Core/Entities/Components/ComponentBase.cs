using System;
using System.Reflection;
using Common.Extensions;
using log4net;

namespace Game.Core.Entities.Components
{
  /// <summary>
  ///   The base for all components that give functionality to an entity.
  /// 
  ///   The entity manages the state of all the components it holds.  A
  ///   component should never change state on its own.
  /// </summary>
  public abstract class ComponentBase
    : EntityLifeCycleBase
  {
    private static readonly ILog Log = LogManager.GetLogger(
      MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    ///   Create the component.
    /// </summary>
    /// <param name="parent">
    ///   The entity that owns this component.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   parent is null.
    /// </exception>
    protected ComponentBase(Entity parent)
    {
      if (parent == null) throw new ArgumentNullException("parent");

      Parent = parent;
      NeedsUpdate = false;
    }

    /// <summary>
    ///   The entity that owns and holds this component.
    /// </summary>
    public Entity Parent { get; private set; }
    
    /// <summary>
    ///   If true, the component will receive updates when it is active.
    /// 
    ///   Must be set by inherited classes.
    /// </summary>
    public bool NeedsUpdate { get; protected set; }

    /// <summary>
    ///   Performs a frame update on the component.  Will be called if 
    ///   NeedsUpdate is true and the component's state is Active.
    /// </summary>
    /// <param name="deltaTime">
    ///   Time since the last update.
    /// </param>
    public abstract void Update(float deltaTime);

    protected override bool DoInitialize()
    {
      Log.VerboseFmt("{0} {1} initialized", Parent.Name, GetType().FullName);
      return true;
    }

    protected override void DoActivate()
    {
      Log.VerboseFmt("{0} {1} activated", Parent.Name, GetType().FullName);
    }

    protected override void DoDeactivate()
    {
      Log.VerboseFmt("{0} {1} deactivated", Parent.Name, GetType().FullName);
    }

    protected override void DoDestroy()
    {
      Log.VerboseFmt("{0} {1} destroyed", Parent.Name, GetType().FullName);
    }
  }
}
