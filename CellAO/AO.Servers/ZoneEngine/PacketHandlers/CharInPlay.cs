﻿#region License
// Copyright (c) 2005-2012, CellAO Team
// 
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the CellAO Team nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
// PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
// PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
// LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
// SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

#region Usings...

#endregion

namespace ZoneEngine.PacketHandlers
{
    using System;

    using AO.Core;

    using ZoneEngine.Misc;

    /// <summary>
    /// 
    /// </summary>
    public class CharInPlay
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="client"></param>
        public static void Read(ref byte[] packet, Client client)
        {
            // client got all the needed data and
            // wants to enter the world. After we
            // reply to this, the character will really be in game

            byte[] chrID = BitConverter.GetBytes(client.Character.ID);
            Array.Reverse(chrID);
            PacketWriter _writer = new PacketWriter();
            _writer.PushByte(0xDF);
            _writer.PushByte(0xDF);
            _writer.PushShort(10);
            _writer.PushShort(1);
            _writer.PushShort(0);
            _writer.PushInt(3086);
            _writer.PushInt(client.Character.ID);
            _writer.PushInt(0x570C2039);
            _writer.PushIdentity(50000, client.Character.ID);
            _writer.PushByte(0);
            byte[] reply = _writer.Finish();
            Announce.Playfield(client.Character.PlayField, ref reply);
            Dynels.GetDynels(client);

            //Mobs get sent whenever player enters playfield, BUT (!) they are NOT synchronized, because the mobs don't save stuff yet.
            //for instance: the waypoints the mob went through will NOT be saved and therefore when you re-enter the PF, it will AGAIN
            //walk the same waypoints.
            //TODO: Fix it
            /*foreach (MobType mob in NPCPool.Mobs)
            {
                //TODO: Make cache - use pf indexing somehow.
                if (mob.pf == client.Character.pf)
                {
                    mob.SendToClient(client);
                }
            }*/
        }
    }
}