using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace _3DTestGame
{
    interface IInput
    {
        MouseState mouseState {get;}

        Boolean up();
        Boolean down();
        Boolean left();
        Boolean right();
        Boolean forward();
        Boolean backward();

        Boolean up2();
        Boolean down2();
        Boolean left2();
        Boolean right2();

        Boolean up3();
        Boolean down3();
        Boolean left3();
        Boolean right3();

        Boolean leftClick();
    }
}
