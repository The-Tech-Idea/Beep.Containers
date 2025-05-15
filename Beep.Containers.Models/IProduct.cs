namespace TheTechIdea.Beep.Container.Model
{
    public interface IProduct
    {
        string Description { get; set; }
        string GuidID { get; set; }
        int ID { get; set; }
        string ProductID { get; set; }
        string Version { get; set; }
    }
}