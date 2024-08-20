using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Raven35.Abstractions.Util;
using Raven35.Imports.Newtonsoft.Json;
using Raven35.Imports.Newtonsoft.Json.Linq;
using Raven35.Json.Linq;
using CsvHelper;
using CsvHelper.Configuration;

namespace Raven35.Abstractions.Streaming
{
    public class ExcelOutputWriter : IOutputWriter
    {
        private const string CsvContentType = "text/csv";

        private readonly Stream stream;
        private readonly string[] customColumns;
        private IEnumerable<string> properties;
        private StreamWriter writer;
        private CsvWriter csvWriter;
        private bool doIncludeId;

        public ExcelOutputWriter(Stream stream, string[] customColumns = null)
        {
            this.stream = stream;
            this.customColumns = customColumns;
        }

        public string ContentType => CsvContentType;

        public void Dispose()
        {
            if (writer == null)
                return;

            writer.Flush();
            stream.Flush();

            csvWriter?.Dispose();
        }

        public void WriteHeader()
        {
            writer = new StreamWriter(stream, Encoding.UTF8);
            csvWriter = new CsvWriter(writer);
        }

        public void Write(RavenJObject result)
        {
            if (properties == null)
            {
                GetPropertiesAndWriteCsvHeader(result, out doIncludeId);
                Debug.Assert(properties != null);
            }

            if (doIncludeId)
            {
                RavenJToken token;
                if (result.TryGetValue("@metadata", out token))
                {
                    var metadata = token as RavenJObject;
                    if (metadata != null)
                    {
                        if (metadata.TryGetValue("@id", out token))
                        {
                            csvWriter.WriteField(token.Value<string>());
                        }
                    }
                }
            }

            foreach (var property in properties)
            {
                var token = result.SelectToken(property);
                if (token == null)
                {
                    csvWriter.WriteField(null);
                    continue;
                }

                switch (token.Type)
                {
                    case JTokenType.Null:
                        csvWriter.WriteField(null);
                        break;

                    case JTokenType.Array:
                    case JTokenType.Object:
                        csvWriter.WriteField(token.ToString(Formatting.None));
                        break;

                    default:
                        csvWriter.WriteField(token.Value<string>());
                        break;
                }
            }
        
            csvWriter.NextRecord();
        }

        public void Write(string result)
        {
            csvWriter.WriteField(result);
            csvWriter.NextRecord();
        }

        public void WriteError(Exception exception)
        {
            writer.WriteLine();
            writer.WriteLine();
            writer.WriteLine(exception.ToString());
        }

        public void Flush()
        {
            writer.Flush();
        }

        private void GetPropertiesAndWriteCsvHeader(RavenJObject result, out bool includeId)
        {
            includeId = false;

            if (customColumns == null || customColumns.Length == 0)
            {
                properties = DocumentHelpers.GetPropertiesFromJObject(result,
                    parentPropertyPath: "",
                    includeNestedProperties: true,
                    includeMetadata: false,
                    excludeParentPropertyNames: true).ToList();
            }
            else
            {
                properties = customColumns;
            }

            RavenJToken token;
            if (result.TryGetValue("@metadata", out token))
            {
                var metadata = token as RavenJObject;
                if (metadata != null)
                {
                    if (metadata.TryGetValue("@id", out token))
                    {
                        csvWriter.WriteField("@id");
                        includeId = true;
                    }
                }
            }

            foreach (var property in properties)
            {
                csvWriter.WriteField(property);
            }

            csvWriter.NextRecord();
        }

    }
}