using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Optional;
using Optional.Unsafe;
using TriResultsCsvReader.PipelineSteps;
using TriResultsCsvReader.Utils;

namespace TriResultsCsvReader
{
    // TODO: better name

    public class ProgramRunner : INotifyCollectionChanged
    {
        public ProgramRunner()
        {
            Errors = new List<string>();
            Info = new List<string>();
            Output = new List<string>();
        }

        public List<string> Errors { get; set; }
        public List<string> Info { get; set; }
        public List<string> Output { get; set; }

        public string OutputSql { get; set; }


        public bool Process(IOptions options)
        {
            if (options.Verbose) Console.WriteLine("Filename: {0}", options.InputFolderOrFile);

            var configFile = string.IsNullOrEmpty(options.ConfigFile) ? "column_config.xml" : options.ConfigFile;
            if (!File.Exists(configFile)) Console.WriteLine("Config file not found: {0}", configFile);

            if (string.IsNullOrEmpty(options.MemberFile)) { Console.WriteLine("Member file not given, cannot filter results."); }

            if (null == options.InputFolderOrFile)
            {
                Errors.Add("No input folder given");
                Console.WriteLine("No input folder given, using 'uitslagen'");
                return false;
            }

            var fileUtils = new FileUtils();
            var inputFiles = fileUtils.GetAllFiles(options.InputFolderOrFile);

            if (!inputFiles.Any())
            {
                Errors.Add("No input files in folder or folder given, will do nothing.");
                Console.WriteLine("No input file or folder given, will do nothing.");
                return false;
            }

            if (options.Verbose) { Console.WriteLine("Files to process:\n"); inputFiles.ForEach(file => Console.WriteLine(file)); }


            var columnsConfig = new ColumnsConfigReader().ReadFile(options.ConfigFile ?? "column_config.xml");

            Expression<Func<ResultRow, bool>> filterExp = null;
            if (string.IsNullOrEmpty(options.MemberFile) && string.IsNullOrEmpty(options.FilterKeywords))
            {
                var errorMessage = $"No Member file or filter keywords given"; Console.WriteLine(errorMessage);
            }


            var hasMemberFilter = !string.IsNullOrEmpty(options.MemberFile) && File.Exists(options.MemberFile);
            var hasFilterKeywords = !string.IsNullOrEmpty(options.MemberFile);

            if (!string.IsNullOrEmpty(options.MemberFile) && !File.Exists(options.MemberFile))
            {
                Errors.Add($"Member file for filtering not found: {options.MemberFile}. Select a a file with a list of members");
                return false;
                //var errorMessage = $"Member file for filtering not found: {options.MemberFile}"; Console.WriteLine(errorMessage); throw new FileNotFoundException(errorMessage);
                
            }
            else
            {
                var members = new MemberReaderCsv().Read(options.MemberFile ?? "../../leden2017.csv");
                var memberWhitelist = new WhitelistFilter(members.Select(m => m.Name));
                filterExp = ((row) => memberWhitelist.ExactMatch(row.Naam));
            }

            if (hasMemberFilter && hasFilterKeywords)
            {
                var members = new MemberReaderCsv().Read(options.MemberFile);
                var memberWhitelist = new WhitelistFilter(members.Select(m => m.Name));
                IEnumerable<string> keywords = new List<string>();
                if (!string.IsNullOrEmpty(options.FilterKeywords))
                {
                    keywords = options.FilterKeywords.Split(new List<char> { '\n', ',', ';' }.ToArray()).Select(x => x.Trim());
                }
                var keywordsFilter = new WhitelistFilter(keywords);
                filterExp = ((row) => memberWhitelist.ExactMatch(row.Naam) || keywordsFilter.ContainsMatch(row.Club));
            }



            if (string.IsNullOrEmpty(options.OutputFolder))
            {
                Console.WriteLine("No output folder given, using 'output'");
                options.OutputFolder = "output/";
            }
            if (!Directory.Exists(options.OutputFolder))
            {
                Directory.CreateDirectory(options.OutputFolder);
            }


            var raceDatas = new List<RaceEnvelope>();
            foreach (var file in inputFiles)
            {
                var filePath = Path.Combine(options.InputFolderOrFile, file);

                var race = new FileUtils().GetRaceDataFromFilename(filePath);

                if (options.Verbose)
                {
                    Console.WriteLine($"Parsing file {file}. Race: {race.ValueOrDefault()?.Name}, date: {race.ValueOrDefault().Date}.");
                }


                if(!race.HasValue) continue;

                var stepData = new RaceEnvelope
                {
                    InputFile = filePath,
                    OutputOptions = new List<string> { "csv" },
                    RaceData = race.ValueOr(new Race())
                };

                raceDatas.Add(stepData);
            }

            // order races by newest first
            raceDatas = raceDatas.OrderByDescending(r => r.RaceData.Date).ToList();


            /// Read and filter races
            var allRaces = new List<RaceEnvelope>();
            foreach (var stepData in raceDatas)
            {
                var readAndStandardizeStep = new ReadFileAndStandardizeStep(columnsConfig, Info);

                try
                {
                    var nextStep = readAndStandardizeStep.Process(stepData);
                    allRaces.Add(nextStep);

                    // TODO: notify caller that step is done
                }
                catch (CsvFormatException ex)
                {
                    var race = string.Format("{0} {1}", stepData.RaceData.Name, stepData.RaceData.Date.ValueOrDefault().ToShortDateString());
                    Errors.Add(string.Format("Error when reading race data: {0}", race));
                    Errors.Add(ex.Message);
                    if (ex.InnerException != null)
                    {
                        Errors.Add(ex.InnerException.Message);
                    }
                }
            }

            var filterStep = new FilterReduceStep(columnsConfig, filterExp, Info);

            var filteredRaces = new ConcurrentQueue<RaceEnvelope>();
            Parallel.ForEach(allRaces, (currentRace) =>
            {
                var filteredRace = filterStep.Process(currentRace);
                if (filteredRace.RaceData.Results.Any())
                {
                    // ReSharper disable once AccessToModifiedClosure (concurrent collection, no problem)
                    filteredRaces.Enqueue(filteredRace);
                }

                // TODO: notify caller
            });

            // clean, not necessary, just trying to see if it makes a difference before the GC kicks in
            allRaces.Clear();
            allRaces = null;

            /// Write normalized race data (columns are normalized, results are filtered) output as csv. One file per race.
            var writeStep = new CsvWriterStep();

            foreach (var race in filteredRaces)
            {
                Console.WriteLine("outputfile: " + race.RaceData.ToFilename());
                race.OutputFolder = Path.Combine(options.InputFolderOrFile, options.OutputFolder);
                writeStep.Process(race);
                if (options.Verbose)
                {
                    var raceData = $"{race.RaceData.Date} {race.RaceData}";
                    Info.Add($"Processed {raceData}");
                    // TODO: CollectionChanged($"Processed {raceData}", null);
                }
            }

            /// Combined output steps

            var htmlOutputStep = new CombineOutputHtmlStep();
            var columns = new ColumnsConfigReader().ReadFile(options.ConfigFile);
            var races = filteredRaces.Select(f => f.RaceData).OrderByDescending(r => r.Date.ValueOrDefault()).ToList();
            filteredRaces = null;

            var outputfile = string.Format("{0}_uitslagen", DateTime.Now.ToString("yyyyMMddhhmm"));
            var outputFolder = Path.Combine(options.InputFolderOrFile, options.OutputFolder);
            var output = htmlOutputStep.Process(outputFolder, outputfile, columns, races);

            Info.Add($"Html output to {outputFolder}\\{outputfile}");

            //options.OutputSql = true; // for now, always set it to true, since it's easier when running from within VS
            if (options.OutputSql)
            {
                var sqlCreateTableStep = new SqlCreateTableStep();
                var sqlCreateTableStmt = sqlCreateTableStep.Process(columns);
                Console.WriteLine("Create table syntax: " + sqlCreateTableStmt);

                var sqlInsertOutputStep = new CombineOutputSqlInsertStep();
                OutputSql = sqlInsertOutputStep.Process(outputFolder, outputfile, columns, races);
                Info.Add($"Sql output to {outputFolder}\\{outputfile}.sql");
            }

            return true;
        }


