using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UpperityProfileManagement.Models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class CDraftOrder
    {
        [JsonProperty("draft_order")]
        public DraftOrder DraftOrder { get; set; }
    }

    public partial class DraftOrder
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("note")]
        public object Note { get; set; }

        [JsonProperty("email")]
        public object Email { get; set; }

        [JsonProperty("taxes_included")]
        public bool TaxesIncluded { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("invoice_sent_at")]
        public object InvoiceSentAt { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("tax_exempt")]
        public bool TaxExempt { get; set; }

        [JsonProperty("completed_at")]
        public object CompletedAt { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("line_items")]
        public List<LineItem> LineItems { get; set; }

        [JsonProperty("shipping_address")]
        public object ShippingAddress { get; set; }

        [JsonProperty("billing_address")]
        public object BillingAddress { get; set; }

        [JsonProperty("invoice_url")]
        public Uri InvoiceUrl { get; set; }

        [JsonProperty("applied_discount")]
        public object AppliedDiscount { get; set; }

        [JsonProperty("order_id")]
        public object OrderId { get; set; }

        [JsonProperty("shipping_line")]
        public object ShippingLine { get; set; }

        [JsonProperty("tax_lines")]
        public List<object> TaxLines { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("note_attributes")]
        public List<object> NoteAttributes { get; set; }

        [JsonProperty("total_price")]
        public string TotalPrice { get; set; }

        [JsonProperty("subtotal_price")]
        public string SubtotalPrice { get; set; }

        [JsonProperty("total_tax")]
        public string TotalTax { get; set; }

        [JsonProperty("admin_graphql_api_id")]
        public string AdminGraphqlApiId { get; set; }

        [JsonProperty("customer")]
        public object Customer { get; set; }
    }

    public partial class LineItem
    {
        [JsonProperty("variant_id")]
        public long VariantId { get; set; }

        [JsonProperty("product_id")]
        public long ProductId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("variant_title")]
        public object VariantTitle { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("vendor")]
        public string Vendor { get; set; }

        [JsonProperty("quantity")]
        public long Quantity { get; set; }

        [JsonProperty("requires_shipping")]
        public bool RequiresShipping { get; set; }

        [JsonProperty("taxable")]
        public bool Taxable { get; set; }

        [JsonProperty("gift_card")]
        public bool GiftCard { get; set; }

        [JsonProperty("fulfillment_service")]
        public string FulfillmentService { get; set; }

        [JsonProperty("grams")]
        public long Grams { get; set; }

        [JsonProperty("tax_lines")]
        public List<object> TaxLines { get; set; }

        [JsonProperty("applied_discount")]
        public object AppliedDiscount { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("properties")]
        public List<object> Properties { get; set; }

        [JsonProperty("custom")]
        public bool Custom { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("admin_graphql_api_id")]
        public string AdminGraphqlApiId { get; set; }
    }
}
