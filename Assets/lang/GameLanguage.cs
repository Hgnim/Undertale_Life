using System.Collections;
using System.Collections.Generic;
using UnityEngine.Localization.Settings;
public static class GameLang
{
  /// <summary>
  /// ��ȡ�����ļ��ڵ��ı�
  /// </summary>
  /// <param name="key">�ı�����Ӧ��ID</param>
  /// <param name="table">�����ļ���<br/>��ѡ����</param>
  /// <returns>ͨ��ָ����ID���ض�Ӧ���ı�</returns>
  public static string GetString(string key,string table="game_language")
    {
        return LocalizationSettings.StringDatabase.GetTable(table).GetEntry(key).GetLocalizedString();
    }
}