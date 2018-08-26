using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace ProductsDiscountResultJson
{
    public class ProductsDiscountResult
    {
        public string id { get; set; }
        List<Card> _会员价 = new  List<Card>();
        public List<Card> 会员价
        {
            get { return _会员价; }
        }
    }

    public class Rootobject
    {
        public Response[] response { get; set; }
        public List<ProductsDiscountResult> Translate()
        {
            var result = new List<ProductsDiscountResult>();
            if (response != null)
            {
                foreach (var i in response)
                {
                    var newProductsDiscountResult = new ProductsDiscountResult();
                    newProductsDiscountResult.id = i.item_id.ToString();
                    foreach (var sk in i.skus)
                    {
                        foreach (var c in sk.cards)
                        {
                            newProductsDiscountResult.会员价.Add(c);
                        }
                    }
                    result.Add(newProductsDiscountResult);

                }
            }
            return result;
        }
    }

    public class Response
    {
        public Sku[] skus { get; set; }
        public int item_id { get; set; }
    }

    public class Sku
    {
        public Card[] cards { get; set; }
        public int price { get; set; }
    }

    public class Card
    {
        public int discount_value { get; set; }
        public int discount_type { get; set; }
        public int card_id { get; set; }
    }



}
