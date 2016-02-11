using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Net;
using System.Text;

namespace shipstation_erp.Models
{
    public class util
    {
        public order GetOrderObjectRequestByExternalRef(long external_ref, string status_description)
        {
            order o = new order();
            using (var dc = new stylusDataContext())
            {
                List<item> itm = new List<item>();
                List<tracking> trk = new List<tracking>();
                ISingleResult<sp_get_order_by_external_refResult> res = dc.sp_get_order_by_external_ref(external_ref);
                foreach (sp_get_order_by_external_refResult ret in res)
                {

                    o.id = ret.order_external_ref;
                    o.status = status_description;

                    item i = new item();
                    i.id = ret.item_external_ref;
                    itm.Add(i);

                    o.item = itm;

                    tracking t = new tracking();
                    t.number = ret.tracking;
                    trk.Add(t);

                    o.tracking = trk;
                }
            }
            return o;
        }

        public oShipStationCreateOrderRequest GetShipStationCreateOrderRequestByExternalRef(long external_ref)
        {
            oShipStationCreateOrderRequest o = new oShipStationCreateOrderRequest();
            billTo b = new billTo();
            shipTo s = new shipTo();
            List<customsItems> Custom = new List<customsItems>();
            oShipInternational intl = new oShipInternational();
            using (var dc = new stylusDataContext())
            {
                ISingleResult<sp_get_order_by_external_refResult> res = dc.sp_get_order_by_external_ref(external_ref);
                foreach (sp_get_order_by_external_refResult ret in res)
                {
                    o.orderNumber = external_ref.ToString();
                    o.orderDate = ret.sale_datetime.ToString();

                    o.orderStatus = "awaiting_shipment";

                    b.name = "Your department.";
                    b.company = "Your company";
                    b.street1 = "Your address";
                    b.city = "city";
                    b.state = "ST";
                    b.postalCode = "06902";
                    b.country = "US";
                    b.phone = "888-888-5555";
                    b.residential = false;

                    o.billTo = b;

                    shipTo st = new shipTo();
                    st.name = ret.customer_name;
                    st.company = null;
                    st.street1 = ret.shipping_address_1;
                    st.street2 = ret.shipping_address_2;
                    //st.street3 = ret.shipping_address_3;
                    if (ret.shipping_country_code != "US")
                    {
                        intl.contents = "merchandise";
                        intl.nonDelivery = "return_to_sender";
                        customsItems c = new customsItems();
                        c.description = ret.description;
                        c.quantity = ret.quantity;
                        c.value = 14 * ret.quantity;
                        c.harmonizedTariffCode = "";
                        c.countryOfOrigin = "US";
                        Custom.Add(c);
                        intl.customsItems = Custom;
                        o.internationalOptions = intl;
                    }

                    st.city = ret.shipping_address_3;
                    st.state = ret.shipping_address_4;
                    st.postalCode = ret.shipping_postcode;
                    st.country = ret.shipping_country_code;
                    st.phone = ret.phone;
                    st.residential = true;

                    o.shipTo = st;
                }
                return o;
            }
            return o;
        }

        public oShipStationCreateLabelRequest GetShipStationCreateLabelRequestByExternalRef(long external_ref, long? orderId)
        {
            oShipStationCreateLabelRequest o = new oShipStationCreateLabelRequest();
            List<customsItems> Custom = new List<customsItems>();
            oShipInternational intl = new oShipInternational();

            using (var dc = new stylusDataContext())
            {
                ISingleResult<sp_get_order_by_external_refResult> res = dc.sp_get_order_by_external_ref(external_ref);
                foreach (sp_get_order_by_external_refResult ret in res)
                {
                    o.orderId = orderId;

                    if (ret.shipping_method.ToLower().Trim() == "standard" && ret.shipping_country_code == "US")
                    {
                        o.carrierCode = "ups";
                        o.serviceCode = "expedited_mail_innovations";
                    }

                    if (ret.shipping_method.ToLower().Trim() == "express" && ret.shipping_country_code == "US")
                    {
                        o.carrierCode = "ups";
                        o.serviceCode = "ups_2nd_day_air";
                    }

                    if (ret.shipping_method.ToLower().Trim() == "standard" && ret.shipping_country_code != "US")
                    {
                        o.carrierCode = "apc";
                        o.serviceCode = "apc_priority_ddu";
                    }

                    if (ret.shipping_method.ToLower().Trim() == "express" && ret.shipping_country_code != "US")
                    {
                        o.carrierCode = "ups";
                        o.serviceCode = "ups_worldwide_saver";
                    }


                    o.packageCode = "package";
                    if (o.serviceCode == "expedited_mail_innovations")
                    {
                        o.packageCode = "mi_irregulars";
                    }
                    else
                    {
                        o.packageCode = "package";
                    }

                    o.confirmation = "delivery";
                    o.shipDate = DateTime.Now.ToString();

                    weight w = new weight();
                    w.units = "ounces";
                    if (ret.size == "XS")
                    {
                        w.value = 8 * ret.quantity;
                    }
                    if (ret.size == "SMALL")
                    {
                        w.value = 8 * ret.quantity;
                    }
                    if (ret.size == "MEDIUM")
                    {
                        w.value = 9 * ret.quantity;
                    }
                    if (ret.size == "LARGE")
                    {
                        w.value = 8 * ret.quantity;
                    }
                    if (ret.size == "XL")
                    {
                        w.value = 10 * ret.quantity;
                    }
                    if (ret.size == "2XL")
                    {
                        w.value = 10 * ret.quantity;
                    }
                    if (ret.size == "3XL")
                    {
                        w.value = 11 * ret.quantity;
                    }

                    o.weight = w;

                    if (ret.shipping_country_code != "US")
                    {
                        intl.contents = "merchandise";
                        intl.nonDelivery = "return_to_sender";
                        customsItems c = new customsItems();
                        c.description = ret.description;
                        c.quantity = ret.quantity;
                        c.value = 14 * ret.quantity;
                        c.harmonizedTariffCode = "";
                        c.countryOfOrigin = "US";
                        Custom.Add(c);
                        intl.customsItems = Custom;
                        o.internationalOptions = intl;
                    }

                    o.testLabel = false;
                }
            }
            return o;
        }

