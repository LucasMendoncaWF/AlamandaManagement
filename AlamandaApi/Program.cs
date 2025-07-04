namespace AlamandaApi {
  public class Program {
    public static IHostBuilder CreateHostBuilder(string[] args) =>
      Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder => {
          webBuilder.UseStartup<Startup>();
        });

    public static void Main(string[] args) {
      var envFileName = Environment.GetEnvironmentVariable("DOTNET_ENV_FILE") ?? ".env";
      DotNetEnv.Env.Load(envFileName);
      CreateHostBuilder(args).Build().Run();
    }
  }
}
