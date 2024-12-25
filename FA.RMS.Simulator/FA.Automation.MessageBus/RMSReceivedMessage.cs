namespace FA.Automation.MessageBus
{
    public class RMSReceivedMessage
    {
        public string MessageName { get; set; }
        public string UserID { get; set; }
        public string TransctionID { get; set; }
        public string EQPID { get; set; }
        public string RecipeID { get; set; }
        public string ProductID { get; set; }
        public string LotType { get; set; }
        public string PortID { get; set; }
        public string RecipeFormat { get; set; }
    }
}
