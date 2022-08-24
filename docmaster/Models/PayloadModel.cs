namespace docmaster.Models
{
    public class PayloadModel
    {
        public string fullName { get; set; }
        public string path { get; set; }
    }

    public class ProtectModel
    {
        public string fullName { get; set; }
        public string path { get; set; }

        public string state { get; set; }
    }
}
