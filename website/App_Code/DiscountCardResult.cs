using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YZOpenSDK;

namespace DiscountCardResultJson
{
    public class DiscountCardResult
    {
        public string id { get; set; }
        public string 名称 { get; set; }
    }


    public class Rootobject
    {
        public Response response { get; set; }
        public List<DiscountCardResult> Translate()
        {
            var result = new List<DiscountCardResult>();
            if (response != null)
            {
                foreach (var i in response.items)
                {
                    var newProductsDiscountResult = new DiscountCardResult();
                    newProductsDiscountResult.id = i.card_alias;
                    newProductsDiscountResult.名称 = i.name;
                    
                    result.Add(newProductsDiscountResult);

                }
            }
            return result;
        }
        public static List<DiscountCardResult> GetProdustsInfo(string token)
        {
            var QueryResult = new List<DiscountCardResult>();

            Auth auth = new Token(token);
            YZClient yzClient = new DefaultYZClient(auth);
            Dictionary<string, object> dict = new System.Collections.Generic.Dictionary<string, object>();
            //dict.Add("is_displays", "[1]");
            //dict.Add("measurement", 10);
            dict.Add("page", 1);

            var result = yzClient.Invoke("youzan.scrm.card.list", "3.0.0", "POST", dict, null);
            var jobj = (Rootobject)Newtonsoft.Json.JsonConvert.DeserializeObject(result, typeof(Rootobject));

            return jobj.Translate();
        }
    }

    public class Response
    {
        public int total { get; set; }
        public int page { get; set; }
        public Item[] items { get; set; }
        public int page_size { get; set; }
    }

    public class Item
    {
        public string create_time { get; set; }
        public string card_url { get; set; }
        public string name { get; set; }
        public string card_alias { get; set; }
        public bool is_available { get; set; }
    }



}
