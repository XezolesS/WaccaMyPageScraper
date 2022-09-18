using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Wpf.Events;
using WaccaMyPageScraper.Wpf.Models;

namespace WaccaMyPageScraper.Wpf.ViewModels
{
    public sealed class RecordDetailWindowViewModel : BindableBase
    {
        private RecordDetailModel _recordDetails;
        public RecordDetailModel RecordDetails
        {
            get => _recordDetails;
            set => SetProperty(ref _recordDetails, value);
        }

        public RecordDetailWindowViewModel(IEventAggregator ea)
        {
            ea.GetEvent<OpenRecordDetailEvent>().Subscribe(OnWindowOpenEvent);
        }

        public void OnWindowOpenEvent(RecordDetailModel model)
        {
            this.RecordDetails = model;
        }
    }
}
