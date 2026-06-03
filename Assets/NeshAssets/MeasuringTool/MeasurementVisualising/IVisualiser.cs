using NUnit.Framework;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

/*
 * IVisualiser Interface
 * All visualisers must implement this.
 */
public interface IVisualiser
{
    // Creates and returns a list of the visualised object.
    List<IVisualisedObject> Create(List<Vector3> points);

    // Scales the visualised objects based on how far they are from the player/camera.
    void Scale(IVisualisedObject obj, Camera camera);
}
