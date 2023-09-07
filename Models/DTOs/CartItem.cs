namespace API_Test1.Models.DTOs
{
    public class CartItem
    {
        public string CartId { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public double Price { get; set; }
        public int DiscountPercentage { get; set; }
        public int Quantity { get; set; }
        public double OriginalPrice { get; set; }
        public string CartSignature { get; set; }

        public CartItem()
        {
            GenerateCartId();
            GenerateCartSignature();
        }

        private void GenerateCartId()
        {
            CartId = Guid.NewGuid().ToString();
        }

        private void GenerateCartSignature()
        {
            var dataToSign = $"{CartId}{ProductId}{ProductName}{Price}{DiscountPercentage}{Quantity}{OriginalPrice}{DateTime.Now.Ticks}";
            CartSignature = ComputeSignature(dataToSign);
        }

        public bool IsCartValid()
        {
            var dataToSign = $"{CartId}{ProductId}{ProductName}{Price}{DiscountPercentage}{Quantity}{OriginalPrice}{DateTime.Now.Ticks}";
            var calculatedSignature = ComputeSignature(dataToSign);
            return CartSignature == calculatedSignature;
        }

        private string ComputeSignature(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
