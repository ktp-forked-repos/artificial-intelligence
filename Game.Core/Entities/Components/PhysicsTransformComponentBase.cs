using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Game.Core.Interfaces;
using Microsoft.Xna.Framework;

namespace Game.Core.Entities.Components
{
  /// <summary>
  ///   A transform whose position and rotation are bound to a physics body.
  /// </summary>
  /// <remarks>
  ///   It's assumed that the body of the physics object is set during 
  ///   initialization and that this component won't be accessed until 
  ///   after initialization.
  /// 
  ///   Not tested because there's really no logic here
  /// </remarks>
  [ExcludeFromCodeCoverage]
  public class PhysicsTransformComponentBase
    : TransformComponentBase
  {
    private IPhysicsObject m_physicsObject;

    /// <summary>
    ///   Create the manager.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="physicsObject"></param>
    /// <exception cref="ArgumentNullException">
    ///   physicsManager is null
    /// </exception>
    protected PhysicsTransformComponentBase(Entity parent,
      IPhysicsObject physicsObject) 
      : base(parent)
    {
      if (physicsObject == null) 
        throw new ArgumentNullException("physicsObject");

      m_physicsObject = physicsObject;
      
      NeedsUpdate = false;
    }

    #region TransformComponentBase

    public override Vector2 Position
    {
      get
      {
        Debug.Assert(m_physicsObject.Body != null);
        return m_physicsObject.Body.Position;
      }
      set
      {
        Debug.Assert(m_physicsObject.Body != null);
        m_physicsObject.Body.Position = value;
      }
    }

    public override float Rotation
    {
      get
      {
        Debug.Assert(m_physicsObject.Body != null);
        return m_physicsObject.Body.Rotation;
      }
      set
      {
        Debug.Assert(m_physicsObject.Body != null);
        m_physicsObject.Body.Rotation = value;
      }
    }

    #endregion
    #region ComponentBase

    public override void Update(float deltaTime)
    {
    }

    protected override void DoDestroy()
    {
      m_physicsObject = null;
      base.DoDestroy();
    }

    #endregion
    #region IDisposable

    protected override void Dispose(bool disposing)
    {
      m_physicsObject = null;
    }

    #endregion
  }
}
