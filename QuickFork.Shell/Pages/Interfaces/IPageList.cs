namespace QuickFork.Shell.Pages.Interfaces
{
    using Lib.Model.Interfaces;

    /// <summary>
    /// IPageList interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IPageList<T>
        where T : IModel
    {
        /// <summary>
        /// Gets or sets the new item.
        /// </summary>
        /// <value>
        /// The new item.
        /// </value>
        T NewItem { get; set; }
    }
}