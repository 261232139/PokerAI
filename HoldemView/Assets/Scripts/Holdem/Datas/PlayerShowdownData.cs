using System;
using System.Collections.Generic;

namespace Holdem.Datas
{
    [System.Serializable]
    public class PlayerShowdownData
    {
        public int seatIndex;          // 座位号
        public List<CardId> holeCards;   // 手牌
    }
}
