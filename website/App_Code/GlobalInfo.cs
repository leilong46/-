using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// GlobalInfo 的摘要说明
/// </summary>
public static class GlobalInfo
{
    public static string AppID;
    public static string Secret;
    public static string MaketID;
    public static string Token;
    public static Boolean IsTokenValidate(string appID)
    {
        return String.Equals(AppID,appID) && !string.IsNullOrWhiteSpace(Token);
    }
    static public void SetToken(string appID,string token)
    {
        AppID = appID;
        Token = token;
    }
}