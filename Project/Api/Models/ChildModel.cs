namespace Api.Models
{
    public abstract class ChildModel : BaseModel
    {
        public int Parent { get; set; }

        public bool isEqualTo(ChildModel other)
        {
            return (this.isEqualTo((BaseModel)other)
                && this.Parent == other.Parent);
        }
    }
}