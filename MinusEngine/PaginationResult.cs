using System;
using System.Text;
using Newtonsoft.Json;

namespace MinusEngine
{
    public class PaginationResult
    {
        [JsonProperty("page")]
        public Int32 Page { get; set; }

        [JsonProperty("next")]
        public String Next { get; set; }

        [JsonProperty("per_page")]
        public Int32 PerPage { get; set; }

        [JsonProperty("total")]
        public Int32 TotalItems { get; set; }

        [JsonProperty("pages")]
        public Int32 NumPages { get; set; }

        [JsonProperty("previous")]
        public String Previous { get; set; }

        public PaginationResult()
        {
            
        }

        public PaginationResult(Int32 page, String next, Int32 perPage, Int32 totalItems, Int32 numPages, String previous)
        {
            Page = page;
            Next = next;
            PerPage = perPage;
            TotalItems = totalItems;
            NumPages = numPages;
            Previous = previous;
        }

        #region Low level overrides

        public override string ToString()
        {
            return new StringBuilder("PaginationResults{")
                .Append(", _Page=").Append(Page)
                .Append(", _Next=").Append(Next)
                .Append(", _PerPage=").Append(PerPage)
                .Append(", _TotalItems=").Append(TotalItems)
                .Append(", _NumPages=").Append(NumPages)
                .Append(", Previous=").Append(Previous)
                .Append('}').ToString();
        }

        #endregion
    }
}
