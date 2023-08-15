namespace API_Test1
{
    public class Pagination
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int TotalItem { get; set; }
        public int TotalPage
        {
            get
            {
                if (PageSize == 0) return 0;
                var total = TotalItem / PageSize;
                if (TotalItem % PageSize != 0)  total += 1;
                return total;
            }
        }
        public Pagination()
        {
            PageSize = -1;
            PageNumber = 1;
        }
    }
}
