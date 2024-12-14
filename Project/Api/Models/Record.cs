namespace Api.Models
{
    public class Record : ChildModel
    {
        public string Content { get; set; }

        public bool isEqualTo(Record other)
        {
            return (this.isEqualTo((ChildModel)other)
                && this.Content == other.Content);
        }
    }
}