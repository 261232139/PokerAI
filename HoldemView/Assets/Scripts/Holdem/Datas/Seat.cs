using System;
using System.Collections.Generic;

namespace Holdem.Datas
{
    [System.Serializable]
    public class Seat
    {
        public int seatIndex;          // 座位号 (0-8)
        public string positionName;     // 位置名 (BTN, SB, BB, UTG...)
        public PlayerInfo player;       // 引用你的 PlayerInfo 类
        public float currentStack;      // 当前筹码量
        public bool isHero;             // 是否是“我”
        public HoldemPlayerAction lastAction;       // 本轮动作记录 (如: "Raise to 10bb")
        public bool hasFolded;          // 是否已弃牌
    }

    public class HoldemPlayerAction
    {
        public HoldemPlayerActionType ActionType { get; set; }
        public float Amount { get; set; } // 仅在 Bet 和 Raise 时使用
    }

    public enum HoldemPlayerActionType
    {
        Fold,
        Check,
        Call,
        Bet,
        Raise,
        AllIn
    }
}
