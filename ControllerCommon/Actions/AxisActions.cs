﻿using System;
using System.Numerics;
using System.Windows.Forms;
using ControllerCommon.Inputs;
using ControllerCommon.Utils;

namespace ControllerCommon.Actions;

[Serializable]
public class AxisActions : IActions
{
    public AxisActions()
    {
        ActionType = ActionType.Joystick;
        this.Value = new Vector2();
    }

    public AxisActions(AxisLayoutFlags axis) : this()
    {
        Axis = axis;
    }

    public AxisLayoutFlags Axis { get; set; }

    // Axis to axis
    public bool ImproveCircularity { get; set; } = false;
    public int AxisAntiDeadZone { get; set; } = 0;
    public int AxisDeadZoneInner { get; set; } = 0;
    public int AxisDeadZoneOuter { get; set; } = 0;
    public bool AxisRotated { get; set; } = false;
    public bool AxisInverted { get; set; } = false;

    public void Execute(AxisFlags axis, short value)
    {
        // Apply inner and outer deadzone adjustments
        value = (short)InputUtils.InnerOuterDeadzone(value, AxisDeadZoneInner, AxisDeadZoneOuter, short.MaxValue);

        // Apply anti deadzone adjustments
        value = (short)InputUtils.ApplyAntiDeadzone(value, AxisAntiDeadZone, short.MaxValue);

        Value = (short)(value * (AxisInverted ? -1 : 1));
    }

    public void Execute(AxisLayout layout)
    {
        // Apply inner and outer deadzone adjustments
        layout.vector = InputUtils.ThumbScaledRadialInnerOuterDeadzone(layout.vector, AxisDeadZoneInner, AxisDeadZoneOuter);

        // Apply anti-deadzone adjustments
        layout.vector = InputUtils.ApplyAntiDeadzone(layout.vector, AxisAntiDeadZone);
        
        // Apply improved circularity
        if (ImproveCircularity)
            layout.vector = InputUtils.ImproveCircularity(layout.vector);

        if (AutoRotate)
            Value = ((Orientation & ScreenOrientation.Angle90) == ScreenOrientation.Angle90
                         ? new Vector2(layout.vector.Y, -layout.vector.X)
                         : layout.vector)
                     * ((Orientation & ScreenOrientation.Angle180) == ScreenOrientation.Angle180 ? -1.0f : 1.0f);
        else
            Value = (AxisRotated ? new Vector2(layout.vector.Y, -layout.vector.X) : layout.vector)
                     * (AxisInverted ? -1.0f : 1.0f);
    }

    public Vector2 GetValue()
    {
        return (Vector2)this.Value;
    }
}