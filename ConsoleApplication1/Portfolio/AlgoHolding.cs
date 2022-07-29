namespace DataAnalyst.Portfolio
{
    public class AlgoHolding
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Exchange { get; set; }
        public int Balance { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal LatestPrice { get; set; }
        public decimal HighestPriceAfterPurchase { get; set; }
        public bool StopLossAlerted { get; set; }
        public bool HighestPriceFallAlerted { get; set; }
        public bool ProfitDownToLossAlerted { get; set; }
    }
}
