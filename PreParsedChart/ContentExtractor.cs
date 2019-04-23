using CommonCode.Charts;
using CommonCode.Interfaces;
using DialogService;
using System;
using System.IO;
using System.Xml.Linq;

namespace GmDashboard.ChartBuilder
{
    public interface IContentExtractor
    {
        IChart ExtractFromText(FileInfo file);
        IChart ExtractFromRgf(FileInfo file);
        IChart ExtractFromPowerShell(FileInfo file);
    }

    public class ContentExtractor : IContentExtractor
    {
        protected IChartBuilder builder;

        public ContentExtractor()
        {
            builder = new ChartBuilder();
        }

        private string ExtractStringFromFile(FileInfo file)
        {
            string extractedValue = "";
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(file.FullName))
                {
                    // Read the stream to a string, and write the string to the console.
                    extractedValue = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                //TODO Log4Net
            }

            return extractedValue;
        }

        public IChart ExtractFromText(FileInfo file)
        {
            var extracted = ExtractStringFromFile(file);
            return builder.BuildFromTxt(extracted);
        }

        public IChart ExtractFromRgf(FileInfo file)
        {
            return builder.BuildFromRgf(XDocument.Load(file.FullName));
        }

        public IChart ExtractFromPowerShell(FileInfo file)
        {
            var extractedString = ExtractStringFromFile(file);
            var extractedParams = builder.BuildFromParams(extractedString);
            var powerShellParams = Dialogs.ExtractPowerShellParameters(extractedParams);
            return new Chart();
        }
    }
}
