using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
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
                var errorMessage = $"Member file for filtering not found: {options.MemberFile}"; Console.WriteLine(errorMessage); throw new FileNotFoundException(errorMessage);
                
            }
            else
            {
                var members = new MemberReaderCsv().Read(options.MemberFile ?? "../../leden2017.csv");
                var memberWhitelist = new WhitelistFilter(members.Select(m => m.Name));
                filterExp = ((row) => memberWhitelist.ExactMatch(row.Naam));
            }

            //if(hasFilterKeywords)
            //{
            //    var keywords = options.FilterKeywords.Split(new List<char> { '\n', ',', ';' }.ToArray());
            //    var keywordsFilter = new WhitelistFilter(keywords);
            //    filterExp = ((row) =>  keywordsFilter.ContainsMatch(row.Club));
            //}

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

            var filteredRaces = new List<StepData>();
            foreach (var file in inputFiles)
            {
                var filePath = Path.Combine(options.InputFolderOrFile, file);
                Console.WriteLine("filePath: " + filePath);
                var raceData =  GetRaceFileData(filePath, options.OutputFolder, options.RaceDate);

                Console.Write("P! race (from filename): {0}, date: {1}", raceData.Name, raceData.Date);

                if (options.Verbose)
                {
                    Console.WriteLine($"Parsing file {file}. Race: {raceData.Name}, date: {raceData.Date}. Output file: {raceData.OutputFile}");
                }

                var raceFileData = GetRaceDataFromFilename(filePath);

                var stepData = new StepData
                {
                    InputFile = filePath,
                    Filter = filterExp,
                    ColumnConfigFile = options.ConfigFile,
                    OutputOptions = new List<string> { "csv" },
                    RaceData = raceFileData.ValueOr(new Race())
                };

                var readAndFilterStep = new StandardizeHeadersAndFilterStep(columnsConfig, true);

                try
                {
                    var nextStep = readAndFilterStep.Process(stepData);

                    if (nextStep.RaceData.Results.Any())
                        filteredRaces.Add(nextStep);
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

            // order races by newest first
            filteredRaces = filteredRaces.OrderByDescending(r => r.RaceData.Date).ToList();


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
            var races = filteredRaces.Select(f => f.RaceData).ToList();

            var outputfile = string.Format("{0}_uitslagen", DateTime.Now.ToString("yyyyMMddhhmm"));
            var outputFolder = Path.Combine(options.InputFolderOrFile, options.OutputFolder);
            var output = htmlOutputStep.Process(outputFolder, outputfile, columns, races);

            Info.Add($"Html output to {outputFolder}\\{outputfile}");

            options.OutputSql = true; // for now, always set it to true, since it's easier when running from within VS
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


        protected Option<Race> GetRaceDataFromFilename(string filename)
        {
            var file = filename.Substring(filename.LastIndexOf('\\') + 1);

            var raceDate = DateUtils.FromFilename(filename);
            var raceName = Option.None<string>();

            if (raceDate.HasValue)
            {
                raceName = Option.Some<string>(file.Substring(10).Replace(".csv", "").Replace("-", " ").Replace("_", ""));

                return Option.Some(new Race() {Date = raceDate, Name = raceName});
            }

            return Option.None<Race>();
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

            var filteredRaces = new List<StepData>();
            foreach (var file in inputFiles)
            {
                var filePath = Path.Combine(options.InputFolderOrFile, file);
                Console.WriteLine("filePath: " + filePath);
                var raceData = GetRaceFileData(filePath, options.OutputFolder, options.RaceDate);

                Console.Write("P! race (from filename): {0}, date: {1}", raceData.Name, raceData.Date);

                if (options.Verbose)
                {
                    Console.WriteLine($"Parsing file {file}. Race: {raceData.Name}, date: {raceData.Date}. Output file: {raceData.OutputFile}");
                }


                var race = new Race();
                race.Date = (options.RaceDate == DateTime.MinValue)
                    ? Option.None<DateTime>()
                    : Option.Some(options.RaceDate);
                race.Name = string.IsNullOrEmpty(options.RaceName)
                    ? Option.None<string>()
                    : Option.Some(options.RaceName);
                
                var stepData = new StepData
                {
                    InputFile = filePath,
                    Filter = filterExp,
                    ColumnConfigFile = options.ConfigFile,
                    OutputOptions = new List<string> { "csv" },
                    RaceData = race
                };

                var readAndFilterStep = new StandardizeHeadersAndFilterStep(columnsConfig, true);

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




        public static RaceFileData GetRaceFileData(string filename, string outputDir, DateTime raceDate)
        {
            var fi = new FileInfo(filename);

            var index = fi.Name.IndexOf(".csv");
            var name = DateUtils.ReplaceStringMonth(fi.Name);

            RaceFileData output = new RaceFileData() { Name = name };

            if (raceDate.Equals(DateTime.MinValue))
            {
                Console.WriteLine("raceDate is null, will try to get date from filename");
                var datePart = name.Substring(0, 10);
                if (DateTime.TryParseExact(datePart, "yyyy-MM-dd", Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out var racedate))
                {
                    Console.WriteLine("Got raceDate from filename: " + racedate.ToShortDateString());
                    output.Date = racedate;
                    output.Name = name.Substring(9); // remove date
                    raceDate = racedate;
                }
                else
                {
                    //throw new FormatException("Filename must start with date in format yyyyMMdd_racefile.csv");
                }
            }
            else
            {
                Console.WriteLine("Using racedate given by args: " + (raceDate == null ? default(DateTime) : raceDate).ToShortDateString());
                output.Date = raceDate;
            }

            output.Name = output.Name.Replace(".csv", "");
            var raceDateStr = DateTime.MinValue == raceDate ? "" : raceDate.ToString("yyyyMMdd");

            output.FullPath = Path.Combine(fi.Directory.FullName, outputDir);
            output.OutputFile = Path.Combine(fi.Directory.FullName, outputDir, name);
            Console.WriteLine("OutputFile: " + output.OutputFile);
            return output;
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
