namespace MistakeDance.Domain
{
    public class Branch : BaseEntity
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Address { get; set; }
    }
}