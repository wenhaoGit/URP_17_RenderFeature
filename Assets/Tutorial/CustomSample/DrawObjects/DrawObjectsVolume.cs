using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable, VolumeComponentMenu("Custom/DrawObjects")]
public class DrawObjectsVolume : VolumeComponent
{
    public MaterialParameter overrideMaterial = new MaterialParameter(null);
    public LayerMaskParameter filterLayerMask = new LayerMaskParameter(0);
}
