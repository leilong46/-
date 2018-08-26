using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace ProductsSearchResultJson
{
    public class ProductsSearchResult
    {
        public string id { get; set; }
        public string 商品条码 { get; set; }
        public string 门店id { get; set; }
        public string 名称 { get; set; }
        public string 单位 { get; set; }
        public float 成本价 { get; set; }

        public float 零售价 { get; set; }
        public float 会员价 { get; set; }
        public string 会员卡名称  { get; set; }

        public float 会员价2 { get; set; }
        public string 会员卡名称2 { get; set; }


        public float 会员价3 { get; set; }
        public string 会员卡名称3 { get; set; }


        public float 会员价4 { get; set; }
        public string 会员卡名称4{ get; set; }
        public string 产地 { get; set; }
        public string 规格 { get; set; }
        public string 等级 { get; set; }
    }


    public class Rootobject
    {
        public Response response { get; set; }
        public List<ProductsSearchResult> Translate()
        {
            var result = new List<ProductsSearchResult>();
            foreach (var i in response.items)
            {
                result.Add(new ProductsSearchResult()
                {
                    id = i.item_id.ToString(),
                    名称 = i.title,
                    商品条码 = i.sku_no,
                    门店id = i.kdt_id.ToString(),
                    单位 = i.unit,
                    零售价 = i.price / 100.0f,
                    会员卡名称 = "默认",
                    会员价 = i.price / 100.0f * 0.88f,
                规格 = i.specifications,
                });
            }
            return result;
        }
    }

    public class Response
    {
        public Paginator paginator { get; set; }
        public Item[] items { get; set; }
    }

    public class Paginator
    {
        public int total_count { get; set; }
        public int page { get; set; }
        public int page_size { get; set; }
    }

    public class Item
    {
        public string category_name { get; set; }
        public int item_id { get; set; }
        public int sell_stock_count { get; set; }
        public Offline_Sell_On_Sub_Kdt_V_Os[] offline_sell_on_sub_kdt_v_os { get; set; }
        public long created_at { get; set; }
        public string biz_mark_code { get; set; }
        public string title { get; set; }
        public string specifications { get; set; }
        public int join_level_discount { get; set; }
        public int measurement { get; set; }
        public int sold_num { get; set; }
        public int kdt_id { get; set; }
        public string unit { get; set; }
        public string biz_mark_name { get; set; }
        public string sku_no { get; set; }
        public int category_id { get; set; }
        public long updated_at { get; set; }
        public int sell_type { get; set; }
        public int price { get; set; }
        public int goods_type { get; set; }
        public string photo_url { get; set; }
        public int is_display { get; set; }
    }

    public class Offline_Sell_On_Sub_Kdt_V_Os
    {
        public string kdt_name { get; set; }
        public int kdt_id { get; set; }
        public string address { get; set; }
        public int item_id { get; set; }
        public int total_stock { get; set; }
    }
}
