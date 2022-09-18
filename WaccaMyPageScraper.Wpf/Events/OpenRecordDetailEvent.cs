using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Wpf.Models;

namespace WaccaMyPageScraper.Wpf.Events
{
    public sealed class OpenRecordDetailEvent : PubSubEvent<RecordDetailModel> { }
}
