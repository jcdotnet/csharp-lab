namespace Asyncrhonous
{
    public class HttpServerAsync(string url)
    {
        public string Url => url;
        public int SimulatedDelay { get; set; }

        public async Task<string> GetUrlContentAsync(CancellationToken cancellationToken, int simulatedDelay = 0)
        {
            using var client = new HttpClient();

            Task<string> getStringTask = client.GetStringAsync(url, cancellationToken);

            Task independentWorkTask = DoIndependentWork(cancellationToken, simulatedDelay);

            await Task.WhenAll(getStringTask, independentWorkTask);

            return await getStringTask;

        }

        private async Task DoIndependentWork(CancellationToken cancellationToken, int simulatedDelay)
        {
            if (simulatedDelay > 0) await Task.Delay(TimeSpan.FromSeconds(simulatedDelay), cancellationToken);
            
            Console.WriteLine("Some independent work...");
        }
    }
}
