using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

public class StringUtils
{

    /// <summary>
    /// 数据转化成字符串
    /// </summary>
    /// <param name="list"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string GetPrettyFormatStringOfList<T>(List<T> list)
    {
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.Append("[");
        foreach (T value in list)
        {
            strBuilder.Append(value.ToString() + ", ");
        }

        strBuilder.Remove(strBuilder.Length - 2, 2);
        strBuilder.Append("]");
        return strBuilder.ToString();
    }

    public static string UTF8ByteArrayToString(byte[] characters)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        string constructedString = encoding.GetString(characters);
        return (constructedString);
    }

    public static byte[] StringToUTF8ByteArray(String pXmlString)
    {
        UTF8Encoding encoding = new UTF8Encoding();
        byte[] byteArray = encoding.GetBytes(pXmlString);
        return byteArray;
    }

    /// <summary>
    /// string转为int
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defult"></param>
    /// <returns></returns>
    public static int GetInt(string str, int defult = 0)
    {
        int value = defult;
        if (!int.TryParse(str, out value))
        {
            value = defult;
        }

        return value;
    }

    /// <summary>
    /// string转为floot
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defult"></param>
    /// <returns></returns>
    public static float GetFloat(string str, float defult = 0)
    {
        float value = defult;
        if (!float.TryParse(str, out value))
        {
            value = defult;
        }

        return value;
    }

    /// <summary>
    /// string转为floot
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defult"></param>
    /// <returns></returns>
    public static double GetDouble(string str, double defult = 0)
    {
        double value = defult;
        if (!double.TryParse(str, out value))
        {
            value = defult;
        }

        return value;
    }

    /// <summary>
    /// string转为bool
    /// </summary>
    /// <param name="str"></param>
    /// <param name="defult"></param>
    /// <returns></returns>
    public static bool GetBool(string str, bool defult = false)
    {
        bool value = defult;
        if (!bool.TryParse(str, out value))
        {
            value = defult;
        }

        return value;
    }


    /// <summary>
    /// 根据卡牌ID 获取小写字符串
    /// </summary>
    /// <param name="UnitId"></param>
    /// <returns></returns>
    public static string GetASCIIStringLower(int UnitId)
    {
        byte[] array = new byte[1];
        array[0] = (byte)(Convert.ToInt32(96 + UnitId)); //ASCII码强制转换二进制
        string str = Convert.ToString(Encoding.ASCII.GetString(array));

        return str;
    }


    /// <summary>
    /// string转为DateTime
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static DateTime GetDateTime(string str)
    {
        DateTime temp = DateTime.Now;

        if (!string.IsNullOrEmpty(str))
        {
            DateTime.TryParse(str, out temp);
        }

        return temp;
    }


    /// <summary>
    /// 获得utf8字符串
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static string GetStringFromUTF8Bytes(byte[] bytes)
    {
        string info = Encoding.UTF8.GetString(bytes);

        return info;
    }

    // 去掉字符中的换行，空格，tab
    public static string RemoveSpace(string strContent)
    {
        strContent.Replace("\t", "")
            .Replace("\n", "")
            .Replace("\r", "");

        return strContent;
    }

    /// <summary>
    /// 版本号比较
    /// 支持两拉版本的版本号比较. (1.0, 1.2)
    /// 1: 代表ver1 > ver2
    /// -1: 代表ver1 < ver2
    /// 0: 两个版本号一样
    /// </summary>
    /// <param name="value0"></param>
    /// <param name="value1"></param>
    /// <returns></returns>
    public static int CompareVersion(string value0, string value1)
    {
        int state = 0;

        string[] valueArray0 = value0.Split('.');
        string[] valueArray1 = value1.Split('.');

        int value00 = int.Parse(valueArray0[0]);
        int value10 = int.Parse(valueArray1[0]);

        if (value00 > value10)
        {
            state = 1;
        }
        else if (value00 < value10)
        {
            state = -1;
        }
        else
        {
            if (valueArray0.Length == 1)
            {
                state = -1;
            }
            else if (valueArray1.Length == 1)
            {
                state = 1;
            }
            else
            {
                int value01 = int.Parse(valueArray0[1]);
                int value11 = int.Parse(valueArray1[1]);

                if (value01 > value11)
                {
                    state = 1;
                }
                else if (value01 < value11)
                {
                    state = -1;
                }
                else
                {
                    state = 0;
                }
            }
        }

        return state;
    }
    public static bool SetValueListDataByString<T>(string strContent, ref List<T> targetList, string splitChar = ",")
    {
        bool anyNoSupportType = false;
        List<string> strList = SplitStringToStringList(strContent, splitChar);
        if (strList != null && strList.Count > 0)
        {
            for (int i = 0; i < strList.Count; i++)
            {
                //分类型处理string
                if (typeof(T) == typeof(string))
                {
                    targetList.Add((T)(object)strList[i]);
                }
                //int
                else if (typeof(T) == typeof(int))
                {
                    targetList.Add((T)(object)int.Parse(strList[i]));
                }
                //float
                else if (typeof(T) == typeof(float))
                {
                    targetList.Add((T)(object)float.Parse(strList[i], System.Globalization.CultureInfo.InvariantCulture));
                }
                //jsonnode
                else if (typeof(T) == typeof(JObject))
                {
                    targetList.Add((T)(object)strList[i]);
                }
                //other 暂不支持
                else
                {
                    anyNoSupportType = true;
                }
            }
        }

        return anyNoSupportType;
    }

    /// <summary>
    /// 拿到数据列表
    /// </summary>
    public static List<List<T>> GetListValueListByKey<T>(string str, string layer0SplitChar = ";", string layer1SplitChar = ",")
    {
        List<List<T>> targetList = new List<List<T>>();
        if (!string.IsNullOrEmpty(str))
        {
            List<string> splits = SplitStringToStringList(str, layer0SplitChar);


            if (splits != null && splits.Count > 0)
            {
                for (int i = 0; i < splits.Count; i++)
                {
                    List<T> tmp = new List<T>();
                    bool anyNoSupportType = SetValueListDataByString(splits[i], ref tmp, layer1SplitChar);
                    if (anyNoSupportType)
                    {
                        GameDebugLog.LogError(string.Format(
                            "ToolString.GetListValueListByKey error only type of \"string\" \"int\" \"float\" is support {0}",
                            str));
                    }
                    targetList.Add(tmp);
                }
            }
        }


        if (targetList.Count <= 0)
        {
            GameDebugLog.LogError(string.Format(
                "error in ToolString.GetListValueListByKey At least one must be supported name:{0}",
                str));
        }

        return targetList;
    }

    /// <summary>
    /// 分隔字符串为List<int>
    /// </summary>
    public static List<string> SplitStringToStringList(string strContent, string strSplit = ",")
    {
        string[] l_strSplitList;
        List<string> l_targetList = new List<string>();
        l_strSplitList =
            strContent.Split(new string[] { strSplit }, System.StringSplitOptions.RemoveEmptyEntries);
        foreach (string l_str in l_strSplitList)
        {
            l_targetList.Add(l_str);
        }

        return l_targetList;
    }

}
