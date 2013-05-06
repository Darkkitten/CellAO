﻿#region License
/*
Copyright (c) 2005-2011, CellAO Team

All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
#endregion

#region Usings...
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
#endregion

namespace AO.Core
{
    /// <summary>
    /// AOItem
    /// </summary>
    [Serializable]
    public class AOItem
    {
        /// <summary>
        /// Item Flags
        /// </summary>
        public int Flags;

        /// <summary>
        /// Item low ID
        /// </summary>
        public int LowID;

        /// <summary>
        /// Item high ID
        /// </summary>
        public int HighID;

        /// <summary>
        /// Quality level
        /// </summary>
        public int Quality;

        /// <summary>
        /// Stacked item count
        /// </summary>
        public int MultipleCount;

        /// <summary>
        /// Type of instanced item
        /// </summary>
        public int Type;

        /// <summary>
        /// Instance of instanced item
        /// </summary>
        public int Instance;

        /// <summary>
        /// dunno yet
        /// </summary>
        public int Nothing;

        /// <summary>
        /// Item type
        /// </summary>
        public int ItemType;

        /// <summary>
        /// Item attributes
        /// </summary>
        public List<AOItemAttribute> Stats = new List<AOItemAttribute>();

        /// <summary>
        /// List of Attack attributes
        /// </summary>
        public List<AOItemAttribute> Attack = new List<AOItemAttribute>();

        /// <summary>
        /// List of defensive attributes
        /// </summary>
        public List<AOItemAttribute> Defend = new List<AOItemAttribute>();

        /// <summary>
        /// List of Item events
        /// </summary>
        public List<AOEvents> Events = new List<AOEvents>();

        /// <summary>
        /// List of Item Actions (requirement checks)
        /// </summary>
        public List<AOActions> Actions = new List<AOActions>();

        /// Methods to do:
        /// Read Item
        /// Write Item
        /// Return Dynel Item (placing on the ground)
        public AOItem ShallowCopy()
        {
            AOItem it = new AOItem();
            it.LowID = this.LowID;
            it.HighID = this.HighID;

            foreach (AOItemAttribute ai in Attack)
            {
                AOItemAttribute z = new AOItemAttribute();
                z.Stat = ai.Stat;
                z.Value = ai.Value;
                it.Attack.Add(z);
            }

            foreach (AOItemAttribute ai in Defend)
            {
                AOItemAttribute z = new AOItemAttribute();
                z.Stat = ai.Stat;
                z.Value = ai.Value;
                it.Defend.Add(z);
            }

            foreach (AOItemAttribute ai in Stats)
            {
                AOItemAttribute z = new AOItemAttribute();
                z.Stat = ai.Stat;
                z.Value = ai.Value;
                it.Stats.Add(z);
            }

            foreach (AOEvents ev in Events)
            {
                AOEvents newEV = new AOEvents();
                foreach (AOFunctions aof in ev.Functions)
                {
                    AOFunctions newAOF = new AOFunctions();
                    foreach (AORequirements aor in aof.Requirements)
                    {
                        AORequirements newAOR = new AORequirements();
                        newAOR.ChildOperator = aor.ChildOperator;
                        newAOR.Operator = aor.Operator;
                        newAOR.Statnumber = aor.Statnumber;
                        newAOR.Target = aor.Target;
                        newAOR.Value = aor.Value;
                        newAOF.Requirements.Add(newAOR);
                    }

                    foreach (object ob in aof.Arguments.Values)
                    {
                        if (ob.GetType() == typeof (string))
                        {
                            string z = (string) ob;
                            newAOF.Arguments.Values.Add(z);
                        }
                        if (ob.GetType() == typeof (int))
                        {
                            int i = (int) ob;
                            newAOF.Arguments.Values.Add(i);
                        }
                        if (ob.GetType() == typeof (Single))
                        {
                            Single s = (Single) ob;
                            newAOF.Arguments.Values.Add(s);
                        }
                    }
                    newAOF.dolocalstats = aof.dolocalstats;
                    newAOF.FunctionType = aof.FunctionType;
                    newAOF.Target = aof.Target;
                    newAOF.TickCount = aof.TickCount;
                    newAOF.TickInterval = aof.TickInterval;
                    newEV.Functions.Add(newAOF);
                }
                newEV.EventType = ev.EventType;
                it.Events.Add(newEV);
            }


            it.Flags = this.Flags;
            it.Instance = Instance;
            it.ItemType = ItemType;
            it.MultipleCount = this.MultipleCount;
            it.Nothing = Nothing;
            it.Quality = Quality;

            return it;
        }

        /// <summary>
        /// Empty
        /// </summary>
        public AOItem()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Boolean isInstanced()
        {
            if ((Type == 0) && (Instance == 0))
            {
                return false;
            }
            return true;
        }
        /*
        #region Serialition stuff
        /// <summary>
        /// Deserialize AOItem, internal use only
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public AOItem(SerializationInfo info, StreamingContext context)
        {
            this.Flags = (int) info.GetValue("flags", typeof (int));
            this.LowID = (int) info.GetValue("lowID", typeof (int));
            this.HighID = (int) info.GetValue("highID", typeof (int));
            Quality = (int) info.GetValue("Quality", typeof (int));
            this.MultipleCount = (int) info.GetValue("multiplecount", typeof (int));
            Type = (int) info.GetValue("Type", typeof (int));
            Instance = (int) info.GetValue("Instance", typeof (int));
            Nothing = (int) info.GetValue("Nothing", typeof (int));
            ItemType = (int) info.GetValue("ItemType", typeof (int));
            Stats = (List<AOItemAttribute>) info.GetValue("Stats", typeof (List<AOItemAttribute>));
            Attack = (List<AOItemAttribute>) info.GetValue("Attack", typeof (List<AOItemAttribute>));
            Defend = (List<AOItemAttribute>) info.GetValue("Defend", typeof (List<AOItemAttribute>));
            Events = (List<AOEvents>) info.GetValue("Events", typeof (List<AOEvents>));
        }

        /// <summary>
        /// Serialize AOItem, internal use only
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("flags", this.Flags);
            info.AddValue("lowID", this.LowID);
            info.AddValue("highID", this.HighID);
            info.AddValue("Quality", Quality);
            info.AddValue("multiplecount", this.MultipleCount);
            info.AddValue("Type", Type);
            info.AddValue("Instance", Instance);
            info.AddValue("Nothing", Nothing);
            info.AddValue("ItemType", ItemType);
            info.AddValue("Stats", Stats);
            info.AddValue("Attack", Attack);
            info.AddValue("Defend", Defend);
            info.AddValue("Events", Events);
        }
        #endregion
        */
        #region GetWeaponStyle
        /// <summary>
        /// Get WeaponStyle (Stat 274)
        /// </summary>
        /// <returns>Value of Stat 274 or 0</returns>
        public int GetWeaponStyle()
        {
            foreach (AOItemAttribute at in Stats)
            {
                if (at.Stat != 274) continue;

                return at.Value;
            }

            // Odd, no WeaponWieldFlags found...
            return 0;
        }
        #endregion

        #region Get Override Texture
        /// <summary>
        /// Get override texture number
        /// </summary>
        /// <returns></returns>
        public int GetOverrideTexture()
        {
            foreach (AOItemAttribute attr in Stats)
            {
                if (attr.Stat != 336) continue;

                return attr.Value;
            }

            // No Override OH NOES!
            return 0;
        }
        #endregion

        /// <summary>
        /// Get item attribute
        /// </summary>
        /// <param name="number">number of attribute</param>
        /// <returns>Value of item attribute</returns>
        public Int32 getItemAttribute(int number)
        {
            int c;
            for (c = 0; c < Stats.Count; c++)
            {
                if (number == Stats[c].Stat)
                {
                    return Stats[c].Value;
                }
            }
            return 0;
        }

        /// <summary>
        /// Is item useable?
        /// </summary>
        /// <returns></returns>
        public bool isUseable()
        {
            return ((getItemAttribute(30) & Constants.canflag_use) > 0);
        }

        /// <summary>
        /// Is item consumable
        /// </summary>
        /// <returns></returns>
        public bool isConsumable()
        {
            return ((getItemAttribute(30) & Constants.canflag_consume) > 0);
        }

        /// <summary>
        /// Is item stackable
        /// </summary>
        /// <returns></returns>
        public bool isStackable()
        {
            return ((getItemAttribute(30) & Constants.canflag_stackable) > 0);
        }

        /// <summary>
        /// Can item be worn
        /// </summary>
        /// <returns></returns>
        public bool isWearable()
        {
            return ((getItemAttribute(30) & Constants.canflag_wear) > 0);
        }
    }

    /// <summary>
    /// AOItemAttribute
    /// </summary>
    [Serializable]
    public class AOItemAttribute
    {
        /// <summary>
        /// Stat number of attribute
        /// </summary>
        public Int32 Stat;

        /// <summary>
        /// Value of attribute
        /// </summary>
        public Int32 Value;

        /// <summary>
        /// Create a empty AOItemAttribute
        /// </summary>
        public AOItemAttribute()
        {
            Stat = 0;
            Value = 0;
        }
        /*
        /// <summary>
        /// Serialization, internal use only
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public AOItemAttribute(SerializationInfo info, StreamingContext context)
        {
            Stat = (Int32) info.GetValue("Stat", typeof (Int32));
            Value = (Int32) info.GetValue("Value", typeof (Int32));
        }

        /// <summary>
        /// Deserialization, internal use only
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Stat", Stat);
            info.AddValue("Value", Value);
        }
         */
    }
}