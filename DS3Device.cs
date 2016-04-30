using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ODIF;
using ODIF.Extensions;
namespace ScpControl
{
    internal class DS3Device
    {
        public InputChannelTypes.JoyAxis LSx { get; set; }
        public InputChannelTypes.JoyAxis LSy { get; set; }
        public InputChannelTypes.JoyAxis RSx { get; set; }
        public InputChannelTypes.JoyAxis RSy { get; set; }

        public InputChannelTypes.JoyAxis vCross { get; set; }
        public InputChannelTypes.JoyAxis vCircle { get; set; }
        public InputChannelTypes.JoyAxis vSquare { get; set; }
        public InputChannelTypes.JoyAxis vTriangle { get; set; }

        public InputChannelTypes.Button L3 { get; set; }
        public InputChannelTypes.Button R3 { get; set; }

        public InputChannelTypes.JoyAxis L2 { get; set; }
        public InputChannelTypes.JoyAxis R2 { get; set; }
        public InputChannelTypes.Button L1 { get; set; }
        public InputChannelTypes.Button R1 { get; set; }

        public InputChannelTypes.Button DUp { get; set; }
        public InputChannelTypes.Button DDown { get; set; }
        public InputChannelTypes.Button DLeft { get; set; }
        public InputChannelTypes.Button DRight { get; set; }

        public InputChannelTypes.Button Cross { get; set; }
        public InputChannelTypes.Button Circle { get; set; }
        public InputChannelTypes.Button Square { get; set; }
        public InputChannelTypes.Button Triangle { get; set; }

        public InputChannelTypes.Button PS { get; set; }
        public InputChannelTypes.Button Select { get; set; }
        public InputChannelTypes.Button Start { get; set; }

        public InputChannelTypes.JoyAxis Battery { get; set; }
        public InputChannelTypes.Button Charging { get; set; }

        public OutputChannelTypes.RumbleMotor BigRumble { get; set; }
        public OutputChannelTypes.RumbleMotor SmallRumble { get; set; }
        public OutputChannelTypes.RGBLED LightBar { get; set; }

        public DS3Device()
        {
            LSx = new InputChannelTypes.JoyAxis("Left Stick X", "", Properties.Resources.PS3_Left_Stick.ToImageSource());
            LSy = new InputChannelTypes.JoyAxis("Left Stick Y", "", Properties.Resources.PS3_Left_Stick.ToImageSource());
            RSx = new InputChannelTypes.JoyAxis("Right Stick X", "", Properties.Resources.PS3_Right_Stick.ToImageSource());
            RSy = new InputChannelTypes.JoyAxis("Right Stick Y", "", Properties.Resources.PS3_Right_Stick.ToImageSource());

            L3 = new InputChannelTypes.Button("L3", "Left stick", Properties.Resources.PS3_Left_Stick.ToImageSource());
            R3 = new InputChannelTypes.Button("R3", "Right Stick", Properties.Resources.PS3_Right_Stick.ToImageSource());

            L2 = new InputChannelTypes.JoyAxis("L2", "Left Trigger", Properties.Resources.PS3_L2.ToImageSource()) { min_Value = 0 };
            R2 = new InputChannelTypes.JoyAxis("R2", "Right Trigger", Properties.Resources.PS3_R2.ToImageSource()) { min_Value = 0 };
            L1 = new InputChannelTypes.Button("L1", "Left Bumper", Properties.Resources.PS3_L1.ToImageSource());
            R1 = new InputChannelTypes.Button("R1", "Right Bumper", Properties.Resources.PS3_R1.ToImageSource());

            DUp = new InputChannelTypes.Button("DPad Up", "", Properties.Resources.PS3_Dpad_Up.ToImageSource());
            DDown = new InputChannelTypes.Button("DPad Down", "", Properties.Resources.PS3_Dpad_Down.ToImageSource());
            DLeft = new InputChannelTypes.Button("DPad Left", "", Properties.Resources.PS3_Dpad_Left.ToImageSource());
            DRight = new InputChannelTypes.Button("DPad Right", "", Properties.Resources.PS3_Dpad_Right.ToImageSource());

            Cross = new InputChannelTypes.Button("Cross", "", Properties.Resources.PS3_Cross.ToImageSource());
            Circle = new InputChannelTypes.Button("Circle", "", Properties.Resources.PS3_Circle.ToImageSource());
            Square = new InputChannelTypes.Button("Square", "", Properties.Resources.PS3_Square.ToImageSource());
            Triangle = new InputChannelTypes.Button("Triangle", "", Properties.Resources.PS3_Triangle.ToImageSource());

            vCross = new InputChannelTypes.JoyAxis("Variable Cross", "", Properties.Resources.PS3_Cross.ToImageSource()) { min_Value = 0 };
            vCircle = new InputChannelTypes.JoyAxis("Variable Circle", "", Properties.Resources.PS3_Circle.ToImageSource()) { min_Value = 0 };
            vSquare = new InputChannelTypes.JoyAxis("Variable Square", "", Properties.Resources.PS3_Square.ToImageSource()) { min_Value = 0 };
            vTriangle = new InputChannelTypes.JoyAxis("Variable Triangle", "", Properties.Resources.PS3_Triangle.ToImageSource()) { min_Value = 0 };

            PS = new InputChannelTypes.Button("PS", "");
            Select = new InputChannelTypes.Button("Select", "", Properties.Resources.PS3_Select.ToImageSource());
            Start = new InputChannelTypes.Button("Start", "", Properties.Resources.PS3_Start.ToImageSource());

            Battery = new InputChannelTypes.JoyAxis("Battery Level", "");
            Charging = new InputChannelTypes.Button("Charging", "");

            BigRumble = new OutputChannelTypes.RumbleMotor("Big Rumble", "");
            SmallRumble = new OutputChannelTypes.RumbleMotor("Small Rumble", "");
            LightBar = new OutputChannelTypes.RGBLED("Light Bar", "");
        }
    }
}
