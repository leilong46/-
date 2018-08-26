using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using YZOpenSDK;

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
  

    public static HttpWebResponse CreatePostHttpResponse(string url, IDictionary<string, string> parameters, Encoding charset)
    {
        HttpWebRequest request = null;
        //HTTPSQ请求  
        //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
        request = WebRequest.Create(url) as HttpWebRequest;
        request.ProtocolVersion = HttpVersion.Version10;
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        //request.UserAgent = DefaultUserAgent;
        //如果需要POST数据     
        if (!(parameters == null || parameters.Count == 0))
        {
            StringBuilder buffer = new StringBuilder();
            int i = 0;
            foreach (string key in parameters.Keys)
            {
                if (i > 0)
                {
                    buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                }
                else
                {
                    buffer.AppendFormat("{0}={1}", key, parameters[key]);
                }
                i++;
            }
            byte[] data = charset.GetBytes(buffer.ToString());
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
        }
        return request.GetResponse() as HttpWebResponse;
    }

    public static void GetToken(string appId,string secret,string maket_id)
    {
        string url = "http://open.youzan.com/oauth/token";
        Encoding encoding = Encoding.GetEncoding("utf-8");
        IDictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("client_id", appId);
        parameters.Add("client_secret", secret);
        parameters.Add("grant_type", "silent");
        parameters.Add("kdt_id", maket_id);
        HttpWebResponse response = CreatePostHttpResponse(url, parameters, encoding);
        //打印返回值  
        Stream stream = response.GetResponseStream();   //获取响应的字符串流  
        StreamReader sr = new StreamReader(stream); //创建一个stream读取流  
        string html = sr.ReadToEnd();   //从头读到尾，放到字符串html  
        var jobj = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(html);
        var temToken = jobj["access_token"].ToString();
        if (!string.IsNullOrWhiteSpace(temToken))
        {
            AppID = appId;
            Secret = secret;
            Token = temToken;
        }
    }

    public static List<DiscountCardResultJson.DiscountCardResult> GetDiscountCardsInfo()
    {
        var QueryResult = new List<DiscountCardResultJson.DiscountCardResult>();

        Auth auth = new Token(Token);
        YZClient yzClient = new DefaultYZClient(auth);
        Dictionary<string, object> dict = new System.Collections.Generic.Dictionary<string, object>();
        //dict.Add("is_displays", "[1]");
        //dict.Add("measurement", 10);
        dict.Add("page", 1);

        var result = yzClient.Invoke("youzan.scrm.card.list", "3.0.0", "POST", dict, null);
        var jobj = (DiscountCardResultJson.Rootobject)Newtonsoft.Json.JsonConvert.DeserializeObject(result, typeof(DiscountCardResultJson.Rootobject));

        return jobj.Translate();
    }

    public static string GetProdustsInfoString(int PageSise, int currentPageIndex)
    {
        Auth auth = new Token(GlobalInfo.Token);
        YZClient yzClient = new DefaultYZClient(auth);
        Dictionary<string, object> dict = new System.Collections.Generic.Dictionary<string, object>();
        //dict.Add("is_displays", "[1]");
        //dict.Add("measurement", 10);
        dict.Add("page_no", currentPageIndex);
        dict.Add("page_size", PageSise);
        //dict.Add("show_sold_out", 0);
        dict.Add("source", "test");

        var result = yzClient.Invoke("youzan.retail.products.offline.search", "3.0.0", "POST", dict, null);

        return result;
    }
    public static string GetSingleProdustsInfo(string productId)
    {
        string url = "https://open.youzan.com/api/oauthentry/youzan.retail.product.offline/3.0.0/get";
        Encoding encoding = Encoding.GetEncoding("utf-8");
        IDictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("access_token", GlobalInfo.Token);

        parameters.Add("item_id", productId);
        parameters.Add("source", "test");
        HttpWebResponse response = CreatePostHttpResponse(url, parameters, encoding);
        //打印返回值  
        Stream stream = response.GetResponseStream();   //获取响应的字符串流  
        StreamReader sr = new StreamReader(stream); //创建一个stream读取流  
        string html = sr.ReadToEnd();   //从头读到尾，放到字符串html  
        return html;


    }

    public static List<ProductsSearchResultJson.ProductsSearchResult> GetProdustsInfo()
    {
        var inventories=GetProdustsInventoryInfo();

        int PageSise = 40;
        int currentPageIndex = 1;
        var QueryResult = new List<ProductsSearchResultJson.ProductsSearchResult>();

        string result = GlobalInfo.GetProdustsInfoString(PageSise, currentPageIndex);
        var jobj = (ProductsSearchResultJson.Rootobject)Newtonsoft.Json.JsonConvert.DeserializeObject(result, typeof(ProductsSearchResultJson.Rootobject));

            var temResult = jobj.Translate();
            StringBuilder sb = new StringBuilder();
            temResult.Aggregate(sb, (x, y) => x.Append(y.id + ","));
            var discountInfoList = QueryDiscountInfo(sb.ToString());
            foreach (var r in temResult)
            {
            var inv = inventories.SingleOrDefault(p => p.id == r.id);
            if(inv!=null)
            {
                r.成本价 = inv.成本价;
             }
                var discountInfo = discountInfoList.FirstOrDefault(p => p.id == r.id);
            if (discountInfo != null && discountInfo.会员价.Count > 0)
            {
                r.会员价 = discountInfo.会员价[0].discount_value / 100f;
                r.会员卡名称 = discountInfo.会员价[0].card_id.ToString();
                if (discountInfo.会员价.Count > 1)
                {
                    r.会员价2 = discountInfo.会员价[1].discount_value / 100f;
                    r.会员卡名称2 = discountInfo.会员价[1].card_id.ToString();
                    if (discountInfo.会员价.Count > 2)
                    {
                        r.会员价3 = discountInfo.会员价[2].discount_value / 100f;
                        r.会员卡名称3 = discountInfo.会员价[2].card_id.ToString();
                        if (discountInfo.会员价.Count > 3)
                        {
                            r.会员价4 = discountInfo.会员价[3].discount_value / 100f;
                            r.会员卡名称4 = discountInfo.会员价[3].card_id.ToString();
                        }
                    }
                }
            }
        }

            QueryResult.AddRange(temResult);

            int totalProductsCount = jobj.response.paginator.total_count;
            while (totalProductsCount > PageSise * currentPageIndex)
            {
                currentPageIndex++;
                result = GetProdustsInfoString(PageSise, currentPageIndex);
                jobj = (ProductsSearchResultJson.Rootobject)Newtonsoft.Json.JsonConvert.DeserializeObject(result, typeof(ProductsSearchResultJson.Rootobject));
                temResult = jobj.Translate();
                sb.Clear();
                temResult.Aggregate(sb, (x, y) => x.Append(y.id + ","));
                discountInfoList = QueryDiscountInfo(sb.ToString());
                foreach (var r in temResult)
                {
                var inv = inventories.SingleOrDefault(p => p.id == r.id);
                if (inv != null)
                {
                    r.成本价 = inv.成本价;
                }
                r.会员价 = r.零售价 * 0.88f;
                    r.会员卡名称 = "默认";
                    var discountInfo = discountInfoList.FirstOrDefault(p => p.id == r.id);
                    if (discountInfo != null && discountInfo.会员价.Count > 0)
                    {
                        r.会员价 = discountInfo.会员价[0].discount_value / 100f;
                        r.会员卡名称 = discountInfo.会员价[0].card_id.ToString();
                        if (discountInfo.会员价.Count > 1)
                        {
                            r.会员价2 = discountInfo.会员价[1].discount_value / 100f;
                            r.会员卡名称2 = discountInfo.会员价[1].card_id.ToString();
                            if (discountInfo.会员价.Count > 2)
                            {
                                r.会员价3 = discountInfo.会员价[2].discount_value / 100f;
                                r.会员卡名称3 = discountInfo.会员价[2].card_id.ToString();
                                if (discountInfo.会员价.Count > 3)
                                {
                                    r.会员价4 = discountInfo.会员价[3].discount_value / 100f;
                                    r.会员卡名称4 = discountInfo.会员价[3].card_id.ToString();
                                }
                            }
                        }
                    }
                }

                QueryResult.AddRange(temResult);
            }


            
        
        return QueryResult;
    }


    public static string GetProdustsInventoryInfoString(int PageSise, int currentPageIndex)
    {
        Auth auth = new Token(GlobalInfo.Token);
        YZClient yzClient = new DefaultYZClient(auth);
        Dictionary<string, object> dict = new System.Collections.Generic.Dictionary<string, object>();
        //dict.Add("is_displays", "[1]");
        //dict.Add("measurement", 10);
        dict.Add("page_no", currentPageIndex);
        dict.Add("page_size", PageSise);
        //dict.Add("show_sold_out", 0);
        dict.Add("source", "test");

        var result = yzClient.Invoke("youzan.items.inventory.get", "3.0.0", "POST", dict, null);

        return result;
    }
    public static List<ProductsInventoryResultJson.ProductsInventoryResult> GetProdustsInventoryInfo()
    {
        int PageSise = 40;
        int currentPageIndex = 1;
        var QueryResult = new List<ProductsInventoryResultJson.ProductsInventoryResult>();

        string result = GlobalInfo.GetProdustsInventoryInfoString(PageSise, currentPageIndex);
        var jobj = (ProductsInventoryResultJson.Rootobject)Newtonsoft.Json.JsonConvert.DeserializeObject(result, typeof(ProductsInventoryResultJson.Rootobject));
     

        QueryResult.AddRange(jobj.Translate());

        int totalProductsCount = jobj.response.count;
        while (totalProductsCount > PageSise * currentPageIndex)
        {
            currentPageIndex++;
            result = GetProdustsInventoryInfoString(PageSise, currentPageIndex);
            jobj = (ProductsInventoryResultJson.Rootobject)Newtonsoft.Json.JsonConvert.DeserializeObject(result, typeof(ProductsInventoryResultJson.Rootobject));
          
            QueryResult.AddRange(jobj.Translate());
        }




        return QueryResult;
    }

    public  static  List<ProductsDiscountResultJson.ProductsDiscountResult> QueryDiscountInfo(string temId)
    {
        var QueryResult = new List<ProductsDiscountResultJson.ProductsDiscountResult>();
        
        if (GlobalInfo.IsTokenValidate(AppID))
        {

            Auth auth = new Token(GlobalInfo.Token); // Auth auth = new Sign("app_id", "app_secret");

            YZClient yzClient = new DefaultYZClient(auth);
            Dictionary<string, object> dict = new System.Collections.Generic.Dictionary<string, object>();
            dict.Add("item_ids", "[" + temId + "]");

            var result = yzClient.Invoke("youzan.ump.memberprice.query", "3.0.0", "POST", dict, null);
            var jobj = (ProductsDiscountResultJson.Rootobject)Newtonsoft.Json.JsonConvert.DeserializeObject(result, typeof(ProductsDiscountResultJson.Rootobject));
            QueryResult.AddRange(jobj.Translate());
        }
        return QueryResult;
    }
}