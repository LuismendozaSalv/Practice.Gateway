using Newtonsoft.Json;
using Ocelot.Configuration;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using Practice.Gateway.Dto;
using System.IO.Compression;
using System.Net;

namespace Practice.Gateway.Aggregator
{
    public class StaffPropertyAggregator : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            if (responses.Count == 0)
            {
                return new DownstreamResponse(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            if (responses[0].Items.Errors().Count > 0)
            {
                return new DownstreamResponse(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
            List<Staff> staff = new List<Staff>();
            List<Properties> properties = new List<Properties>();

            foreach (var response in responses)
            {
                string downStreamRouteKey = ((DownstreamRoute)response.Items["DownstreamRoute"]).Key;
                DownstreamResponse downstreamResponse = (DownstreamResponse)response.Items["DownstreamResponse"];
                string downstreamResponseContent = await downstreamResponse.Content.ReadAsStringAsync();

                if (downStreamRouteKey == "staff")
                {
                    staff = JsonConvert.DeserializeObject<List<Staff>>(downstreamResponseContent);
                }

                if (downStreamRouteKey == "properties")
                {
                    properties = JsonConvert.DeserializeObject<List<Properties>>(downstreamResponseContent);
                }
            }
            staff[0].props = properties;
            var stringContent = new StringContent(JsonConvert.SerializeObject(staff))
            {
                Headers = { ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json") }
            };
            return new DownstreamResponse(stringContent,
                HttpStatusCode.OK, new List<Header>(), "OK");
        }

        private string DeCompressBrotli(byte[] xResponseContent)
        {
            byte[] decompressedData;
            using (var compressedStream = new MemoryStream(xResponseContent))
            using (var decompressedStream = new MemoryStream())
            using (var brotliStream = new BrotliStream(compressedStream, CompressionMode.Decompress))
            {
                brotliStream.CopyTo(decompressedStream);
                decompressedData = decompressedStream.ToArray();
            }

            return System.Text.Encoding.UTF8.GetString(decompressedData);
        }
    }
}
