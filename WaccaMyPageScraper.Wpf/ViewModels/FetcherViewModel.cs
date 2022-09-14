using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaccaMyPageScraper.Wpf.ViewModels
{
    /// <summary>
    /// An abstract class of all fetcher releated ViewModels.
    /// </summary>
    public abstract class FetcherViewModel : BindableBase
    {
        public FetcherViewModel() => InitializeData();

        /// <summary>
        /// Initialize data and might be processed or showed.
        /// </summary>
        public abstract void InitializeData();

        /// <summary>
        /// An event callback interface for fetcher event.
        /// </summary>
        public abstract void FetcherEvent();
    }
}
