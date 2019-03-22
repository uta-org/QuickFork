using Newtonsoft.Json;
using System;
using System.IO;

namespace QuickFork.Lib.Model
{
    using Interfaces;

    /// <summary>
    /// The ProjectItem class
    /// </summary>
    /// <seealso cref="QuickFork.Lib.Model.Interfaces.IModel" />
    [Serializable]
    public class ProjectItem : IModel
    {
        /// <summary>
        /// Gets the selected path.
        /// </summary>
        /// <value>
        /// The selected path.
        /// </value>
        [JsonProperty]
        public string SelectedPath { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [JsonIgnore]
        public string Name => Path.GetFileName(SelectedPath);

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [JsonIgnore]
        public OperationType Type { get; set; }

        /// <summary>
        /// Prevents a default instance of the <see cref="ProjectItem"/> class from being created.
        /// </summary>
        private ProjectItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectItem"/> class.
        /// </summary>
        /// <param name="selectedPath">The selected path.</param>
        public ProjectItem(string selectedPath)
        {
            SelectedPath = selectedPath;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Name;
        }
    }
}