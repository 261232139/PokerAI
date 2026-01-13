using System;
using System.Collections.Generic;

namespace Holdem.Datas
{
    public class PlayerInfo
    {
        public string playerId;
        public float vpip; // Voluntarily Put Money In Pot 主动入池率
        public float pfr;  // Pre-Flop Raise 翻牌前加注率
        public float threeBetFrequency; //3B频率
        public float foldToThreeBet; //面对3-Bet的弃牌率
        public float cBetFrequency; //持续下注率
        public float foldToCBet; //面对C-Bet的弃牌率

        public PlayerInfo(string playerId)
        {
            this.playerId = playerId;
        }
    }
}
