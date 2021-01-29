namespace Microsoft.BotBuilderSamples
{

    class SellItem
    {
        public string imageSrc { get; set; }
        public string type { get; set; }//物品種類
        public string time { get; set; }
        public string description { get; set; }//name         
        public string status { get; set; }//selling / sold out
        public string location { get; set; }//JSON Style
        public string ownerUserId { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
    }
}
