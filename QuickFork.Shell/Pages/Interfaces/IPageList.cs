using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickFork.Shell.Pages.Interfaces
{
    using Lib.Model.Interfaces;

    internal interface IPageList<T>
        where T : IModel
    {
        T NewItem { get; set; }
    }
}