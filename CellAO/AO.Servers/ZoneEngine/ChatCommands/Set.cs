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

namespace ZoneEngine.ChatCommands
{
    using System;
    using System.Collections.Generic;

    using AO.Core;

    using ZoneEngine.Misc;
    using ZoneEngine.Script;

    public class ChatCommandSet : AOChatCommand
    {
        public override void ExecuteCommand(Client client, Identity target, string[] args)
        {
            // Fallback to self if no target is selected
            if (target.Instance == 0)
            {
                target.Type = client.Character.Type;
                target.Instance = client.Character.Id;
            }

            int statId = StatsList.GetStatId(args[1]);
            if (statId == 1234567890)
            {
                client.SendChatText("Unknown Stat name " + args[1]);
                return;
            }
            uint statNewValue = 1234567890;
            try
            {
                statNewValue = uint.Parse(args[2]);
            }
            catch
            {
                try
                {
                    // For values >= 2^31
                    statNewValue = (uint.Parse(args[2]));
                }
                catch (FormatException)
                {
                }
                catch (OverflowException)
                {
                }
            }
            Character tempch = (Character)FindDynel.FindDynelById(target.Type, target.Instance);
            uint statOldValue;
            try
            {
                statOldValue = tempch.Stats.GetBaseValue(statId);
                tempch.Stats.SetStatValueByName(statId, statNewValue);
                if (tempch.Client != null)
                {
                    tempch.Stats.Send(tempch.Client, statId);
                }
            }
            catch
            {
                client.SendChatText("Unknown StatId " + statId);
                return;
            }

            string response = "Character " + tempch.Name + " (" + target.Instance + "): Stat "
                              + StatsList.GetStatName(statId) + " (" + statId + ") =";
            response += " Old: " + statOldValue;
            response += " New: " + statNewValue;
            client.SendChatText(response);
        }

        public override void CommandHelp(Client client)
        {
            // No help needed, no arguments can be given
            return;
        }

        public override bool CheckCommandArguments(string[] args)
        {
            // Two different checks return true: <int> <uint> and <string> <uint>
            List<Type> check = new List<Type>();
            check.Add(typeof(int));
            check.Add(typeof(uint));
            bool check1 = CheckArgumentHelper(check, args);

            check.Clear();
            check.Add(typeof(string));
            check.Add(typeof(uint));
            check1 |= CheckArgumentHelper(check, args);
            return check1;
        }

        public override int GMLevelNeeded()
        {
            // Be a GM
            return 1;
        }

        public override List<string> ListCommands()
        {
            List<string> temp = new List<string>();
            temp.Add("set");
            return temp;
        }
    }
}