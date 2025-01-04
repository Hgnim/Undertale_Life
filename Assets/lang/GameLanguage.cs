using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;
public static class GameLang
{
  /// <summary>
  /// 获取语言文件内的文本
  /// </summary>
  /// <param name="key">文本所对应的ID</param>
  /// <param name="table">语言文件名<br/>可选参数</param>
  /// <returns>通过指定的ID返回对应的文本</returns>
  public static string GetString(string key,string table="game_language")
    {
        return LocalizationSettings.StringDatabase.GetTable(table).GetEntry(key).GetLocalizedString();
    }
}