 public MyWebResponse PostJsonShipStation(object s, string address)
        {
            ServicePointManager.DefaultConnectionLimit = 1;
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string sJSON = oSerializer.Serialize(s);
            string wRespStatusCode = string.Empty;
            string responseText = string.Empty;
            var r = new MyWebResponse();
            bool bwaserror = false;

            var http = (HttpWebRequest)WebRequest.Create(new Uri(address));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";
            http.Headers.Add("Authorization", "Basic Y2E2YjFlOTExM2ZlNGY3YTllMjFlYmEwNmVmZTE0N2M6NjM1MDkwNzQ2MzM5NDlkOGI4ZDQwNGQ3ZTc2YTYwMDE=");
            http.KeepAlive = false;
            var encoding = new ASCIIEncoding();
            Byte[] bytes = encoding.GetBytes(sJSON);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)http.GetResponse();
            }
            catch (WebException we)
            {
                wRespStatusCode = ((HttpWebResponse)we.Response).StatusCode.ToString();
                bwaserror = true;
                r.bexception = true;
                using (var reader = new System.IO.StreamReader(we.Response.GetResponseStream(), encoding))
                {
                    responseText = reader.ReadToEnd();
                    if (responseText.IndexOf("ExceptionMessage") > 0)
                    {
                        responseText = responseText.Substring(responseText.IndexOf("ExceptionMessage"));
                        responseText = responseText.Substring(0, responseText.IndexOf("ExceptionType") - 1);
                        responseText = responseText.Replace("\\", "");
                        responseText = responseText.Replace("\"", "");
                        r.smessage = responseText;
                    }
                    else
                    {
                        responseText = responseText.Replace("\\", "");
                        responseText = responseText.Replace("\"", "");
                        r.smessage = responseText;
                    }
                }
                r.smessage += " " + wRespStatusCode;
                // r.smessage = we.InnerException.ToString();
                r.statuscode = wRespStatusCode;
            }

            if (bwaserror == false)
            {
                if (wRespStatusCode == "")
                {
                    wRespStatusCode = response.StatusCode.ToString();
                    r.bexception = false;
                    r.statuscode = wRespStatusCode;
                }


                using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                {
                    responseText = reader.ReadToEnd();
                    r.smessage = responseText;
                }
            }
            try
            {
                response.Close();
            }
            catch { }
            try
            {
                newStream.Dispose();
            }
            catch { }
            try
            {
                response.Dispose();
            }
            catch { }
            try
            {
                bytes = null;
            }
            catch { }

            return r;
        }
        public string PostJsonOrderRedBubble(order o)
        {
            ServicePointManager.DefaultConnectionLimit = 6;
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            dynamic itemClassName = AddClassName(o);
            string sJSON = oSerializer.Serialize(itemClassName);
            string wRespStatusCode = string.Empty;

            var baseAddress = "https://www.redbubble.com/orders/status_update";

            var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));

            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";
            http.Headers.Add("Redbubble-Auth-Token", "YourSecretCode");
            http.Timeout = 30000;
            http.KeepAlive = false;
            ASCIIEncoding encoding = new ASCIIEncoding();
            Byte[] bytes = encoding.GetBytes(sJSON);

            Stream newStream = http.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)http.GetResponse();
            }
            catch (WebException we)
            {
                wRespStatusCode = ((HttpWebResponse)we.Response).StatusCode.ToString();
            }
            catch (Exception ex)
            {
                wRespStatusCode = ex.Message.ToString();
            }

            if (wRespStatusCode == "")
            {
                wRespStatusCode = response.StatusCode.ToString();
            }
            try
            {
                response.Close();
                newStream.Dispose();
                response.Dispose();
                bytes = null;
            }
            catch { }

            return wRespStatusCode;
        }

        private dynamic AddClassName(order item)
        {//used if you want the class name in your JSION serialized
            return new { order = item };
        }
    }
}
