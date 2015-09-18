using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Common.Extensions;
using log4net;

namespace Common.Game.Components
{
  public abstract class ComponentBase
    : EntityLifeCycleBase
  {
    private static readonly ILog Log = LogManager.GetLogger(
      MethodBase.GetCurrentMethod().DeclaringType);

    protected ComponentBase(Entity parent)
    {
      if (parent == null) throw new ArgumentNullException("parent");

      Parent = parent;
    }

    public Entity Parent { get; private set; }
    
    public bool NeedsUpdate { get; protected set; }

    public abstract void Update(double deltaTime);
  }
}
