using Microsoft.AspNetCore.ResponseCompression;
using Newtonsoft.Json;
using Ocelot.Configuration;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using Practice.Gateway.Dto;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace Practice.Gateway.Aggregator
{
    public class UsersPostsAggregator : IDefinedAggregator
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
            List<Post> posts = new List<Post>();
            List<User> users = new List<User>();

            foreach (var response in responses)
            {
                string downStreamRouteKey = ((DownstreamRoute)response.Items["DownstreamRoute"]).Key;
                DownstreamResponse downstreamResponse = (DownstreamResponse)response.Items["DownstreamResponse"];
                byte[] downstreamResponseContent = await downstreamResponse.Content.ReadAsByteArrayAsync();

                if (downStreamRouteKey == "posts")
                {
                    posts = JsonConvert.DeserializeObject<List<Post>>(DeCompressBrotli(downstreamResponseContent));
                }

                if (downStreamRouteKey == "users")
                {
                    users = JsonConvert.DeserializeObject<List<User>>(DeCompressBrotli(downstreamResponseContent));
                }
            }

            var usersPosts = new List<UserResponse>();
            usersPosts = users.Select(p => new UserResponse
            {
                id = p.id,
                name = p.name,
                username = p.username,
                email = p.email,
                posts = posts
                            .Where(post => post.userId == p.id)
                            .Select(post => 
                                new PostResponse { 
                                    id = post.id,
                                    title = post.title,
                                    body = post.body,
                            }).ToList(),
            }).ToList();

            var stringContent = new StringContent(JsonConvert.SerializeObject(usersPosts))
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
