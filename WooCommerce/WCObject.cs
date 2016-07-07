using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace WooCommerceNET.WooCommerce
{
    public class WCObject
    {
        private RestAPI API;
        public WCObject(RestAPI api)
        {
            API = api;
        }

        public async Task<Store> GetStoreInfo()
        {
            string json = await API.SendHttpClientRequest(string.Empty, HttpMethod.Get, string.Empty);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            Store store = RestAPI.DeserializeJSon<Store>(json);
            store.WCRoutes = store.GetRoutes(json);
            return store;
        }

        #region "customers..."

        public async Task<CustomerList> GetCustomers(Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("customers", HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<CustomerList>(json);
        }

        public async Task<int> GetCustomerCount(Dictionary<string, string> parms = null)
        {
            string json = await API.SendHttpClientRequest("customers/count", HttpMethod.Get, string.Empty, parms);
            return int.Parse(json.Substring(json.IndexOf(':') + 1, json.IndexOf('}') - json.IndexOf(':') - 1).Trim('"'));
        }

        public async Task<Customer> GetCustomer(int id, Dictionary<string, string> parms = null)
        {
            string json = await API.SendHttpClientRequest("customers/" + id.ToString(), HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<Customer>(json);
        }

        public async Task<Customer> GetCustomerByEmail(string email, Dictionary<string, string> parms = null)
        {
            string json = await API.SendHttpClientRequest("customers/email/" + email, HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<Customer>(json);
        }

        public async Task<OrderList> GetCustomerOrders(int id, Dictionary<string, string> parms = null)
        {
            string json = await API.GetRestful("customers/" + id.ToString() + "/orders", parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<OrderList>(json);
        }

        public async Task<DownloadList> GetCustomerDownloads(int id, Dictionary<string, string> parms = null)
        {
            string json = await API.GetRestful("customers/" + id.ToString() + "/downloads", parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<DownloadList>(json);
        }

        //Don't forget to include a password when creating a customer, the example in REST API DOCS will not work!!!
        public async Task<string> PostCustomer(Customer c, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("customers", HttpMethod.Post, c, parms);
        }

        public async Task<string> UpdateCustomer(int id, Customer c, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("customers/" + id.ToString(), HttpMethod.Post, c, parms);
        }

        public async Task<string> UpdateCustomers(Customer[] cs, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("customers/bulk", HttpMethod.Post, cs, parms);
        }

        public async Task<string> DeleteCustomer(int id, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("customers/" + id.ToString(), HttpMethod.Delete, string.Empty, parms);
        }

        #endregion

        #region "orders..."

        public async Task<OrderList> GetOrders(Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("orders", HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<OrderList>(json);
        }

        public async Task<int> GetOrderCount(Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("orders/count", HttpMethod.Get, string.Empty, parms);
            return int.Parse(json.Substring(json.IndexOf(':') + 1, json.IndexOf('}') - json.IndexOf(':') - 1).Trim('"'));
        }

        public async Task<Order> GetOrder(int orderid, Dictionary<string, string> parms = null)
        {
            string json = await API.SendHttpClientRequest("orders/" + orderid.ToString(), HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<Order>(json);
        }

        public async Task<List<KeyValuePair<string, string>>> GetOrderStatuses(Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("orders/statuses", HttpMethod.Get, string.Empty, parms);
            json = json.Substring(20, json.Length - 22).Replace("\"", string.Empty);

            List<KeyValuePair<string, string>> statuses = new List<KeyValuePair<string, string>>();
            foreach(string status in json.Split(','))
            {
                KeyValuePair<string, string> value = new KeyValuePair<string, string>(status.Split(':')[0], status.Split(':')[1]);
                statuses.Add(value);
            }

            return statuses;
        }

        public async Task<string> PostOrder(Order c, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("orders", HttpMethod.Post, c, parms);
        }

        public async Task<string> UpdateOrder(int id, Order c, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("orders/" + id.ToString(), HttpMethod.Put, c, parms);
        }

        public async Task<string> UpdateOrders(Order[] cs, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("orders/bulk", HttpMethod.Put, cs, parms);
        }

        public async Task<string> DeleteOrder(int orderid, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("orders/" + orderid.ToString(), HttpMethod.Delete, string.Empty, parms);
        }

        public async Task<OrderNoteList> GetOrderNotes(int id, Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("orders/" + id.ToString() + "/notes", HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<OrderNoteList>(json);
        }

        public async Task<Order_Note> GetOrderNote(int orderid, int noteid, Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("orders/" + orderid.ToString() + "/notes/" + noteid.ToString(), HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<Order_Note>(json);
        }

        public async Task<string> PostOrderNote(int orderid, Order_Note n, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("orders/" + orderid.ToString() + "/notes", HttpMethod.Post, n, parms);
        }

        public async Task<string> UpdateOrderNote(int orderid, int noteid, Order_Note n, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("orders/" + orderid.ToString() + "/notes/" + noteid.ToString(), HttpMethod.Put, n, parms);
        }

        public async Task<string> DeleteOrderNote(int orderid, int noteid, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("orders/" + orderid.ToString() + "/notes/" + noteid.ToString(), HttpMethod.Delete, parms);
        }

        public async Task<OrderRefundList> GetOrderRefunds(int orderid, Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("orders/" + orderid.ToString() + "/refunds", HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<OrderRefundList>(json);
        }

        public async Task<Order_Refund> GetOrderRefund(int orderid, int refundid, Dictionary<string, string> parms = null)
        {
            string json = await API.SendHttpClientRequest("orders/" + orderid.ToString() + "/refunds/" + refundid.ToString(), HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<Order_Refund>(json);
        }

        public async Task<string> PostOrderRefund(int orderid, Order_Refund r, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("orders/" + orderid.ToString() + "/refunds", HttpMethod.Post, r, parms);
        }

        public async Task<string> UpdateOrderRefund(int orderid, int refundid, Order_Refund r, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("orders/" + orderid.ToString() + "/refunds/" + refundid.ToString(), HttpMethod.Put, r, parms);
        }

        public async Task<string> DeleteOrderRefund(int orderid, int refundid, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("orders/" + orderid.ToString() + "/refunds/" + refundid.ToString(), HttpMethod.Delete, string.Empty, parms);
        }


        #endregion

        #region "products..."

        public async Task<ProductList> GetProducts(Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("products", HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<ProductList>(json);
        }

        public async Task<int> GetProductCount(Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("products/count", HttpMethod.Get, string.Empty, parms);
            return int.Parse(json.Substring(json.IndexOf(':') + 1, json.IndexOf('}') - json.IndexOf(':') - 1).Trim('"'));
        }

        public async Task<Product> GetProduct(int productid, Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("products/" + productid.ToString(), HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<Product>(json);
        }

        public async Task<string> PostProduct(Product p, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("products", HttpMethod.Post, p, parms);
        }

        public async Task<string> UpdateProduct(int productid, Product p, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("products/" + productid.ToString(), HttpMethod.Put, p, parms);
        }

        public async Task<string> UpdateProducts(Product[] ps, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("products/bulk", HttpMethod.Put, ps, parms);
        }

        public async Task<string> DeleteProduct(int productid, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("products/" + productid.ToString(), HttpMethod.Delete, string.Empty, parms);
        }

        public async Task<ProductReviewList> GetProductReviews(int productid, Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("products/" + productid.ToString() + "/reviews", HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<ProductReviewList>(json);
        }

        public async Task<ProductCategoryList> GetProductCategories(Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("products/categories", HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<ProductCategoryList>(json);
        }

        public async Task<Product_Category> GetProductCategory(int categoryid, Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("products/categories/" + categoryid.ToString(), HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<Product_Category>(json);
        }

        public async Task<string> PostProductCategory(Product_Category pc, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("products/categories", HttpMethod.Post, pc, parms);
        }

        public async Task<string> UpdateProductCategory(int categoryid, Product_Category pc, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("products/categories/" + categoryid.ToString(), HttpMethod.Put, pc, parms);
        }

        public async Task<string> DeleteProductCategory(int categoryid, Dictionary<string, string> parms = null)
        {
            return await API.SendHttpClientRequest("products/categories/" + categoryid.ToString(), HttpMethod.Delete, string.Empty, parms);
        }

        #endregion

        #region "coupons..."

        public async Task<CouponList> GetCoupons(Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("coupons", HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<CouponList>(json);
        }

        public async Task<int> GetCouponCount(Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("coupons/count", HttpMethod.Get, string.Empty, parms);
            return int.Parse(json.Substring(json.IndexOf(':') + 1, json.IndexOf('}') - json.IndexOf(':') - 1).Trim('"'));
        }

        public async Task<Coupon> GetCoupon(int couponid, Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("coupons/" + couponid.ToString(), HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<Coupon>(json);
        }

        public async Task<Coupon> GetCoupon(string code, Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("coupons/code/" + code, HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<Coupon>(json);
        }

        public async Task<string> PostCoupon(Coupon c, Dictionary<string,string> parms = null)
        {
            return await API.SendHttpClientRequest("coupons", HttpMethod.Post, c, parms);
        }

        public async Task<string> UpdateCoupon(int couponid, Coupon c, Dictionary<string,string> parms = null)
        {
            return await API.SendHttpClientRequest("coupons/" + couponid.ToString(), HttpMethod.Put, c, parms);
        }

        public async Task<string> UpdateCoupons(Coupon[] cs, Dictionary<string,string> parms = null)
        {
            return await API.SendHttpClientRequest("coupons/bulk", HttpMethod.Put, cs, parms);
        }

        public async Task<string> DeleteCoupon(int couponid, Dictionary<string,string> parms = null)
        {
            return await API.SendHttpClientRequest("coupons/" + couponid.ToString(), HttpMethod.Delete, string.Empty, parms);
        }

        #endregion

        #region "reports..."

        public async Task<List<string>> GetReports(Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("reports", HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<List<string>>(json);
        }

        public async Task<string> GetSalesReport(Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("reports/sales", HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<string>(json);
        }

        public async Task<List<string>> GetTopSellerReport(Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("reports/sales/top_sellers", HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<List<string>>(json);
        }

        #endregion

        #region "webhooks..."

        public async Task<WebhookList> GetWebhooks(Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("webhooks", HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<WebhookList>(json);
        }

        public async Task<int> GetWebhookCount(Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("webhooks/count", HttpMethod.Get, string.Empty, parms);
            return int.Parse(json.Substring(json.IndexOf(':') + 1, json.IndexOf('}') - json.IndexOf(':') - 1).Trim('"'));
        }

        public async Task<Webhook> GetWebhook(int id, Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("webhooks/" + id.ToString(), HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<Webhook>(json);
        }

        public async Task<WebhookDeliveryList> GetWebhookDeliveries(int webhookid, Dictionary<string,string> parms = null)
        {
            string json = await API.SendHttpClientRequest("webhooks/" + webhookid.ToString() + "/deliveries", HttpMethod.Get, string.Empty, parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<WebhookDeliveryList>(json);
        }

        public async Task<WebhookDelivery> GetWebhookDelivery(int webhookid, int deliveryid, Dictionary<string,string> parms = null)
        {
            string json = await API.GetRestful("webhooks/" + webhookid.ToString() + "/deliveries/" + deliveryid.ToString(), parms);
            json = json.Substring(json.IndexOf(':') + 1, json.Length - json.IndexOf(':') - 2);
            return RestAPI.DeserializeJSon<WebhookDelivery>(json);
        }

        #endregion
    }
}
