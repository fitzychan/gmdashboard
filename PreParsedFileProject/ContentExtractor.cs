using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices;
using CommonBlocks;
using System.Xml.Linq;
using System.Windows;

namespace DmAssistant.BlockBuilder
{
    public interface IContentExtractor
    {
        IMainBlock ExtractData(FileInfo file);
    }

    public class ContentExtractor : IContentExtractor
    {
        protected IBlockBuilder parseBlock;
        protected IBlockBuilder parseBlockV2;

        public ContentExtractor()
        {
            parseBlock = new BlockBuilder();
            parseBlockV2 = new BlockBuilderV2();
        }


        public IMainBlock ExtractData(FileInfo file)
        {
            switch (file.Extension)
            {
                case ".txt":
                    {
                        return TxtExtractor(file);
                    }
                case ".xlsx":
                    {
                        return XmlExtractor(file);
                    }
                case ".csv":
                    {
                        return CsvExtractor(file);
                    }
                case ".rgf":
                    {
                        return RollMeOneFormatExtractor(file);
                    }
            }
            return new MainBlock();

        }
        private IMainBlock CsvExtractor(FileInfo chartPath)
        {
            throw new NotImplementedException();
        }
        private IMainBlock RollMeOneFormatExtractor(FileInfo chartPath)
        {
            return parseBlock.BuildFromRgf(XDocument.Load(chartPath.FullName));
        }
        private IMainBlock XmlExtractor(FileInfo chartPath)
        {
            throw new NotImplementedException();
        }

        public IMainBlock TxtExtractor(FileInfo chartPaths)
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
            var test = parseBlockV2.BuildFromTxt(cellValues);

            return parseBlock.BuildFromTxt(cellValues);
        }
    }
}
