using System;
using System.Collections.Generic;

namespace Holdem.Datas
{
    public class HoldemHandsData
    {
        public int smallBlind = 1;
        public int bigBlind = 2;
        public int ante = 0;
        public List<PlayerInfo> playersInfo; // 玩家信息列表
    }
}
