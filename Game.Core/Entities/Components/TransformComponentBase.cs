using Game.Core.Extensions;
using Microsoft.Xna.Framework;
using SFML.Graphics;

namespace Game.Core.Entities.Components
{
  /// <summary>
  ///   The base class for all types that supply an object transform.
  /// </summary>
  public abstract class TransformComponentBase
    : ComponentBase
  {
    protected TransformComponentBase(Entity parent) 
      : base(parent)
    {
    }

    /// <summary>
    ///   The position of the object.
    /// </summary>
    public abstract Vector2 Position { get; set; }

    /// <summary>
    ///   The rotation of the object in radians.
    /// </summary>
    public abstract float Rotation { get; set; }

    /// <summary>
    ///   Gets a SFML transform for this transform.
    /// </summary>
    public Transform GraphicsTransform
    {
      get
      {
        var result = Transform.Identity;
        var angle = MathHelper.ToDegrees(Rotation);
        result.Translate(Position.ToVector2f().InvertY());
        result.Rotate(angle);
        return result;
      }
    }

    /// <summary>
    ///   Moves the object by the specified x and y amounts.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void Move(float x, float y)
    {
      Move(new Vector2(x, y));
    }

    /// <summary>
    ///   Moves the object by the x and y components of the vector.
    /// </summary>
    /// <param name="offset"></param>
    public void Move(Vector2 offset)
    {
      Position += offset;
    }

    /// <summary>
    ///   Rotates the object by the specified amount in radians.
    /// </summary>
    /// <param name="radians"></param>
    public void Rotate(float radians)
    {
      Rotation += radians;
    }

    /// <summary>
    ///   Rotates the object by the specified amount in degrees.
    /// </summary>
    /// <param name="degrees"></param>
    public void RotateDegrees(float degrees)
    {
      Rotation += MathHelper.ToRadians(degrees);
    }
  }
}
