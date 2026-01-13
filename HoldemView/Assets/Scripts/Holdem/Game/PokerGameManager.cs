using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Holdem.Datas;

public class PokerGameManager : MonoBehaviour
{
    public static PokerGameManager Instance;

    [Header("盲注配置")]
    public int smallBlind = 1;
    public int bigBlind = 2;
    public int ante = 0;

    [Header("桌态数据")]
    public List<Seat> seats = new List<Seat>(new Seat[9]); // 假设最大9人桌
    public List<CardId> communityCards = new List<CardId>();
    public List<CardId> heroHand = new List<CardId>();
    public int potSize = 0;
    public int buttonIndex = 0;

    private void Awake() => Instance = this;

    // --- 1. 修改基础配置 ---
    public void SetBlinds(int sb, int bb, int a = 0)
    {
        smallBlind = sb;
        bigBlind = bb;
        ante = a;
    }

    // --- 2. 座位管理 ---
    public void JoinTable(int index, PlayerInfo info, int stack, bool isHero, string posName)
    {
        seats[index] = new Seat
        {
            seatIndex = index,
            player = info,
            currentStack = stack,
            isHero = isHero,
            positionName = posName,
            hasFolded = false
        };
    }

    public void LeaveTable(int index) => seats[index] = null;

    // --- 3 & 5. 决策记录 ---
    public void SetPlayerAction(int seatIndex, HoldemPlayerActionType type, float amount = 0)
    {
        var seat = seats[seatIndex];
        if (seat == null) return;

        seat.lastAction = new HoldemPlayerAction { ActionType = type, Amount = amount };

        if (type == HoldemPlayerActionType.Fold) seat.hasFolded = true;

        // 自动更新底池（简化逻辑，实际开发需考虑具体筹码扣除）
        potSize += (int)amount;
        Debug.Log($"座位 {seatIndex} 执行了 {type}，金额: {amount}");
    }

    // --- 4. 发牌 (Pre-flop) ---
    public void DealHoleCards(List<CardId> cards)
    {
        heroHand = cards;
        communityCards.Clear();
        potSize = (int)(smallBlind + bigBlind + (ante * seats.Count(s => s != null)));
        // 清除上一局动作
        foreach (var s in seats.Where(s => s != null)) s.lastAction = null;
    }

    // --- 6, 7, 8. 公共牌推进 ---
    public void DealFlop(CardId c1, CardId c2, CardId c3) => communityCards.AddRange(new[] { c1, c2, c3 });
    public void DealTurn(CardId c1) => communityCards.Add(c1);
    public void DealRiver(CardId c1) => communityCards.Add(c1);

    // --- 9. Showdown 结算 ---
    public void Showdown(List<PlayerShowdownData> allShowdowns)
    {
        foreach (var data in allShowdowns)
        {
            Debug.Log($"玩家 {data.seatIndex} 亮牌: {string.Join(",", data.holeCards)}");
            // 这里可以接入之前提到的复盘逻辑
        }
    }

    // --- 获取当前 JSON 快照 (用于发给 DeepSeek) ---
    public string GetCurrentStateJson()
    {
        var data = new
        {
            blinds = new { sb = smallBlind, bb = bigBlind, ante = ante },
            hero = new { hand = heroHand.Select(c => c.ToShortString()).ToArray() },
            board = communityCards.Select(c => c.ToShortString()).ToArray(),
            pot = potSize,
            players = seats.Where(s => s != null).Select(s => new
            {
                pos = s.positionName,
                is_hero = s.isHero,
                folded = s.hasFolded,
                stack = s.currentStack,
                last_act = s.lastAction?.ActionType.ToString() ?? "None",
                last_amt = s.lastAction?.Amount ?? 0,
                stats = s.player // 包含你定义的 VPIP, PFR 等
            }).ToArray()
        };
        return Newtonsoft.Json.JsonConvert.SerializeObject(data);
    }
}