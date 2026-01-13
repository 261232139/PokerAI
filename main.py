import os
from dotenv import load_dotenv
from openai import OpenAI
from utils import get_hand_strength
import httpx

load_dotenv()
client = OpenAI(
    api_key=os.getenv("DEEPSEEK_API_KEY"),
    base_url="https://api.deepseek.com",
)

def start_poker_assistant():
    print("=== 欢迎使用 DeepSeek R1 德州扑克专业分析器 ===")
    
    while True:
        print("\n--- 请输入当前牌局信息 (输入 'quit' 退出) ---")
        # 1. 采集手牌信息
        my_hand_str = input("请输入你的手牌 (例如: As,Ad): ").split(',')
        board_str = input("请输入公共牌 (例如: Qh,Jd,10s): ").split(',')
        
        # 2. 采集局势信息
        pot_size = input("当前底池大小: ")
        to_call = input("你需要跟注多少: ")
        
        # 3. 计算基础强度
        strength = get_hand_strength(my_hand_str, board_str)
        print(f"\n[系统评估] 当前绝对强度: {strength}")
        
        # 4. 调用 AI 深度思考
        print("\n[AI 正在进行 GTO 推理...] ")
        try:
            response = client.chat.completions.create(
                model="deepseek-reasoner",
                messages=[
                    {"role": "system", "content": "你是一个顶级的德州扑克策略专家，擅长 GTO 和对手心理剥削。"},
                    {"role": "user", "content": f"手牌:{my_hand_str}, 公共牌:{board_str}, 强度:{strength}, 底池:{pot_size}, 需跟注:{to_call}。请给出决策。"}
                ]
            )
            
            # 展示推理过程（德扑的乐趣就在于看 AI 怎么算概率）
            print("\n--- AI 思考路径 ---")
            print(response.choices[0].message.reasoning_content)
            
            print("\n--- 最终决策建议 ---")
            print(response.choices[0].message.content)
            
        except Exception as e:
            print(f"调用失败: {e}")

if __name__ == "__main__":
    start_poker_assistant()