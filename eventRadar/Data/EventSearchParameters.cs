namespace eventRadar.Data.Dtos
{
    public class EventSearchParameters
    {
        private int _pageSize = 45;
        private const int MaxPageSize = 45;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
