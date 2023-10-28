namespace Practice.Gateway.Dto
{
    public class UserResponse
    {
        public int id { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public List<PostResponse> posts { get; set; }
    }
}
