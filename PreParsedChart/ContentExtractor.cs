using CommonCode.Blocks;
using CommonCode.Rolls;
using System;
using System.IO;
using System.Xml.Linq;

namespace GmDashboard.BlockBuilder
{
    public interface IContentExtractor
    {
        IChart ExtractData(FileInfo file);
    }

    public class ContentExtractor : IContentExtractor
    {
        protected IBlockBuilder parseBlock;
        protected IBlockBuilder parseBlockV2;

        public ContentExtractor()
        {
            parseBlockV2 = new BlockBuilder();
        }

        public IChart ExtractData(FileInfo file)
        {
            switch (file.Extension)
            {
                case ".txt":
                    {
                        return TxtExtractor(file);
                    }
                case ".rgf":
                    {
                        return RollMeOneFormatExtractor(file);
                    }
                case ".ps1":
                    {
                        return RunPowershellFunction(file);
                    }

            }
            return new Chart();
        }
 
        private IChart RunPowershellFunction(FileInfo powershell)
        {
            return null;
        }

        private IChart RollMeOneFormatExtractor(FileInfo chartPath)
        {
            return parseBlockV2.BuildFromRgf(XDocument.Load(chartPath.FullName));
        }

        public IChart TxtExtractor(FileInfo chartPaths)
        {
            string cellValues = "";
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(chartPaths.FullName))
                {
                    // Read the stream to a string, and write the string to the console.
                     cellValues = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                //TODO Log4Net
            }
            return parseBlockV2.BuildFromTxt(cellValues);
        }
    }
}
