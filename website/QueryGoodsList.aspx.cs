using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YZOpenSDK;

public partial class QueryGoodsList : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
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
    protected void ButtonQuery_Click(object sender, EventArgs e)
    {
        int PageSise = 10;
        int currentPageIndex = 1;
        if (!GlobalInfo.IsTokenValidate(TextBoxAppID.Text))
        {
            GetToken();
        }
        if (GlobalInfo.IsTokenValidate(TextBoxAppID.Text))
        {
            var QueryResult = new List<ProductsSearchResultJson.ProductsSearchResult>();

            string result = GetProdustsInfo(PageSise, currentPageIndex);
            TextBox1.Text += "\n\n" + result;
            var jobj = (ProductsSearchResultJson.Rootobject)Newtonsoft.Json.JsonConvert.DeserializeObject(result, typeof(ProductsSearchResultJson.Rootobject));

            var temResult = jobj.Translate();
            StringBuilder sb = new StringBuilder();
            temResult.Aggregate(sb, (x, y) => x.Append(y.id + ","));
            var discountInfoList = QueryDiscountInfo(sb.ToString());
            foreach (var r in temResult)
            {
                var discountInfo = discountInfoList.FirstOrDefault(p => p.id == r.id);
                if (discountInfo!=null && discountInfo.会员价.Count > 0)
                {
                    r.会员价 = discountInfo.会员价[0].discount_value;
                    r.会员卡名称 = discountInfo.会员价[0].card_id.ToString();
                }
                else
                {
                    r.会员价 = r.零售价 * 0.88f;
                    r.会员卡名称 = "默认";
                }
            }

            QueryResult.AddRange(temResult);

            int totalProductsCount = jobj.response.paginator.total_count;
            while(totalProductsCount> PageSise* currentPageIndex)
            {
                currentPageIndex++;
                result = GetProdustsInfo(PageSise, currentPageIndex);
                TextBox1.Text += "\n\n" + result;
                jobj = (ProductsSearchResultJson.Rootobject)Newtonsoft.Json.JsonConvert.DeserializeObject(result, typeof(ProductsSearchResultJson.Rootobject));
                temResult = jobj.Translate();
                sb.Clear();
                temResult.Aggregate(sb, (x, y) => x.Append(y.id + ","));
                discountInfoList = QueryDiscountInfo(sb.ToString());
                foreach (var r in temResult)
                {
                    r.会员价 = r.零售价 * 0.88f;
                    r.会员卡名称 = "默认";
                    var discountInfo = discountInfoList.FirstOrDefault(p => p.id == r.id);
                    if (discountInfo != null && discountInfo.会员价.Count > 0)
                    {
                        r.会员价 = discountInfo.会员价[0].discount_value;
                        r.会员卡名称 = discountInfo.会员价[0].card_id.ToString();
                        if (discountInfo.会员价.Count > 1)
                        {
                            r.会员价2 = discountInfo.会员价[1].discount_value;
                            r.会员卡名称2 = discountInfo.会员价[1].card_id.ToString();
                            if (discountInfo.会员价.Count > 2)
                            {
                                r.会员价3 = discountInfo.会员价[2].discount_value;
                                r.会员卡名称3 = discountInfo.会员价[2].card_id.ToString();
                                if (discountInfo.会员价.Count > 3)
                                {
                                    r.会员价4 = discountInfo.会员价[3].discount_value;
                                    r.会员卡名称4 = discountInfo.会员价[3].card_id.ToString();
                                }
                            }
                        }                        
                    }
                }

                QueryResult.AddRange(temResult);
            }

            //foreach (var q in QueryResult)
            //{
            //    var discountInfo = QueryDiscountInfo(q.id);
            //    if (discountInfo.Count > 0)
            //    {
            //        q.会员价 = discountInfo[0].会员价[1].discount_value;
            //    }
            //    else
            //    {
            //        q.会员价 = q.零售价 * 0.88f;
            //    }
            //}

            GridView1.DataSource = QueryResult;
            GridView1.DataBind();
        }
    }

    private void GetToken()
    {
        string url = "http://open.youzan.com/oauth/token";
        Encoding encoding = Encoding.GetEncoding("utf-8");
        IDictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("client_id", TextBoxAppID.Text);
        parameters.Add("client_secret", TextBoxSecret.Text);
        parameters.Add("grant_type", "silent");
        parameters.Add("kdt_id", TextBoxMaketID.Text);
        HttpWebResponse response = CreatePostHttpResponse(url, parameters, encoding);
        //打印返回值  
        Stream stream = response.GetResponseStream();   //获取响应的字符串流  
        StreamReader sr = new StreamReader(stream); //创建一个stream读取流  
        string html = sr.ReadToEnd();   //从头读到尾，放到字符串html  
        var jobj = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(html);
        GlobalInfo.SetToken(TextBoxAppID.Text, jobj["access_token"].ToString());
    }

    private string GetProdustsInfo(int PageSise, int currentPageIndex)
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
    private string GetSingleProdustsInfo(string productId)
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
    protected void Button1_Click(object sender, EventArgs e)
    {
        if (!GlobalInfo.IsTokenValidate(TextBoxAppID.Text))
        {
            GetToken();
        }
        if (GlobalInfo.IsTokenValidate(TextBoxAppID.Text))
        {
            var resultList = DiscountCardResultJson.Rootobject.GetProdustsInfo(GlobalInfo.Token);
            GridViewMemberCard.DataSource = resultList;
            GridViewMemberCard.DataBind();
        }           
    }

    private List<ProductsDiscountResultJson.ProductsDiscountResult> QueryDiscountInfo(string temId)
    {
        var QueryResult = new List<ProductsDiscountResultJson.ProductsDiscountResult>();
        if (!GlobalInfo.IsTokenValidate(TextBoxAppID.Text))
        {
            GetToken();
        }
        if (GlobalInfo.IsTokenValidate(TextBoxAppID.Text))
        {
            
            Auth auth = new Token(GlobalInfo.Token); // Auth auth = new Sign("app_id", "app_secret");

            YZClient yzClient = new DefaultYZClient(auth);
            Dictionary<string, object> dict = new System.Collections.Generic.Dictionary<string, object>();
            dict.Add("item_ids", "[" + temId + "]");

            var result = yzClient.Invoke("youzan.ump.memberprice.query", "3.0.0", "POST", dict, null);
            TextBox1.Text += "\n\n" + result;
            var jobj = (ProductsDiscountResultJson.Rootobject)Newtonsoft.Json.JsonConvert.DeserializeObject(result, typeof(ProductsDiscountResultJson.Rootobject));
            QueryResult.AddRange(jobj.Translate());
        }
        return QueryResult;
    }

    protected void ButtonXBQ_Click(object sender, EventArgs e)
    {
        TextBoxAppID.Text = "a2b31c521fda70634a";
        TextBoxSecret.Text = "d2f2111b804f20f51b88baf942a4dbbb";
        TextBoxMaketID.Text = "40892323";
        TextBox会员卡.Text = "100501980";
    }
}