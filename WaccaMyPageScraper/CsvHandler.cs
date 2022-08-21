using CsvHelper;
using CsvHelper.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaccaMyPageScraper.Data;

namespace WaccaMyPageScraper
{
    public class CsvHandler<T>
    {
        private readonly ILogger? _logger;

        public IEnumerable<T> Records { get; set; }

        public CsvHandler()
        {
            Records = new List<T>();
        }

        public CsvHandler(ILogger? _logger)
        {
            this._logger = _logger;
            Records = new List<T>();
        }

        public CsvHandler(IEnumerable<T> records, ILogger? _logger = null)
        {
            this._logger = _logger;
            Records = records;
        }

        public IEnumerable<T>? Import(string? filePath)
        {
            try
            {
                Path.GetFullPath(filePath);
            }
            catch (Exception ex)
            {
                _logger?.Error("Invalid path!");
                _logger?.Debug("Detail: {ExceptionMessage}", ex.Message);

                return null;
            }

            var fileFullPath = Path.GetFullPath(filePath);
            if (!File.Exists(fileFullPath))
            {
                _logger?.Error("Cannot find a file!");

                return null;
            }

            IEnumerable<T>? records = null;
            try
            {
                using (var reader = new StreamReader(fileFullPath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var classMap = this.GetClassMap<T>();
                    if (classMap is not null)
                        csv.Context.RegisterClassMap(classMap);

                    records = csv.GetRecords<T>().ToList();
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex.Message);

                return null;
            }
            
            return records;
        }

        public void Export(string? directoryUri, string? fileName)
        {
            directoryUri ??= Directory.GetCurrentDirectory();

            var filePath = Path.Combine(directoryUri, fileName);
            try
            {
                Path.GetFullPath(filePath);
            }
            catch (Exception ex)
            {
                _logger?.Error("Invalid path!");
                _logger?.Debug("Detail: {ExceptionMessage}", ex.Message);

                return;
            }

            try
            {
                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    var classMap = this.GetClassMap<T>();
                    if (classMap is not null)
                        csv.Context.RegisterClassMap(classMap);

                    csv.WriteRecords(this.Records);
                }
            }
            catch (Exception ex)
            {
                _logger?.Error(ex.Message);
                _logger?.Debug(ex.StackTrace);

                return;
            }

            _logger.Information("Csv file exported to {FilePath}", filePath);
        }

        private Type? GetClassMap<T>() => typeof(T) switch
        {
            var t when t.Name == typeof(Player).Name => typeof(PlayerMap),
            var t when t.Name == typeof(Stage).Name => typeof(StageMap),
            var t when t.Name == typeof(StageDetail).Name => typeof(StageDetailMap),
            var t when t.Name == typeof(Music).Name => typeof(MusicMap),
            var t when t.Name == typeof(MusicDetail).Name => typeof(MusicDetailMap),
            var t when t.Name == typeof(Trophy).Name => typeof(TrophyMap),
            _ => null
        };
    }
}
