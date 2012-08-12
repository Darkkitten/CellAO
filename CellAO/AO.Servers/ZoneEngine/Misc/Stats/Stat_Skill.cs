﻿#region License
/*
Copyright (c) 2005-2012, CellAO Team

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
#endregion

namespace ZoneEngine.Misc
{
    public class Stat_Skill : Class_Stat
    {
        public Stat_Skill(int Number, int Default, string name, bool sendbase, bool dontwrite, bool announce)
        {
            StatNumber = Number;
            StatDefault = (uint) Default;

            Value = (int) StatDefault;
            SendBaseValue = true;
            this.DoNotDontWriteToSql = false;
            AnnounceToPlayfield = false;
        }

        public override void CalcTrickle()
        {
            double StrengthTrickle = SkillTrickleTable.table[StatNumber - 100, 1];
            double AgilityTrickle = SkillTrickleTable.table[StatNumber - 100, 2];
            double StaminaTrickle = SkillTrickleTable.table[StatNumber - 100, 3];
            double IntelligenceTrickle = SkillTrickleTable.table[StatNumber - 100, 4];
            double SenseTrickle = SkillTrickleTable.table[StatNumber - 100, 5];
            double PsychicTrickle = SkillTrickleTable.table[StatNumber - 100, 6];

            Character_Stats st = ((Character) Parent).Stats;
            Trickle = Convert.ToInt32(Math.Floor((
                                                     StrengthTrickle*st.Strength.Value +
                                                     StaminaTrickle*st.Stamina.Value +
                                                     SenseTrickle*st.Sense.Value +
                                                     AgilityTrickle*st.Agility.Value +
                                                     IntelligenceTrickle*st.Intelligence.Value +
                                                     PsychicTrickle*st.Psychic.Value)/4));

            if (!Parent.startup)
            {
                AffectStats();
            }
        }
    }
}