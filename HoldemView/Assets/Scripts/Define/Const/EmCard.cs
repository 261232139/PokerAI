using System;

/// <summary>
/// 德州扑克 52 张牌枚举
/// 编码规则：0-12 黑桃, 13-25 红桃, 26-38 方片, 39-51 梅花
/// </summary>
public enum CardId
{
    None = -1,

    // 黑桃 (Spades) - S
    As = 0, Ks = 1, Qs = 2, Js = 3, Ts = 4, s9 = 5, s8 = 6, s7 = 7, s6 = 8, s5 = 9, s4 = 10, s3 = 11, s2 = 12,

    // 红桃 (Hearts) - H
    Ah = 13, Kh = 14, Qh = 15, Jh = 16, Th = 17, h9 = 18, h8 = 19, h7 = 20, h6 = 21, h5 = 22, h4 = 23, h3 = 24, h2 = 25,

    // 方片 (Diamonds) - D
    Ad = 26, Kd = 27, Qd = 28, Jd = 29, Td = 30, d9 = 31, d8 = 32, d7 = 33, d6 = 34, d5 = 35, d4 = 36, d3 = 37, d2 = 38,

    // 梅花 (Clubs) - C
    Ac = 39, Kc = 40, Qc = 41, Jc = 42, Tc = 43, c9 = 44, c8 = 45, c7 = 46, c6 = 47, c5 = 48, c4 = 49, c3 = 50, c2 = 51
}

/// <summary>
/// 花色枚举
/// </summary>
public enum CardSuit
{
    Spades = 0,   // 黑桃
    Hearts = 1,   // 红桃
    Diamonds = 2, // 方片
    Clubs = 3     // 梅花
}