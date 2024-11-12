using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//CanvasType Enum can be useful in the future for Scriptable Object to determine and adding new features.
public enum CanvasType
{
  WorldCanvas,
  UICanvas
}

[CreateAssetMenu(fileName = "HealthSO", menuName = "ScriptableObjects/HealthSO")]
public class HealthSO : ScriptableObject
{
  public string canvasName;
  public CanvasType canvasType;
}
