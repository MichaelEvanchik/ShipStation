using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using System.Configuration;
using shipstation_erp.Models;

namespace shipstation_erp
{

protected void SomeEventYourIn()
{
                                        oShipStationCreateOrderRequest c = new oShipStationCreateOrderRequest();
                                        c = GetShipStationCreateOrderRequestByExternalRef(external_ref);

                                        HttpWebResponse wresp = null;
                                        wresp = PostJsonCreateOrderShipStation(c);
                                        var encoding = ASCIIEncoding.ASCII;
                                        responseText = "";

                                        using (var reader = new System.IO.StreamReader(wresp.GetResponseStream(), encoding))
                                        {
                                                responseText = reader.ReadToEnd();
                                        }

                                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                                        var oRes = new oShippmentCreateOrderResponse();

                                        oRes = serializer.Deserialize<oShippmentCreateOrderResponse>(responseText);

                                        long? orderId;
                                        orderId = oRes.orderId;

                                        if (orderId > 0)
                                        {
                                            var encoding2 = ASCIIEncoding.ASCII;
                                            oShipStationCreateLabelRequest ol = new oShipStationCreateLabelRequest();
                                            ol = GetShipStationCreateLabelRequestByExternalRef(external_ref, orderId);
                                            responseText = "";
                                            HttpWebResponse wresp2 = null;
                                            wresp2 = PostJsonPrintLabelShipStation(ol);

                                            using (var reader = new System.IO.StreamReader(wresp2.GetResponseStream(), encoding2))
                                            {
                                                responseText = reader.ReadToEnd();
                                            }
                                            JavaScriptSerializer serializer2 = new JavaScriptSerializer();
                                            var oRes2 = new oShippmentCreateLabelResponse();

                                            oRes2 = serializer2.Deserialize<oShippmentCreateLabelResponse>(responseText);
                                            var binary = System.Convert.FromBase64String(oRes2.labelData);
                                            string spath = "c:\\inetpub\\wwwroot\\ftproot\\redbullbe\\work_orders\\" + lsale_date.Year.ToString() + "\\" + lsale_date.Month.ToString() + "\\" + lsale_date.Day.ToString() + "\\";
                                            System.IO.Directory.CreateDirectory(spath + external_ref.ToString());

                                            File.WriteAllBytes(spath + external_ref.ToString() + "\\Label" + external_ref.ToString() + ".pdf", binary);
                                            using (var dc = new stylusDataContext())
                                            {
                                                dc.sp_update_order_and_items_tracking(external_ref, oRes2.trackingNumber, null, oRes2.insuranceCost, orderId.ToString(), null, DateTime.Now, oRes2.shipmentCost, oRes2.shipmentId);
                                            }
                                        }
}                                        

 order GetOrderObjectRequestByExternalRef(long external_ref, string status_description)
        {
            order o = new order();
            using (var dc = new shipstationDataContext())
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

        oShipStationCreateOrderRequest GetShipStationCreateOrderRequestByExternalRef(long external_ref)
        {
            oShipStationCreateOrderRequest o = new oShipStationCreateOrderRequest();
            billTo b = new billTo();
            shipTo s = new shipTo();
            List<customsItems> Custom = new List<customsItems>();
            oShipInternational intl = new oShipInternational();
            using (var dc = new shipstationDataContext())
            {
                ISingleResult<sp_get_order_by_external_refResult> res = dc.sp_get_order_by_external_ref(external_ref);
                foreach (sp_get_order_by_external_refResult ret in res)
                {
                    o.orderNumber = external_ref.ToString();
                    o.orderDate = ret.sale_datetime.ToString();

                    o.orderStatus = "awaiting_shipment";

                    b.name = "YourDepartment";
                    b.company = "YourCompany";
                    b.street1 = "YourAddress";
                    b.city = "YourState";
                    b.state = "STATE";
                    b.postalCode = "07000";
                    b.country = "US";
                    b.phone = "888-888-8888";
                    b.residential = false;

                    o.billTo = b;

                    shipTo st = new shipTo();
                    st.name = ret.customer_name;
                    st.company = ret.company; 
                    st.street1 = ret.shipping_address_1;
                    st.street2 = ret.shipping_address_2;

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

        oShipStationCreateLabelRequest GetShipStationCreateLabelRequestByExternalRef(long external_ref, long? orderId)
        {
            oShipStationCreateLabelRequest o = new oShipStationCreateLabelRequest();
            List<customsItems> Custom = new List<customsItems>();
            oShipInternational intl = new oShipInternational();

            using (var dc = new shipstationDataContext())
            {
                ISingleResult<sp_get_order_by_external_refResult> res = dc.sp_get_order_by_external_ref(external_ref);
                foreach (sp_get_order_by_external_refResult ret in res)
                {
                    o.orderId = orderId;
                    
                    //this is the translation which you might have to do with your data mapping to the shipstation.com setup
                    
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
                    
                    //this would be your calculation here
                    if(ret.size == "XS")
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

                    if(ret.shipping_country_code != "US")
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

        public HttpWebResponse PostJsonPrintLabelShipStation(oShipStationCreateLabelRequest s)
        {
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string sJSON = oSerializer.Serialize(s);
            string wRespStatusCode = string.Empty;
            var baseAddress = "https://ssapi.shipstation.com/orders/createlabelfororder";
                                                         
            var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";
            http.Headers.Add("Authorization", "Basic YourSecretKeyHere");

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

            if (wRespStatusCode == "")
            {
                wRespStatusCode = Response.StatusCode.ToString();
            }
            return response;
        }

        public HttpWebResponse PostJsonCreateOrderShipStation(oShipStationCreateOrderRequest s)
        {
            var oSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            string sJSON = oSerializer.Serialize(s);
            string wRespStatusCode = string.Empty;
            var baseAddress = "https://ssapi.shipstation.com/orders/createorder";
          

            var http = (HttpWebRequest)WebRequest.Create(new Uri(baseAddress));
            http.Accept = "application/json";
            http.ContentType = "application/json";
            http.Method = "POST";
            http.Headers.Add("Authorization", "Basic YourSecretKeyHere");

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

            if (wRespStatusCode == "")
            {
                wRespStatusCode = Response.StatusCode.ToString();
            }
            return response;
        }

        private dynamic AddClassName(order item)
        {
            return new { order = item };
        }
}
