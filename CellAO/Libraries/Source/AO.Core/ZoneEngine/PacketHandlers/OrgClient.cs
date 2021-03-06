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
using System.Data;
using System.Collections.Generic;
using System.Text;
using AO.Core;
using AO.Core.Config;
#endregion

namespace ZoneEngine.PacketHandlers
{
    /// <summary>
    /// 
    /// </summary>
   
    public class OrgClient
    {
        SqlWrapper ms = new SqlWrapper();
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        /// <param name="client"></param>
        public void Read(ref byte[] packet, Client client)
        {
            PacketReader reader = new PacketReader(ref packet);

            Header header = reader.PopHeader();
            reader.PopByte();
            byte cmd = reader.PopByte();
            Identity target = reader.PopIdentity();
            int unknown = reader.PopInt();
            string CmdStr = "";
            byte CmdByte = 0;

            #region cmd args
            switch (cmd)
            {
                case 1:
                case 7:
                case 9:
                case 13:
                case 17:
                case 19:
                case 20:
                case 21:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                    short CmdStrLen = reader.PopShort();
                    CmdStr = reader.PopString((int)CmdStrLen);
                    break;
                case 10:
                    CmdByte = reader.PopByte();
                    break;
                default:
                    break;
            }
            reader.Finish();
            #endregion
            DataTable dt;
            #region cmd handlers
            switch (cmd)
            {
                #region /org create <name>
                case 1:
                    {
                        // org create
                        /* client wants to create organization
                         * name of org is CmdStr
                         */

                        string SqlQuery = "SELECT * FROM organizations WHERE Name='" + CmdStr + "'";
                        string guild_name = null;
                        uint orgID = 0;
                        dt = ms.ReadDT(SqlQuery);
                        if (dt.Rows.Count > 0)
                        {
                            guild_name = (string)dt.Rows[0]["Name"];
                        }

                        if (guild_name == null)
                        {
                            client.SendChatText("You have created the guild: " + CmdStr);

                            string CurrDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            string SqlQuery2 = "INSERT INTO organizations (Name, creation, LeaderID, GovernmentForm) VALUES ('" + CmdStr + "', '" + CurrDate + "', '" + client.Character.ID + "', '0')";
                            ms.SqlInsert(SqlQuery2);
                            string SqlQuery3 = "SELECT * FROM organizations WHERE Name='" + CmdStr + "'";
                            dt = ms.ReadDT(SqlQuery3);
                            if (dt.Rows.Count > 0)
                            {
                                orgID = (UInt32)dt.Rows[0]["ID"];
                            }

                            // Make sure the order of these next two lines is not swapped -NV
                            client.Character.Stats.ClanLevel.Set(0);
                            client.Character.orgId = orgID;
                            break;
                        }
                        else
                        {
                            client.SendChatText("This guild already <font color=#DC143C>exists</font>");
                            break;
                        }
                    }
                #endregion

                #region /org ranks
                case 2:
                    // org ranks
                    //Displays Org Rank Structure.
                    /* Select governingform from DB, Roll through display from GovForm */
                    if (client.Character.orgId == 0)
                    {
                        client.SendChatText("You're not in an organization!");
                        break;
                    }
                    string ranksSql = "SELECT GovernmentForm FROM organizations WHERE ID = " + client.Character.orgId;
                    int govForm = -1;
                    dt = ms.ReadDT(ranksSql);
                    if (dt.Rows.Count > 0)
                    {
                        govForm = (Int32)dt.Rows[0]["GovernmentForm"];
                    }
                    client.SendChatText("Current Rank Structure: " + GetRankList(govForm));
                    break;
                #endregion

                #region /org contract
                case 3:
                    // org contract
                    break;
                #endregion

                #region unknown org command 4
                case 4:
                    Console.WriteLine("Case 4 Started");
                    break;
                #endregion

                #region /org info
                case 5:
                    {
                        Client tPlayer = null;
                        if (Misc.FindClient.FindClientByID(target.Instance, out tPlayer))
                        {
                            string orgDescription = "", orgObjective = "", orgHistory = "", orgLeaderName = "";
                            int orgGoverningForm = 0, orgLeaderID = 0;
                            dt = ms.ReadDT("SELECT * FROM organizations WHERE ID=" + tPlayer.Character.orgId);

                            if (dt.Rows.Count > 0)
                            {
                                orgDescription = (string)dt.Rows[0]["Description"];
                                orgObjective = (string)dt.Rows[0]["Objective"];
                                orgHistory = (string)dt.Rows[0]["History"];
                                orgGoverningForm = (Int32)dt.Rows[0]["GovernmentForm"];
                                orgLeaderID = (Int32)dt.Rows[0]["LeaderID"];
                            }

                            dt = ms.ReadDT("SELECT Name FROM characters WHERE ID=" + orgLeaderID);
                            if (dt.Rows.Count > 0)
                            {
                                orgLeaderName = (string)dt.Rows[0][0];
                            }

                            string TextGovForm = null;
                            if (orgGoverningForm == 0) { TextGovForm = "Department"; }
                            else if (orgGoverningForm == 1) { TextGovForm = "Faction"; }
                            else if (orgGoverningForm == 2) { TextGovForm = "Republic"; }
                            else if (orgGoverningForm == 3) { TextGovForm = "Monarchy"; }
                            else if (orgGoverningForm == 4) { TextGovForm = "Anarchism"; }
                            else if (orgGoverningForm == 5) { TextGovForm = "Feudalism"; }
                            else { TextGovForm = "Department"; }
                            string orgRank = GetRank(orgGoverningForm, tPlayer.Character.Stats.ClanLevel.StatBaseValue);
                            PacketWriter writer = new PacketWriter();
                            writer.PushBytes(new byte[] { 0xDF, 0xDF });
                            writer.PushShort(10);
                            writer.PushShort(1);
                            writer.PushShort(0);
                            writer.PushInt(3086);
                            writer.PushInt(client.Character.ID);
                            writer.PushInt(0x64582A07);
                            writer.PushIdentity(50000, tPlayer.Character.ID);
                            writer.PushByte(0);
                            writer.PushByte(2);    // OrgServer case 0x02 (Org Info)
                            writer.PushInt(0);
                            writer.PushInt(0);
                            writer.PushInt(0xDEAA); // Type (org)
                            writer.PushUInt(tPlayer.Character.orgId);    // org ID
                            writer.PushShort((short)tPlayer.Character.orgName.Length);
                            writer.PushBytes(Encoding.ASCII.GetBytes(tPlayer.Character.orgName));
                            writer.PushShort((short)orgDescription.Length);
                            writer.PushBytes(Encoding.ASCII.GetBytes(orgDescription));
                            writer.PushShort((short)orgObjective.Length);
                            writer.PushBytes(Encoding.ASCII.GetBytes(orgObjective));
                            writer.PushShort((short)orgHistory.Length);
                            writer.PushBytes(Encoding.ASCII.GetBytes(orgHistory));
                            writer.PushShort((short)TextGovForm.Length);
                            writer.PushBytes(Encoding.ASCII.GetBytes(TextGovForm));
                            writer.PushShort((short)orgLeaderName.Length);
                            writer.PushBytes(Encoding.ASCII.GetBytes(orgLeaderName));
                            writer.PushShort((short)orgRank.Length);
                            writer.PushBytes(Encoding.ASCII.GetBytes(orgRank));
                            writer.Push3F1Count(0);
                            byte[] reply = writer.Finish();

                            client.SendCompressed(reply);
                        }
                    }
                    break;
                #endregion

                #region /org disband
                case 6:
                    break;
                #endregion

                #region /org startvote <text> <duration> <entries>
                case 7:
                    // org startvote <"text"> <duration(minutes)> <entries>
                    // arguments (<text> <duration> and <entries>) are in CmdStr
                    break;
                #endregion

                #region /org vote info
                case 8:
                    // org vote info
                    break;
                #endregion

                #region /org vote <entry>
                case 9:
                    // <entry> is CmdStr
                    break;
                #endregion

                #region /org promote
                case 10:
                    {
                        // some arg in CmdByte. No idea what it is

                        //create the target namespace t_promote
                        Client t_promote = null;
                        string promoteSql = "";
                        int targetOldRank = -1;
                        int targetNewRank = -1;
                        int newPresRank = -1;
                        int oldPresRank = 0;
                        if (Misc.FindClient.FindClientByID(target.Instance, out t_promote))
                        {
                            //First we check if target is in the same org as you
                            if (t_promote.Character.orgId != client.Character.orgId)
                            {
                                //not in same org
                                client.SendChatText("Target is not in your organization!");
                                break;
                            }
                            //Target is in same org, are you eligible to promote?  Promoter Rank has to be TargetRank-2 or == 0
                            if ((client.Character.Stats.ClanLevel.Value == (t_promote.Character.Stats.ClanLevel.Value - 2)) || (client.Character.Stats.ClanLevel.Value == 0))
                            {
                                //Promoter is eligible. Start the process

                                //First we get the details about the org itself
                                promoteSql = "SELECT * FROM organizations WHERE ID = " + client.Character.orgId;
                                dt = ms.ReadDT(promoteSql);

                                int promoteGovForm = -1;
                                string promotedToRank = "";
                                string demotedFromRank = "";

                                if (dt.Rows.Count > 0)
                                {
                                    promoteGovForm = (Int32)dt.Rows[0]["GovernmentForm"];
                                }

                                //Check if new rank == 0, if so, demote promoter
                                if ((targetOldRank - 1) == 0)
                                {
                                    /* This is a bit more complex.  Here we need to promote new president first
                                         * then we go about demoting old president
                                         * finally we set the new leader in SQL
                                         * Reset OrgName to set changes
                                         */

                                    // Set new President's Rank
                                    targetOldRank = t_promote.Character.Stats.ClanLevel.Value;
                                    targetNewRank = targetOldRank - 1;
                                    promotedToRank = GetRank(promoteGovForm, (uint)targetNewRank);
                                    t_promote.Character.Stats.ClanLevel.Set(targetNewRank);
                                    // Demote the old president
                                    oldPresRank = client.Character.Stats.ClanLevel.Value;
                                    newPresRank = oldPresRank + 1;
                                    demotedFromRank = GetRank(promoteGovForm, (uint)newPresRank);
                                    client.Character.Stats.ClanLevel.Set(newPresRank);
                                    //Change the leader id in SQL
                                    string newLeadSql = "UPDATE organizations SET LeaderID = " + t_promote.Character.ID + " WHERE ID = " + t_promote.Character.orgId;
                                    ms.SqlUpdate(newLeadSql);
                                    client.SendChatText("You've passed leadership of the organization to: " + t_promote.Character.Name);
                                    t_promote.SendChatText("You've been promoted to the rank of " + promotedToRank + " by " + client.Character.Name);
                                    break;
                                }
                                else
                                {
                                    //Just Promote
                                    targetOldRank = t_promote.Character.Stats.ClanLevel.Value;
                                    targetNewRank = targetOldRank - 1;
                                    promotedToRank = GetRank(promoteGovForm, (uint)targetNewRank);
                                    t_promote.Character.Stats.ClanLevel.Set(targetNewRank);
                                    client.SendChatText("You've promoted " + t_promote.Character.Name + " to " + promotedToRank);
                                    t_promote.SendChatText("You've been promoted to the rank of " + promotedToRank + " by " + client.Character.Name);
                                }
                            }
                            else
                            {
                                //Promoter not eligible to promote
                                client.SendChatText("Your Rank is not high enough to promote " + t_promote.Character.Name);
                                break;
                            }
                        }
                        break;
                    }
                #endregion

                #region /org demote
                case 11:
                    // demote target player
                    //create the target namespace t_demote
                    Client t_demote = null;
                    string demoteSql = "";
                    int targetCurRank = -1;
                    int targetNewerRank = -1;
                    if (Misc.FindClient.FindClientByID(target.Instance, out t_demote))
                    {
                        //First we check if target is in the same org as you
                        if (t_demote.Character.orgId != client.Character.orgId)
                        {
                            //not in same org
                            client.SendChatText("Target is not in your organization!");
                            break;
                        }
                        //Target is in same org, are you eligible to demote?  Promoter Rank has to be TargetRank-2 or == 0
                        if ((client.Character.Stats.GmLevel.Value == (t_demote.Character.Stats.ClanLevel.Value - 2)) || (client.Character.Stats.ClanLevel.Value == 0))
                        {
                            //Promoter is eligible. Start the process

                            //First we get the details about the org itself
                            demoteSql = "SELECT GovernmentForm FROM organizations WHERE ID = " + client.Character.orgId;
                            dt = ms.ReadDT(demoteSql);
                            int demoteGovForm = -1;
                            string demotedToRank = "";
                            if (dt.Rows.Count > 0)
                            {
                                demoteGovForm = (Int32)dt.Rows[0]["GovernmentForm"];
                            }

                            //Check whether new rank would be lower than lowest for current govform
                            if ((targetCurRank + 1) > GetLowestRank(demoteGovForm))
                            {
                                client.SendChatText("You can't demote character any lower!");
                                break;
                            }
                            targetCurRank = t_demote.Character.Stats.GmLevel.Value;
                            targetNewerRank = targetCurRank + 1;
                            demotedToRank = GetRank(demoteGovForm, (uint)targetNewerRank);
                            t_demote.Character.Stats.ClanLevel.Set(targetNewerRank);
                            client.SendChatText("You've demoted " + t_demote.Character.Name + " to " + demotedToRank);
                            t_demote.SendChatText("You've been demoted to the rank of " + demotedToRank + " by " + client.Character.Name);
                            break;
                        }
                        else
                        {
                            //Promoter not eligible to promote
                            client.SendChatText("Your Rank is not high enough to demote " + t_demote.Character.Name);
                            break;
                        }
                    }
                    break;
                #endregion

                #region unknown org command 12
                case 12:
                    Console.WriteLine("Case 12 Started");
                    break;
                #endregion

                #region /org kick <name>
                case 13:
                    // kick <name> from org
                    // <name> is CmdStr

                    //create the t_player Client namespace, using CmdStr to find character id, in replacement of target.Instance
                    uint kickedFrom = client.Character.orgId;
                    string kickeeSql = "SELECT * FROM characters WHERE Name = '" + CmdStr + "'";
                    int kickeeId = 0;
                    dt = ms.ReadDT(kickeeSql);
                    if (dt.Rows.Count > 0)
                    {
                        kickeeId = (Int32)dt.Rows[0]["ID"];
                    }

                    Client target_player = null;
                    if (Misc.FindClient.FindClientByID(kickeeId, out target_player))
                    {
                        //Check if CmdStr is actually part of the org
                        uint kickeeOrgId = target_player.Character.orgId;
                        if (kickeeOrgId != client.Character.orgId)
                        {
                            //Not part of Org. break out.
                            client.SendChatText(CmdStr + "is not a member of your organization!");
                            break;
                        }

                        //They are part of the org, so begin the processing...
                        //First we check if the player is online...
                        string onlineSql = "SELECT online FROM characters WHERE ID = " + client.Character.ID;
                        dt = ms.ReadDT(onlineSql);
                        int onlineStatus = 0;
                        if (dt.Rows.Count > 0)
                        {
                            onlineStatus = (Int32)dt.Rows[0][0];
                        }

                        if (onlineStatus == 0)
                        {
                            //Player isn't online. Org Kicks are processed in a different method
                            break;
                        }

                        //Player is online. Start the kick.
                        target_player.Character.Stats.ClanLevel.Set(0);
                        target_player.Character.orgId = 0;
                        string kickedFromSql = "SELECT Name FROM organizations WHERE ID = " + client.Character.orgId;
                        dt = ms.ReadDT(kickedFromSql);
                        string KickedFromName = "";
                        if (dt.Rows.Count > 0)
                        {
                            KickedFromName = (string)dt.Rows[0][0];
                        }
                        target_player.SendChatText("You've been kicked from the organization " + KickedFromName);

                    }
                    break;
                #endregion

                #region /org invite
                case 14:
                    {
                        Client t_player = null;
                        if (Misc.FindClient.FindClientByID(target.Instance, out t_player))
                        {
                            PacketWriter writer = new PacketWriter();
                            writer.PushBytes(new byte[] { 0xDF, 0xDF });
                            writer.PushShort(10);
                            writer.PushShort(1);
                            writer.PushShort(0);
                            writer.PushInt(3086); //Sender
                            writer.PushInt(t_player.Character.ID);  //Receiver
                            writer.PushInt(0x64582A07); //Packet ID
                            writer.PushIdentity(50000, t_player.Character.ID); //Target Identity
                            writer.PushByte(0);
                            writer.PushByte(5); //OrgServer Case 0x05 (Invite)
                            writer.PushInt(0);
                            writer.PushInt(0);
                            writer.PushIdentity(0xDEAA, (int)client.Character.orgId); // Type (org)
                            writer.PushShort((short)client.Character.orgName.Length);
                            writer.PushBytes(Encoding.ASCII.GetBytes(client.Character.orgName));
                            writer.PushInt(0);
                            byte[] reply = writer.Finish();

                            t_player.SendCompressed(reply);
                        }
                    }
                    break;
                #endregion

                #region Org Join
                case 15:
                    {
                        //target.Instance holds the OrgID of the Org wishing to be joined.
                        int orgIdtoJoin = target.Instance;
                        string JoinSql = "SELECT * FROM organizations WHERE ID = '" + orgIdtoJoin + "' LIMIT 1";
                        int gov_form = 0;
                        dt = ms.ReadDT(JoinSql);
                        if (dt.Rows.Count > 0)
                        {
                            gov_form = (Int32)dt.Rows[0]["GovernmentForm"];
                        }

                        // Make sure the order of these next two lines is not swapped -NV
                        client.Character.Stats.ClanLevel.Set(GetLowestRank(gov_form));
                        client.Character.orgId = (uint)orgIdtoJoin;
                    }
                    break;
                #endregion

                #region /org leave
                case 16:
                    // org leave
                    // TODO: Disband org if it was leader that left org. -Suiv-
                    // I don't think a Disband happens if leader leaves. I don't think leader -can- leave without passing lead to another
                    // Something worth testing on Testlive perhaps ~Chaz
                    // Just because something happens on TL, doesnt mean its a good idea. Really tbh id prefer it if you had to explicitly type /org disband to disband rather than /org leave doing it... -NV
                    // Agreeing with NV.  Org Leader can't leave without passing lead on.  org disband requires /org disband to specifically be issued, with a Yes/No box.
                    string LeaveSql = "SELECT * FROM organizations WHERE ID = " + client.Character.orgId;
                    int govern_form = 0;
                    dt = ms.ReadDT(LeaveSql);
                    if (dt.Rows.Count > 0)
                    {
                        govern_form = (Int32)dt.Rows[0]["GovernmentForm"];
                    }

                    if ((client.Character.Stats.ClanLevel.Value == 0) && (govern_form != 4))
                    {
                        client.SendChatText("Organization Leader cannot leave organization without Disbanding or Passing Leadership!");
                    }
                    else
                    {
                        client.Character.orgId = 0;
                        client.SendChatText("You left the guild");

                    }
                    break;
                #endregion

                #region /org tax | /org tax <tax>
                case 17:
                    // gets or sets org tax
                    // <tax> is CmdStr
                    // if no <tax>, then just send chat text with current tax info

                    if (CmdStr == null)
                    {
                        client.SendChatText("The current organization tax rate is: ");
                        break;
                    }
                    else
                    {
                        break;
                    }
                #endregion

                #region /org bank
                case 18:
                    {
                        // org bank
                        dt = ms.ReadDT("SELECT * FROM organizations WHERE ID=" + client.Character.orgId);
                        if (dt.Rows.Count > 0)
                        {
                            UInt64 bank_credits = (UInt64)dt.Rows[0]["Bank"];
                            client.SendChatText("Your bank has " + bank_credits + " credits in its account");
                        }
                    }
                    break;
                #endregion

                #region /org bank add <cash>
                case 19:
                    {
                        if (client.Character.orgId == 0)
                        {
                            client.SendChatText("You are not in an organisation.");

                            break;
                        }

                        // org bank add <cash>
                        int minuscredits_fromplayer = Convert.ToInt32(CmdStr);
                        int characters_credits = client.Character.Stats.Cash.Value;

                        if (characters_credits < minuscredits_fromplayer)
                        {
                            client.SendChatText("You do not have enough Credits");
                        }
                        else
                        {
                            int total_Creditsspent = characters_credits - minuscredits_fromplayer;
                            client.Character.Stats.Cash.Set(total_Creditsspent);

                            ms.SqlUpdate("UPDATE `organizations` SET `Bank` = `Bank` + " + minuscredits_fromplayer + " WHERE `ID` = " + client.Character.orgId);
                            client.SendChatText("You have donated " + minuscredits_fromplayer + " to the organization");
                        }
                    }

                    break;
                #endregion

                #region /org bank remove <cash>
                case 20:
                    // org bank remove <cash>
                    // <cash> is CmdStr
                    // player wants to take credits from org bank
                    // only leader can do that
                    if ((client.Character.Stats.ClanLevel.Value != 0) || (client.Character.orgId == 0))
                    {
                        client.SendChatText("You're not the leader of an Organization");
                        break;
                    }
                    int remove_credits = Convert.ToInt32(CmdStr);
                    long org_bank = 0;
                    dt = ms.ReadDT("SELECT Bank FROM organizations WHERE ID = " + client.Character.orgId);
                    if (dt.Rows.Count > 0)
                    {
                        org_bank = (Int64)dt.Rows[0][0];
                    }
                    if (remove_credits > org_bank)
                    {
                        client.SendChatText("Not enough credits in Organization Bank!");
                        break;
                    }
                    else
                    {
                        long neworgbank = org_bank - remove_credits;
                        int existingcreds = 0;
                        existingcreds = client.Character.Stats.Cash.Value;
                        int newcreds = existingcreds + remove_credits;
                        ms.SqlUpdate("UPDATE organizations SET Bank = " + neworgbank + " WHERE ID = " + client.Character.orgId);
                        client.Character.Stats.Cash.Set(newcreds);
                        client.SendChatText("You've removed " + remove_credits + " credits from the organization bank");
                    }
                    break;
                #endregion

                #region /org bank paymembers <cash>
                case 21:
                    // <cash> is CmdStr
                    // give <cash> credits to every org member
                    // credits are taken from org bank
                    // only leader can do it
                    break;
                #endregion

                #region /org debt
                case 22:
                    // send player text about how big is his/her tax debt to org
                    break;
                #endregion

                #region /org history <text>
                case 23:
                    {
                        if (client.Character.Stats.ClanLevel.Value == 0)
                        {
                            // org history <history text>
                            ms.SqlUpdate("UPDATE organizations SET history = '" + CmdStr + "' WHERE ID = '" + client.Character.orgId + "'");
                            client.SendChatText("History Updated");
                        }
                        else { client.SendChatText("You must be the Organization Leader to perform this command!"); }
                    }
                    break;
                #endregion

                #region /org objective <text>
                case 24:
                    {
                        if (client.Character.Stats.ClanLevel.Value == 0)
                        {
                            // org objective <objective text>
                            ms.SqlUpdate("UPDATE organizations SET objective = '" + CmdStr + "' WHERE ID = '" + client.Character.orgId + "'");
                            client.SendChatText("Objective Updated");
                        }
                        else { client.SendChatText("You must be the Organization Leader to perform this command!"); }
                    }
                    break;
                #endregion

                #region /org description <text>
                case 25:
                    {
                        if (client.Character.Stats.ClanLevel.Value == 0)
                        {
                            // org description <description text>
                            ms.SqlUpdate("UPDATE organizations SET description = '" + CmdStr + "' WHERE ID = '" + client.Character.orgId + "'");
                            client.SendChatText("Description Updated");
                        }
                        else { client.SendChatText("You must be the Organization Leader to perform this command!"); }
                    }
                    break;
                #endregion

                #region /org name <text>
                case 26:
                    {
                        // org name <name>
                        /* Renames Organization
                         * Checks for Existing Orgs with similar name to stop crash
                         * Chaz
                         */
                        if (client.Character.Stats.ClanLevel.Value == 0)
                        {
                            string SqlQuery26 = "SELECT * FROM organizations WHERE Name LIKE '" + CmdStr + "' LIMIT 1";
                            string CurrentOrg = null;
                            dt = ms.ReadDT(SqlQuery26);
                            if (dt.Rows.Count > 0)
                            {
                                CurrentOrg = (string)dt.Rows[0]["Name"];
                            }

                            if (CurrentOrg == null)
                            {
                                string SqlQuery27 = "UPDATE organizations SET Name = '" + CmdStr + "' WHERE ID = '" + client.Character.orgId + "'";
                                ms.SqlUpdate(SqlQuery27);
                                client.SendChatText("Organization Name Changed to: " + CmdStr);

                                // Forces reloading of org name and the like
                                // XXXX TODO: Make it reload for all other members in the org
                                client.Character.orgId = client.Character.orgId;
                                break;
                            }
                            else
                            {
                                client.SendChatText("An Organization already exists with that name");
                                break;
                            }
                        }
                        else { client.SendChatText("You must be the organization leader to perform this command!"); }
                        break;
                    }
                #endregion

                #region /org governingform <text>
                case 27:
                    {
                        // org governingform <form>
                        /* Current Governing Forms:
                         * Department, Faction, Republic, Monarchy, Anarchism, Feudalism
                         */
                        //Check on whether your President or not
                        if (client.Character.Stats.ClanLevel.Value == 0)
                        {
                            //first we drop the case on the input, just to be sure.
                            Int32 GovFormNum = -1;
                            if (CmdStr == null)
                            {
                                //list gov forms
                                client.SendChatText("List of Accepted Governing Forms is: department, faction, republic, monarchy, anarchism, feudalism");
                                break;
                            }
                            //was correct input passed?
                            switch (CmdStr.ToLower())
                            {
                                case "department":
                                    GovFormNum = 0;
                                    break;
                                case "faction":
                                    GovFormNum = 1;
                                    break;
                                case "republic":
                                    GovFormNum = 2;
                                    break;
                                case "monarchy":
                                    GovFormNum = 3;
                                    break;
                                case "anarchism":
                                    GovFormNum = 4;
                                    break;
                                case "feudalism":
                                    GovFormNum = 5;
                                    break;
                                default:
                                    client.SendChatText(CmdStr + " Is an invalid Governing Form!");
                                    client.SendChatText("Accepted Governing Forms are: department, faction, republic, monarchy, anarchism, feudalism");
                                    break;
                            }
                            if (GovFormNum != -1)
                            {
                                ms.SqlUpdate("UPDATE organizations SET GovernmentForm = '" + GovFormNum + "' WHERE ID = '" + client.Character.orgId + "'");
                                foreach (int currentCharId in OrgMisc.GetOrgMembers(client.Character.orgId, true))
                                {
                                    client.Character.Stats.ClanLevel.Set(GetLowestRank(GovFormNum));
                                }
                                client.SendChatText("Governing Form is now: " + CmdStr);
                                break;
                            }
                        }
                        else
                        {
                            //Haha! You're not the org leader!
                            client.SendChatText("You must be the Org Leader to perform this command");
                            break;
                        }
                    }
                    break;
                #endregion

                #region /org stopvote <text>
                case 28:
                    // <text> is CmdStr
                    break;
                #endregion

                #region unknown command
                default:
                    break;
                #endregion
            }
            #endregion

            reader.Finish();
        }
        internal static string GetRankList(int GoverningForm)
        {
            string Department = "President, General, Squad Commander, Unit Commander, Unit Leader, Unit Member, Applicant";
            string Faction = "Director, Board Member, Executive, Member, Applicant";
            string Republic = "President, Advisor, Veteran, Member, Applicant";
            string Monarchy = "Monarch, Council, Follower";
            string Anarchism = "Anarchist";
            string Feudalism = "Lord, Knight, Vassal, Peasant";

            switch (GoverningForm)
            {
                case 0:
                    return Department;
                case 1:
                    return Faction;
                case 2:
                    return Republic;
                case 3:
                    return Monarchy;
                case 4:
                    return Anarchism;
                case 5:
                    return Feudalism;
                default:
                    return "";
            }
        }
        internal static string GetRank(int GoverningForm, uint Rank)
        {
            string[] Department = { "President", "General", "Squad Commander", "Unit Commander", "Unit Leader", "Unit Member", "Applicant" };
            string[] Faction = { "Director", "Board Member", "Executive", "Member", "Applicant" };
            string[] Republic = { "President", "Advisor", "Veteran", "Member", "Applicant" };
            string[] Monarchy = { "Monarch", "Council", "Follower" };
            string[] Anarchism = { "Anarchist" };
            string[] Feudalism = { "Lord", "Knight", "Vassal", "Peasant" };

            switch (GoverningForm)
            {
                case 0:
                    if (Rank > 6) { return ""; }
                    return Department[Rank];
                case 1:
                    if (Rank > 4) { return ""; }
                    return Faction[Rank];
                case 2:
                    if (Rank > 4) { return ""; }
                    return Republic[Rank];
                case 3:
                    if (Rank > 2) { return ""; }
                    return Monarchy[Rank];
                case 4:
                    if (Rank > 0) { return ""; }
                    return Anarchism[Rank];
                case 5:
                    if (Rank > 3) { return ""; }
                    return Feudalism[Rank];
                default:
                    // 	wrong governingform (too high number)
                    return "";
            }
        }
        internal static int GetLowestRank(int GoverningForm)
        {
            switch (GoverningForm)
            {
                case 0: return 6;
                case 1: return 4;
                case 2: return 4;
                case 3: return 2;
                case 4: return 0;
                case 5: return 3;
                default: return 0;
            }
        }
    }
}
