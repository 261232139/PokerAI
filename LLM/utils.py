from treys import Card, Evaluator

def get_hand_strength(hand_list, board_list):
    """
    计算手牌强度
    hand_list: ['As', 'Ad']
    board_list: ['Qh', 'Jd', '10s']
    """
    evaluator = Evaluator()
    try:
        # 将字符串列表转换为 treys 的内部卡牌对象
        hand = [Card.new(c) for c in hand_list]
        board = [Card.new(c) for c in board_list]
        
        # 计算得分（分数越低牌力越强）
        rank = evaluator.evaluate(board, hand)
        
        # 获取百分比排名（0-1之间，1代表最强皇家同花顺）
        percentage = 1 - evaluator.get_five_card_rank_percentage(rank)
        
        # 获取牌型描述（如 "Full House"）
        description = evaluator.class_to_string(evaluator.get_rank_class(rank))
        
        return f"{description} (胜率百分比: {percentage:.2%})"
    except Exception as e:
        return f"计算出错: {str(e)}"