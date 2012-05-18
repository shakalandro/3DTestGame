using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace _3DTestGame
{
    public interface ICamera
    {
        Matrix view { get; }
        Matrix projection { get; }
        Vector3 pos { get; }
        Vector3 dir { get; }
        Vector3 up { get; }
        float moveSpeed { get; }
        float rotateSpeed { get; }
        void fixate(EntityModel comp, float distance);
    }
}
