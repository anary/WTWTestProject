using System;
using CPR.Core.Interfaces;
using Unity;

namespace ReportorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //Register container
            var container = Container.Container.Register();

            InteractWithUser(container);
        }

        #region Private Methods

        delegate void WriteDelegate(string format);
        private static void InteractWithUser(UnityContainer container)
        {
            WriteDelegate write = Console.WriteLine;
            ConsoleKeyInfo cki;

            do
            {
                // read txt data file name
                write("Enter data txt file name:");
                string dataFileName = Console.ReadLine();

                //read target file name
                write("Enter target file name:");
                string targetFilename = Console.ReadLine();

                try
                {
                    var report = container.Resolve<IClaimPaymentReport>();
                    report.Generator($@"\{dataFileName}.txt", $@"\{targetFilename}.txt").GetAwaiter().GetResult();
                    write($"you can find the {targetFilename} file in application root\n");
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message + "\n");
                    Console.ResetColor();
                }

                write("Press the Escape (Esc) key to quit or any key to contiue: \n");
                cki = Console.ReadKey();
            } while (cki.Key != ConsoleKey.Escape);
        }

        #endregion
    }
}
