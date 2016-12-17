using System.Collections.Generic;

namespace MovieDB
{
    public class MovieDBResponse
    {
        public int page { get; set; }
        public List<MovieDBResult> results { get; set; }
        public int total_results { get; set; }
        public int total_pages { get; set; }
    }
}