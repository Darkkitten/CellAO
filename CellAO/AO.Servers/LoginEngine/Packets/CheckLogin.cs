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

namespace LoginEngine.Packets
{
    using System;

    using AO.Core;

    using LoginEngine.QueryBase;

    public class CheckLogin
    {
        /// <summary>
        /// 
        /// </summary>
        private int IFalse;

        #region Query Setup...
        /// <summary>
        /// 
        /// </summary>
        private readonly LoginName ln = new LoginName();

        /// <summary>
        /// 
        /// </summary>
        private readonly LoginFlags lf = new LoginFlags();

        /// <summary>
        /// 
        /// </summary>
        private readonly LoginPasswd lp = new LoginPasswd();
        #endregion

        #region Check To See If The Player is allowed to login (Not banned/etc)...
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="accountName"></param>
        /// <returns></returns>
        public bool IsLoginAllowed(Client client, string accountName)
        {
            if (accountName.ToLower() != client.AccountName.ToLower())
            {
                return false;
            }

            this.ln.GetLoginName(accountName);
            this.lf.GetLoginFlags(accountName);

            if (this.ln.LoginN != null && accountName.ToLower() == this.ln.LoginN.ToLower()
                && this.lf.FlagsL == this.IFalse)
            {
                return true; // Login OK
            }
            else
            {
                return false; // Login Not Permitted
            }
        }
        #endregion

        #region Check to See if The Password matches and then send Character List...
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="loginKey"></param>
        /// <returns></returns>
        public bool IsLoginCorrect(Client client, string loginKey)
        {
            LoginEncryption le = new LoginEncryption();

            this.lp.GetLoginPassword(client.AccountName);

            return le.IsValidLogin(loginKey, client.ServerSalt, client.AccountName, this.lp.PasswdL);
        }
        #endregion

        #region Check to See if the Character the client is trying to use is on the account or not
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="characterId"></param>
        /// <returns></returns>
        public bool IsCharacterOnAccount(Client client, int characterId)
        {
            LoginEncryption le = new LoginEncryption();

            return le.IsCharacterOnAccount(client.AccountName, (UInt32)characterId);
        }
        #endregion
    }
}