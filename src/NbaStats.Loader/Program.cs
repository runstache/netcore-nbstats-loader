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
            List<string> files = Directory.GetFiles(settings.ImportDirectory).ToList();
            files.Sort();
            foreach (string file in files.Where(c => !c.Contains("injuries")))
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

            string injuryFile = Directory.GetFiles(settings.ImportDirectory).ToList().Where(c => c.Contains("injuries")).FirstOrDefault();
            if (!string.IsNullOrEmpty(injuryFile) && !settings.ScheduleOnly)
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
