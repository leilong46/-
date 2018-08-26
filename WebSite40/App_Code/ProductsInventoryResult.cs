using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace ProductsInventoryResultJson
{
    public class ProductsInventoryResult
    {
        public string id { get; set; }
        public string 系统标识{ get; set; }
        public string 名称 { get; set; }
        public float 成本价 { get; set; }        
    }

    public class Rootobject
    {
        public Response response { get; set; }

        public List<ProductsInventoryResult> Translate()
        {
            var result = new List<ProductsInventoryResult>();
            foreach (var i in response.items)
            {
                result.Add(new ProductsInventoryResult()
                {
                    id = i.item_id.ToString(),
                    名称 = i.title,
                    成本价 = i.price / 100.0f,
                    系统标识 = i.alias,
                });
            }
            return result;
        }
    }

    public class Response
    {
        public int count { get; set; }
        public Item[] items { get; set; }
    }

    public class Item
    {
        public string created_time { get; set; }
        public string detail_url { get; set; }
        public int quantity { get; set; }
        public int post_fee { get; set; }
        public int item_id { get; set; }
        public int item_type { get; set; }
        public int num { get; set; }
        public Item_Imgs[] item_imgs { get; set; }
        public string title { get; set; }
        public string item_no { get; set; }
        public string update_time { get; set; }
        public int price { get; set; }
        public string alias { get; set; }
        public int post_type { get; set; }
        public Delivery_Template delivery_template { get; set; }
    }

    public class Delivery_Template
    {
        public string delivery_template_fee { get; set; }
        public int delivery_template_id { get; set; }
        public int delivery_template_valuation_type { get; set; }
        public string delivery_template_name { get; set; }
    }

    public class Item_Imgs
    {
        public string thumbnail { get; set; }
        public string created { get; set; }
        public string medium { get; set; }
        public int id { get; set; }
        public string url { get; set; }
        public string combine { get; set; }
    }

}
