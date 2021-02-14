using System;
using System.Collections.Generic;
using NbaStats.Loader.DataObject;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using NbaStats.Loader.Configuration;
using NbaStats.Data.Repositories;
using NbaStats.Loader.Processors;
using System.Linq;


namespace NbaStats.Loader
{
    class Program
    {
        static void Main()
        {
            IServiceCollection services = new ServiceCollection();
            ConfigureServices(services);

            IServiceProvider provider = services.BuildServiceProvider();
            AppSettings settings = provider.GetService<AppSettings>();
            if (string.IsNullOrEmpty(settings.LogFile))
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console(Serilog.Events.LogEventLevel.Information)
                    .CreateLogger();
            }
            else
            {
                Directory.CreateDirectory(Path.GetDirectoryName(settings.LogFile));

                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console(Serilog.Events.LogEventLevel.Information)
                    .WriteTo.File(settings.LogFile, Serilog.Events.LogEventLevel.Information)
                    .CreateLogger();

            }

            var logger = provider.GetService<ILogger<Program>>();

            logger.LogInformation("READY TO IMPORT. PRESS ANY KEY TO BEGIN..");
            Console.ReadKey();

            

            IRepository repo = provider.GetService<IRepository>();

            logger.LogInformation("Retrieving Games");
            List<string> files = Directory.GetFiles(settings.ImportDirectory + Path.AltDirectorySeparatorChar + "games").ToList();
            files.Sort();
            foreach (string file in files)
            {
                try
                {

                    logger.LogInformation($"Processing Game File: {file}");
                    string json = File.ReadAllText(file);
                    GameEntry game = JsonConvert.DeserializeObject<GameEntry>(json);
                    IProcessor<GameEntry> processor = new GameProcessor(repo, settings, logger);
                    processor.Process(game);
                    logger.LogInformation($"Finished Processing Game File: {file}");
                }
                catch (Exception ex)
                {
                    logger.LogError($"Error Processing File: {file} : {ex.Message}", ex);
                    continue;
                }
            }

            string[] injuryFiles = Directory.GetFiles(settings.ImportDirectory + Path.AltDirectorySeparatorChar + "injuries");
            if (!settings.ScheduleOnly)
            {
                foreach (string injuryFile in injuryFiles)
                {
                    try
                    {
                        string json = File.ReadAllText(injuryFile);
                        IProcessor<InjuryEntry> processor = new InjuryProcessor(repo, logger);
                        JArray array = JArray.Parse(json);
                        foreach (JObject obj in array)
                        {
                            InjuryEntry injury = JsonConvert.DeserializeObject<InjuryEntry>(obj.ToString());
                            processor.Process(injury);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Error processing Injuries: {ex.Message}", ex);
                    }
                }
            }

            if (!settings.ScheduleOnly || settings.RostersOnly)
            {
                string[] rosters = Directory.GetFiles(settings.ImportDirectory + Path.AltDirectorySeparatorChar + "rosters");

                foreach (string roster in rosters)
                {
                    logger.LogInformation($"Processing Roster {roster}");
                    string json = File.ReadAllText(roster);
                    RosterItem item = JsonConvert.DeserializeObject<RosterItem>(json);
                    RosterProcessor rosterProcessor = new RosterProcessor(repo, logger);
                    rosterProcessor.Process(item);

                    logger.LogInformation($"Finished Processing Roster {roster}");
                }
            }

            logger.LogInformation("FINISHED.");
            Console.ReadKey();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            AppSettings settings = new AppSettings();
            config.GetSection("application").Bind(settings);

            services.AddSingleton(settings);

            IRepository repo = new SqlRepository(settings.ConnectionString);
            services.AddSingleton(repo);

            services.AddLogging(config => config.AddSerilog());                                 
        }
    }
}
