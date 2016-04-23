namespace EMRCorefResol.TestingGUI
{
    /// <summary>
    /// Represents an interface that should be implemented by views that decide whether they are 
    /// document or anchorable
    /// </summary>
    public interface IDecideDockableType
    {
        DockableType DockableType { get; }
    }
}
