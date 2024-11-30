namespace RabbitmqExample.Consumers
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ManualResetEvent _quitEvent = new ManualResetEvent(false);

            Consumer fiz = new Consumer("Fiz", new List<string> { "fiz" });
            fiz.StartConsuming();
            Consumer buz = new Consumer("Buz", new List<string> { "buz" });
            buz.StartConsuming();
            Consumer wiz = new Consumer("Wiz", new List<string> { "wiz" });
            wiz.StartConsuming();
            Consumer fizBuz = new Consumer("FizBuz", new List<string> { "fiz", "buz" });
            fizBuz.StartConsuming();
            Consumer fizWiz = new Consumer("FizWiz", new List<string> { "fiz", "wiz" });
            fizWiz.StartConsuming();
            Consumer buzWiz = new Consumer("BuzWiz", new List<string> { "buz", "wiz" });
            buzWiz.StartConsuming();
            Consumer fizBuzWiz = new Consumer(
                "FizBuzWiz",
                new List<string> { "fiz", "buz", "wiz" }
            );
            fizBuzWiz.StartConsuming();

            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                _quitEvent.Set();
            };

            _quitEvent.WaitOne();
        }
    }
}
