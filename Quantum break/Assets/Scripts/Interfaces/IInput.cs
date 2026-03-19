using System;
using UnityEngine;

public interface IInput
{
        Vector2 Move { get; }

        bool JumpPressed { get; }
        bool DashPressed { get; }

        void ClearFrameInput();
}
