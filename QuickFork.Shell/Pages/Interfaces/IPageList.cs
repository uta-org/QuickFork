namespace QuickFork.Shell.Pages.Interfaces
{
    using Lib.Model.Interfaces;

    internal interface IPageList<T>
        where T : IModel
    {
        T NewItem { get; set; }
    }
}