namespace  FreedomFridayServerless.Configuration
{
    public class OrchestratorSettings
    {
        public string BaseUrl { get;set; }
        public string FunctionName {get;set;}
        public Endpoint Endpoints { get;set; }
    }

    public class Endpoint
    {
        public string Start { get; set; }
    }
}