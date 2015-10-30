using Game.Core.Entities;
using Game.Core.Entities.Components;
using Game.Core.Interfaces;
using Microsoft.Xna.Framework;
using Moq;
using SFML.Graphics;

namespace Game.Core.Tests.Stubs
{
  internal class ComponentStub
    : ComponentBase
  {
    public ComponentStub(Entity parent, bool initializationResult = true) 
      : base(parent)
    {
      InitializationResult = initializationResult;
    }

    public bool InitializationResult { get; private set; }

    public float LastUpdateDeltaTime { get; private set; }
    public int UpdateCallCount { get; private set; }
    public int DoInitializeCallCount { get; private set; }
    public int DoActivateCallCount { get; private set; }
    public int DoDeactivateCallCount { get; private set; }
    public int DoDestroyCallCount { get; private set; }
    public int DisposeCallCount { get; private set; }

    public override void Update(float deltaTime)
    {
      LastUpdateDeltaTime = deltaTime;
      UpdateCallCount++;
    }

    protected override bool DoInitialize()
    {
      DoInitializeCallCount++;
      return base.DoInitialize() && InitializationResult;
    }

    protected override void DoActivate()
    {
      base.DoActivate();
      DoActivateCallCount++;
    }

    protected override void DoDeactivate()
    {
      base.DoDeactivate();
      DoDeactivateCallCount++;
    }

    protected override void DoDestroy()
    {
      base.DoDestroy();
      DoDestroyCallCount++;
    }

    protected override void Dispose(bool disposing)
    {
      DisposeCallCount++;
    }
  }

  internal class UpdateComponentStub
    : ComponentStub
  {
    public UpdateComponentStub(Entity parent, bool initializationResult = true) 
      : base(parent, initializationResult)
    {
      NeedsUpdate = true;
    }
  }

  internal class TransformComponentStub
    : TransformComponentBase
  {
    public TransformComponentStub(Entity parent, bool initializationResult = true) 
      : base(parent)
    {
      InitializationResult = initializationResult;
    }

    public bool InitializationResult { get; private set; }

    public float LastUpdateDeltaTime { get; private set; }
    public int UpdateCallCount { get; private set; }
    public int DoInitializeCallCount { get; private set; }
    public int DoActivateCallCount { get; private set; }
    public int DoDeactivateCallCount { get; private set; }
    public int DoDestroyCallCount { get; private set; }
    public int DisposeCallCount { get; private set; }

    public override void Update(float deltaTime)
    {
      LastUpdateDeltaTime = deltaTime;
      UpdateCallCount++;
    }

    protected override bool DoInitialize()
    {
      DoInitializeCallCount++;
      return base.DoInitialize() && InitializationResult;
    }

    protected override void DoActivate()
    {
      base.DoActivate();
      DoActivateCallCount++;
    }

    protected override void DoDeactivate()
    {
      base.DoDeactivate();
      DoDeactivateCallCount++;
    }

    protected override void DoDestroy()
    {
      base.DoDestroy();
      DoDestroyCallCount++;
    }

    protected override void Dispose(bool disposing)
    {
      DisposeCallCount++;
    }

    public override Vector2 Position { get; set; }

    public override float Rotation { get; set; }
  }

  internal class RenderComponentStub
    : ComponentStub, IRenderable
  {
    public RenderComponentStub(Entity parent, bool initializationResult = true) 
      : base(parent, initializationResult)
    {
      RenderDepth = 1;
    }

    public int CompareTo(IRenderable other)
    {
      return RenderableCompare.CompareTo(this, other);
    }

    public int RenderId { get; set; }

    public int RenderDepth { get; private set; }
    
    public void Draw(RenderTarget target)
    {
      var drawMock = new Mock<Drawable>();
      target.Draw(drawMock.Object);
    }
  }

  internal class RenderComponentStub2
    : RenderComponentStub
  {
    public RenderComponentStub2(Entity parent, bool initializationResult = true) 
      : base(parent, initializationResult)
    {
    }
  }
}
