using CommonCode.Blocks;
using CommonCode.Rolls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                        return ParameterExtractor(file);
                    }
            }
            return new Chart();
        }
 
        private IChart ParameterExtractor(FileInfo file)
        {
            var lineFromFile = File.ReadAllLines(file.FullName);
            var listOfParams = new List<string>();

            //We want to make sure there is a comment block to read the params from.
            if(lineFromFile[0].ToCharArray().Take(2) == "<#")
            {

            }
            for(int i = 0; i < lineFromFile.Length; i++)
            {

                var line = lineFromFile[i];
                if (line.Contains(".PARAMETER"))
                {
                    while(line.ToCharArray()[0] != '.' )
                    {

                    }
                }
            }


            return new Chart();
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
