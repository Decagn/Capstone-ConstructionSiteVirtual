using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public interface IMeasTool
{
    string Name { get; }
    Sprite ToolIcon { get; }
    List<Vector3> SelecPoints { get; }
    Meas TakeMeas();
    IMeas TakePoint(Vector3 point);
}
