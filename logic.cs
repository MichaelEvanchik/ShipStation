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
                            var resp = new MyWebResponse();
                            resp = u.PostJsonShipStation((oShipStationCreateLabelRequest)ol, "https://ssapi.shipstation.com/orders/createlabelfororder");
                            

                            if (resp.bexception == false)
                            {
                                JavaScriptSerializer serializer = new JavaScriptSerializer();
                                var oRes = new oShippmentCreateLabelResponse();

                                oRes = serializer.Deserialize<oShippmentCreateLabelResponse>(r.smessage);
                                var binary = System.Convert.FromBase64String(oRes.labelData);
                                spath += lsale_date.Year.ToString() + "\\" + lsale_date.Month.ToString() + "\\" + lsale_date.Day.ToString() + "\\";
                                System.IO.Directory.CreateDirectory(spath + external_ref.ToString());
                                File.WriteAllBytes(spath + external_ref.ToString() + "\\Label" + external_ref.ToString() + ".pdf", binary);
                            }
}
                        
