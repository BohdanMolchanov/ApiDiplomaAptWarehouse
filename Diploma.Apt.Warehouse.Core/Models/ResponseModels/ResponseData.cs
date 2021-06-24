using System.Collections.Generic;

namespace Diploma.Apt.Warehouse.Core.Models.ResponseModels
{
    public class ResponseData
    {
        public object Data { get; set; }
        public Dictionary<string, object> Meta { get; set; }
        public Dictionary<string, object> Populated { get; set; }
        public PaginationResponse Paging { get; set; }
        
        public ResponseData(){}

        public ResponseData(object data)
        {
            Data = data;
        }

        public void AddPaginationData(long length, int page, int limit)
        {
            Paging = new PaginationResponse(length, page, limit);
        }
        
        
        public class PaginationResponse
        {
            public long Length { get; internal set; }
            public int Page { get; }
            public int? Limit { get; }

            public PaginationResponse(long length, int page = 1, int? limit = null)
            {
                Length = length;
                Page = page;
                Limit = limit;
            }
        }
    }
}