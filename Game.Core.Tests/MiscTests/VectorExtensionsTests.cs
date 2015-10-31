using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Core.Extensions;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using SFML.Graphics;
using SFML.System;

namespace Game.Core.Tests.MiscTests
{
  [TestFixture]
  public class VectorExtensionsTests
  {
    [Test]
    public void Vector2_ToVector2f_Converts()
    {
      var input = new Vector2(1, 2);
      var expected = new Vector2f(1, 2);

      var result = input.ToVector2f();

      Assert.AreEqual(expected, result);
    }

    [Test]
    public void Vector2f_ToVector2_Converts()
    {
      var input = new Vector2f(1, 2);
      var expected = new Vector2(1, 2);

      var result = input.ToVector2();

      Assert.AreEqual(expected, result);
    }

    [Test]
    public void Vector2f_InvertY_Converts()
    {
      var input = new Vector2f(1, 2);
      var expected = new Vector2f(1, -2);

      var result = input.InvertY();

      Assert.AreEqual(expected, result);
    }

    [Test]
    public void Vector2_InvertY_Converts()
    {
      var input = new Vector2(1, 2);
      var expected = new Vector2(1, -2);

      var result = input.InvertY();

      Assert.AreEqual(expected, result);
    }
  }
}
