public static class CardUtils
{
    /// <summary>
    /// 将 CardId 转换为 Python 后端识别的字符串 (例如 CardId.As -> "As")
    /// </summary>
    public static string ToShortString(this CardId card)
    {
        if (card == CardId.None) return "";

        string s = card.ToString();
        // 处理数字开头的特殊情况 (如 s9 -> 9s)
        if (s.StartsWith("s") || s.StartsWith("h") || s.StartsWith("d") || s.StartsWith("c"))
        {
            return s.Substring(1) + s.Substring(0, 1);
        }
        return s;
    }

    /// <summary>
    /// 获取花色
    /// </summary>
    public static CardSuit GetSuit(this CardId card)
    {
        return (CardSuit)((int)card / 13);
    }

    /// <summary>
    /// 获取点数 (0是A, 1是K... 12是2)
    /// </summary>
    public static int GetRank(this CardId card)
    {
        return (int)card % 13;
    }
}