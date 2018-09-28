namespace  FreedomFridayServerless.Configuration
{
    public class JournalSettings
    {
        public string BaseUrl { get;set; }
        public string FunctionName {get;set;}
        public Endpoints Endpoints { get;set; }
    }

    public class Endpoints
    {
        public string AddJournal { get; set; }
    }
}