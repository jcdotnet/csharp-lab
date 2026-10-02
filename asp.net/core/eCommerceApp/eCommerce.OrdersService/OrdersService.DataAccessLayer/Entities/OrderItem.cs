using MongoDB.Bson.Serialization.Attributes;

namespace OrdersService.DataAccessLayer.Entities
{
    public class OrderItem
    {
        private int v1;
        private int v2;

        public OrderItem(Guid productId, int v1, int v2)
        {
            ProductId = productId;
            this.v1 = v1;
            this.v2 = v2;
        }

        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid _id { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public Guid ProductId { get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.Double)]
        public decimal UnitPrice {  get; set; }

        [BsonRepresentation(MongoDB.Bson.BsonType.Int32)]
        public int Quantity { get; set; }

        // total price = UnitPrice x Quantity
        [BsonRepresentation(MongoDB.Bson.BsonType.Double)]
        public decimal TotalPrice { get; set; }
    }
}
