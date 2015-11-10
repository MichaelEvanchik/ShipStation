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
                                        util u = new util();
                                        oShipStationCreateOrderRequest c = new oShipStationCreateOrderRequest();
                                        c = u.GetShipStationCreateOrderRequestByExternalRef(external_ref);

                                        HttpWebResponse wresp = null;
                                        wresp = u.PostJsonCreateOrderShipStation(c);
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
                                            ol = u.GetShipStationCreateLabelRequestByExternalRef(external_ref, orderId);
                                            responseText = "";
                                            HttpWebResponse wresp2 = null;
                                            wresp2 = u.PostJsonPrintLabelShipStation(ol);

                                            using (var reader = new System.IO.StreamReader(wresp2.GetResponseStream(), encoding2))
                                            {
                                                responseText = reader.ReadToEnd();
                                            }
                                            JavaScriptSerializer serializer2 = new JavaScriptSerializer();
                                            var oRes2 = new oShippmentCreateLabelResponse();

                                            oRes2 = serializer2.Deserialize<oShippmentCreateLabelResponse>(responseText);
                                            var binary = System.Convert.FromBase64String(oRes2.labelData);
                                            string spath = "c:\\inetpub\\wwwroot\\ftproot\\redbubble\\work_orders\\" + lsale_date.Year.ToString() + "\\" + lsale_date.Month.ToString() + "\\" + lsale_date.Day.ToString() + "\\";
                                            System.IO.Directory.CreateDirectory(spath + external_ref.ToString());

                                            File.WriteAllBytes(spath + external_ref.ToString() + "\\Label" + external_ref.ToString() + ".pdf", binary);
                                            using (var dc = new stylusDataContext())
                                            {
                                                dc.sp_update_order_and_items_tracking(external_ref, oRes2.trackingNumber, null, oRes2.insuranceCost, orderId.ToString(), null, DateTime.Now, oRes2.shipmentCost, oRes2.shipmentId);
                                            }
                                        }
}                                        
}
