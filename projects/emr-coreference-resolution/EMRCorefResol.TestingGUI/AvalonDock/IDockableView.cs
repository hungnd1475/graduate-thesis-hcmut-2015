namespace EMRCorefResol.TestingGUI
{
    /// <summary>
    /// Represents an interface that should be implemented by views that decide whether they are 
    /// document or anchorable
    /// </summary>
    public interface IDockableView
    {
        DockableType DockableType { get; }
        string Title { get; }
        string ContentId { get; }
    }
}