        public bool Test(IOptions options)
        {
            if (options.Verbose) Console.WriteLine("Filename: {0}", options.InputFolderOrFile);

            var configFile = string.IsNullOrEmpty(options.ConfigFile) ? "column_config.xml" : options.ConfigFile;
            if (!File.Exists(configFile)) Console.WriteLine("Config file not found: {0}", configFile);

            if (string.IsNullOrEmpty(options.MemberFile)) { Errors.Add("Member file not given, cannot filter results."); }

            if (null == options.InputFolderOrFile)
            {
                Errors.Add("No input folder given");
                Console.WriteLine("No input folder given, using 'uitslagen'");
                return false;
            }

            var fileUtils = new FileUtils();
            var inputFiles = fileUtils.GetAllFiles(options.InputFolderOrFile);

            if (!inputFiles.Any())
            {
                Errors.Add("No input files in folder or folder given, will do nothing.");
                Console.WriteLine("No input file or folder given, will do nothing.");
                return false;
            }

            if (options.Verbose) { Console.WriteLine("Files to process:\n"); inputFiles.ForEach(file => Console.WriteLine(file)); }


            var columnsConfig = new ColumnsConfigReader().ReadFile(options.ConfigFile ?? "column_config.xml").ToList();

            Expression<Func<ResultRow, bool>> filterExp = null;
            if (string.IsNullOrEmpty(options.MemberFile) && string.IsNullOrEmpty(options.FilterKeywords))
            {
                var errorMessage = $"No Member file or filter keywords given"; Console.WriteLine(errorMessage);
            }


            var hasMemberFilter = !string.IsNullOrEmpty(options.MemberFile) && File.Exists(options.MemberFile);
            var hasFilterKeywords = !string.IsNullOrEmpty(options.MemberFile);

            if (!string.IsNullOrEmpty(options.MemberFile) && !File.Exists(options.MemberFile))
            {
                Info.Add($"Member file for filtering not found: {options.MemberFile}. Select a a file with a list of members");
                //return false;
                //var errorMessage = $"Member file for filtering not found: {options.MemberFile}"; Console.WriteLine(errorMessage); throw new FileNotFoundException(errorMessage);
            }
            else
            {
                var members = new MemberReaderCsv().Read(options.MemberFile ?? "../../leden2017.csv");
                var memberWhitelist = new WhitelistFilter(members.Select(m => m.Name));
                filterExp = ((row) => memberWhitelist.ExactMatch(row.Naam));
            }


            if (hasMemberFilter && hasFilterKeywords)
            {
                var members = new MemberReaderCsv().Read(options.MemberFile);
                var memberWhitelist = new WhitelistFilter(members.Select(m => m.Name));
                IEnumerable<string> keywords = new List<string>();
                if (!string.IsNullOrEmpty(options.FilterKeywords))
                {
                    keywords = options.FilterKeywords.Split(new List<char> { '\n', ',', ';' }.ToArray()).Select(x => x.Trim());
                }
                var keywordsFilter = new WhitelistFilter(keywords);
                filterExp = ((row) => memberWhitelist.ExactMatch(row.Naam) || keywordsFilter.ContainsMatch(row.Club));
            }



            if (string.IsNullOrEmpty(options.OutputFolder))
            {
                Console.WriteLine("No output folder given, using 'output'");
                options.OutputFolder = "output/";
            }
            if (!Directory.Exists(options.OutputFolder))
            {
                Directory.CreateDirectory(options.OutputFolder);
            }


            /// Read and filter races

            var filteredRaces = new List<RaceEnvelope>();
            foreach (var file in inputFiles)
            {
                var filePath = Path.Combine(options.InputFolderOrFile, file);
                Console.WriteLine("filePath: " + filePath);
                var raceData = new FileUtils().GetRaceDataFromFilename(filePath);

                Console.Write("P! race (from filename): {0}, date: {1}", raceData.ValueOr(new Race()).Name, raceData.ValueOr(new Race()).Date);

                if (options.Verbose)
                {
                    Console.WriteLine($"Parsing file {file}. Race: {raceData.ValueOr(new Race()).Name}, date: {raceData.ValueOrDefault().Date}");
                }


                var race = new Race();
                race.Date = (options.RaceDate == DateTime.MinValue)
                    ? Option.None<DateTime>()
                    : Option.Some(options.RaceDate);
                race.Name = string.IsNullOrEmpty(options.RaceName)
                    ? Option.None<string>()
                    : Option.Some(options.RaceName);
                
                var stepData = new RaceEnvelope
                {
                    InputFile = filePath,
                    OutputOptions = new List<string> { "csv" },
                    RaceData = race
                };

                var readAndFilterStep = new ReadFileAndStandardizeStep(columnsConfig, Info);

                try
                {
                    var nextStep = readAndFilterStep.Process(stepData);

                    if (nextStep.RaceData.Results.Any())
                        filteredRaces.Add(nextStep);
                }
                catch (CsvFormatException ex)
                {
                    var raceStr = string.Format("{0} {1}", stepData.RaceData.Name, stepData.RaceData.Date);
                    Errors.Add(string.Format("Error when reading race data: {0}", raceStr));
                    Errors.Add(ex.Message);
                    if (ex.InnerException != null)
                    {
                        Errors.Add(ex.InnerException.Message);
                    }
                }
            }

            // order races by newest first
            filteredRaces = filteredRaces.OrderByDescending(r => r.RaceData.Date).ToList();


            /// Write normalized race data (columns are normalized, results are filtered) output as csv. One file per race.
            var writeStep = new CsvWriterStep();

            foreach (var race in filteredRaces)
            {
                Console.WriteLine("outputfile: " + race.RaceData.ToFilename());
                var inputFolder = options.InputFolderOrFile;
                var fi = new FileInfo(inputFolder);
                if (fi.Exists)
                {
                    inputFolder = fi.Directory.FullName;
                }

                race.OutputFolder = Path.Combine(inputFolder, options.OutputFolder);
                writeStep.Process(race);
                if (options.Verbose)
                {
                    var raceData = $"{race.RaceData.Date} {race.RaceData}";
                    Info.Add($"Processed {raceData}");
                    // TODO: CollectionChanged($"Processed {raceData}", null);
                }
            }


            // output test file
            foreach (var race in filteredRaces)
            {
                Output.Add($"Race {race.RaceData.Name} on {race.RaceData.Date.ValueOrDefault().ToShortDateString()}  type: {race.RaceData.RaceType}");

                foreach (var line in race.RaceData.Results)
                {
                    Output.Add($"{line.Naam}  {line.Total}");
                }

            }


                ///// Combined output steps

                //var htmlOutputStep = new CombineOutputHtmlStep();
                //var columns = new ColumnsConfigReader().ReadFile(options.ConfigFile);
                //var races = filteredRaces.Select(f => f.RaceData).ToList();

                //var outputfile = string.Format("{0}_uitslagen", DateTime.Now.ToString("yyyyMMddhhmm"));
                //var output = htmlOutputStep.Process("output", outputfile, columns, races);

                //Info.Add($"Html output to {outputfile}");

                //options.OutputSql = true; // for now, always set it to true, since it's easier when running from within VS
                //if (options.OutputSql)
                //{
                //    var sqlCreateTableStep = new SqlCreateTableStep();
                //    var sqlCreateTableStmt = sqlCreateTableStep.Process(columns);
                //    Console.WriteLine("Create table syntax: " + sqlCreateTableStmt);

                //    var sqlInsertOutputStep = new CombineOutputSqlInsertStep();
                //    OutputSql = sqlInsertOutputStep.Process("output", outputfile, columns, races);
                //    Info.Add($"Sql output to {outputfile}.sql");
                //}

                return !Errors.Any();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnCollectionChanged(string name)
        {
            NotifyCollectionChangedEventHandler handler = CollectionChanged;
            if (handler != null)
            {
                //handler(this, new NotifyCollectionChangedEventArgs());
            }
        }
    }
}
