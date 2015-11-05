
namespace shipstation_erp.Models
{
  //there are more fields here but this was all i cared about
    public class oShippmentCreateOrderResponse
    {
        private long _orderId;
        private string _orderNumber;

        public long orderId
        {
            get { return _orderId; }
            set { _orderId = value; }
        }

        public string orderNumber
        {
            get { return _orderNumber; }
            set { _orderNumber = value; }
        }
    }

}
