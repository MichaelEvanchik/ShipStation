
namespace shipstation_erp.Models
{
    public class oShippmentCreateLabelResponse
    {
        private long _shipmentId;
        private decimal _shipmentCost;
        private decimal _insuranceCost;
        private string _trackingNumber;
        private string _labelData;
        private string _formData;

        public long shipmentId
        {
            get { return _shipmentId; }
            set { _shipmentId = value; }
        }

        public decimal shipmentCost
        {
            get { return _shipmentCost; }
            set { _shipmentCost = value; }
        }

        public decimal insuranceCost
        {
            get { return _insuranceCost; }
            set { _insuranceCost = value; }
        }

        public string trackingNumber
        {
            get { return _trackingNumber; }
            set { _trackingNumber = value; }
        }

        public string labelData
        {
            get { return _labelData; }
            set { _labelData = value; }
        }

        public string formData
        {
            get { return _formData; }
            set { _formData = value; }
        }
    }
}
