using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Reflection;

namespace ScpControl 
{
    public partial class XmlMapper : Component 
    {        
        public event EventHandler<DebugEventArgs> Debug = null;

        protected virtual void LogDebug(String Data) 
        {
            DebugEventArgs args = new DebugEventArgs(Data);

            if (Debug != null)
            {
                Debug(this, args);
            }
        }

        protected Profile    m_Empty  = new Profile(true, DsMatch.None.ToString(), DsMatch.Global.ToString(), String.Empty);
        protected ProfileMap m_Mapper = new ProfileMap();

        protected Ds3ButtonAxisMap Ds3ButtonAxis = new Ds3ButtonAxisMap();

        protected volatile Boolean m_Remapping = false;
        protected volatile String  m_Active = String.Empty, m_Version = String.Empty, m_Description = String.Empty;

        protected Profile Find(String Mac, Int32 PadId) 
        {
            Profile Found = m_Empty;
            String  Pad   = ((DsPadId) PadId).ToString();

            DsMatch Current = DsMatch.None, Target = DsMatch.None;

            foreach(Profile Item in m_Mapper.Values)
            {
                Target = Item.Usage(Pad, Mac);

                if (Target > Current)
                {
                    Found = Item; Current = Target;
                }
            }

            return Found;
        }

        public XmlMapper() 
        {
            InitializeComponent();

            Ds3ButtonAxis[Ds3Button.L1      ] = Ds3Axis.L1;
            Ds3ButtonAxis[Ds3Button.L2      ] = Ds3Axis.L2;
            Ds3ButtonAxis[Ds3Button.R1      ] = Ds3Axis.R1;
            Ds3ButtonAxis[Ds3Button.R2      ] = Ds3Axis.R2;

            Ds3ButtonAxis[Ds3Button.Triangle] = Ds3Axis.Triangle;
            Ds3ButtonAxis[Ds3Button.Circle  ] = Ds3Axis.Circle;
            Ds3ButtonAxis[Ds3Button.Cross   ] = Ds3Axis.Cross;
            Ds3ButtonAxis[Ds3Button.Square  ] = Ds3Axis.Square;

            Ds3ButtonAxis[Ds3Button.Up      ] = Ds3Axis.Up;
            Ds3ButtonAxis[Ds3Button.Right   ] = Ds3Axis.Right;
            Ds3ButtonAxis[Ds3Button.Down    ] = Ds3Axis.Down;
            Ds3ButtonAxis[Ds3Button.Left    ] = Ds3Axis.Left;
        }

        public XmlMapper(IContainer container) 
        {
            container.Add(this);

            InitializeComponent();

            Ds3ButtonAxis[Ds3Button.L1      ] = Ds3Axis.L1;
            Ds3ButtonAxis[Ds3Button.L2      ] = Ds3Axis.L2;
            Ds3ButtonAxis[Ds3Button.R1      ] = Ds3Axis.R1;
            Ds3ButtonAxis[Ds3Button.R2      ] = Ds3Axis.R2;

            Ds3ButtonAxis[Ds3Button.Triangle] = Ds3Axis.Triangle;
            Ds3ButtonAxis[Ds3Button.Circle  ] = Ds3Axis.Circle;
            Ds3ButtonAxis[Ds3Button.Cross   ] = Ds3Axis.Cross;
            Ds3ButtonAxis[Ds3Button.Square  ] = Ds3Axis.Square;

            Ds3ButtonAxis[Ds3Button.Up      ] = Ds3Axis.Up;
            Ds3ButtonAxis[Ds3Button.Right   ] = Ds3Axis.Right;
            Ds3ButtonAxis[Ds3Button.Down    ] = Ds3Axis.Down;
            Ds3ButtonAxis[Ds3Button.Left    ] = Ds3Axis.Left;
        }


        public virtual Boolean Initialize() 
        {
            return false; //TODO: Clean this up
        }

        public virtual Boolean Shutdown() 
        {
            m_Remapping = false;

            LogDebug("## Mapper.Shutdown()");
            return true;
        }

        public virtual Boolean Construct() 
        {
            Boolean Constructed = true;


            return Constructed;
        }


        public virtual Boolean Remap(DsModel Type, Int32 Pad, String Mac, Byte[] Input, Byte[] Output) 
        {
            Boolean Mapped = false;

            try
            {
                if (m_Remapping)
                {
                    switch (Type)
                    {
                        case DsModel.DS3: Mapped = RemapDs3(Find(Mac, Pad), Input, Output); break;
                    }
                }
            }
            catch { }

            return Mapped;
        }


        public virtual Boolean RemapDs3(Profile Map, Byte[] Input, Byte[] Output) 
        {
            Boolean Mapped = false;

            try
            {
                Array.Copy(Input, Output, Input.Length);

                // Map Buttons
                Ds3Button In = (Ds3Button)(UInt32)((Input[10] << 0) | (Input[11] << 8) | (Input[12] << 16) | (Input[13] << 24));
                Ds3Button Out = In;

                foreach (Ds3Button Item in Map.Ds3Button.Keys) if ((Out & Item) != Ds3Button.None) Out ^= Item;
                foreach (Ds3Button Item in Map.Ds3Button.Keys) if ((In  & Item) != Ds3Button.None) Out |= Map.Ds3Button[Item];

                Output[10] = (Byte)((UInt32) Out >>  0 & 0xFF);
                Output[11] = (Byte)((UInt32) Out >>  8 & 0xFF);
                Output[12] = (Byte)((UInt32) Out >> 16 & 0xFF);
                Output[13] = (Byte)((UInt32) Out >> 24 & 0xFF);

                // Map Axis
                foreach (Ds3Axis Item in Map.Ds3Axis.Keys)
                {
                    switch (Item)
                    {
                        case Ds3Axis.LX:
                        case Ds3Axis.LY:
                        case Ds3Axis.RX:
                        case Ds3Axis.RY: 
                            Output[(UInt32) Item] = 127; // Centred
                            break;

                        default:
                            Output[(UInt32) Item] =   0;
                            break;
                    }
                }

                foreach (Ds3Axis Item in Map.Ds3Axis.Keys)
                {
                    if (Map.Ds3Axis[Item] != Ds3Axis.None)
                    {
                        Output[(UInt32) Map.Ds3Axis[Item]] = Input[(UInt32) Item];
                    }
                }

                // Fix up Button-Axis Relations
                foreach (Ds3Button Key in Ds3ButtonAxis.Keys)
                {
                    if ((Out & Key) != Ds3Button.None && Output[(UInt32) Ds3ButtonAxis[Key]] == 0)
                    {
                        Output[(UInt32) Ds3ButtonAxis[Key]] = 0xFF;
                    }
                }

                Mapped = true;
            }
            catch { }

            return Mapped;
        }
        public virtual String[] Profiles 
        {
            get 
            {
                Int32 Index = 0;
                String[] List = new String[m_Mapper.Count];

                foreach (String Item in m_Mapper.Keys)
                {
                    List[Index++] = Item;
                }

                return List;
            }
        }

        public virtual String   Active   
        {
            get { return m_Active; }
        }

        public virtual ProfileMap Map    
        {
            get { return m_Mapper; }
        }
    }
}
