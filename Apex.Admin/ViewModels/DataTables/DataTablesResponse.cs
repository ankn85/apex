using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Apex.Admin.ViewModels.DataTables
{
    public sealed class DataTablesResponse
    {
        private DataTablesResponse(int draw, string errorMessage)
        {
            Draw = draw;
            Error = errorMessage;
        }

        private DataTablesResponse(
            int draw,
            int totalRecords,
            int totalRecordsFiltered,
            object data)
        {
            Draw = draw;
            TotalRecords = totalRecords;
            TotalRecordsFiltered = totalRecordsFiltered;
            Data = data;
        }

        public int Draw { get; private set; }

        public string Error { get; private set; }

        public int TotalRecords { get; private set; }

        public int TotalRecordsFiltered { get; private set; }

        public object Data { get; private set; }

        public static DataTablesResponse Create(
            DataTablesRequest request,
            int totalRecords,
            int totalRecordsFiltered,
            object data)
        {
            if (request == null || request.Draw < 1)
            {
                return null;
            }

            return new DataTablesResponse(request.Draw, totalRecords, totalRecordsFiltered, data);
        }

        public static DataTablesResponse Create(DataTablesRequest request, string errorMessage)
        {
            if (request == null || request.Draw < 1)
            {
                return null;
            }

            return new DataTablesResponse(request.Draw, errorMessage);
        }

        public override string ToString()
        {
            using (var stringWriter = new System.IO.StringWriter())
            {
                using (var jsonWriter = new JsonTextWriter(stringWriter))
                {
                    // Start json object.
                    jsonWriter.WriteStartObject();

                    // Draw
                    jsonWriter.WritePropertyName("draw", true);
                    jsonWriter.WriteValue(Draw);

                    if (IsSuccessResponse())
                    {
                        // TotalRecords
                        jsonWriter.WritePropertyName("recordsTotal", true);
                        jsonWriter.WriteValue(TotalRecords);

                        // TotalRecordsFiltered
                        jsonWriter.WritePropertyName("recordsFiltered", true);
                        jsonWriter.WriteValue(TotalRecordsFiltered);

                        // Data
                        jsonWriter.WritePropertyName("data", true);
                        jsonWriter.WriteRawValue(SerializeData(Data));
                    }
                    else
                    {
                        // Error
                        jsonWriter.WritePropertyName("error", true);
                        jsonWriter.WriteValue(Error);
                    }

                    // End json object
                    jsonWriter.WriteEndObject();

                    jsonWriter.Flush();

                    return stringWriter.ToString();
                }
            }
        }

        public string SerializeData(object data)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(data, settings);
        }

        private bool IsSuccessResponse()
        {
            return Data != null && string.IsNullOrWhiteSpace(Error);
        }
    }
}
