using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Speedo.Feature.ICanHazDadJokeProvider
{
    public class ICanHazDadJokeClient
    {
        /// <summary>
		/// The URL of the service (https://icanhazdadjoke.com/).
		/// </summary>
		public const string BaseUrl = "https://icanhazdadjoke.com/";

        private const string NoUserAgentMessage =
            "ICanHazDadJoke Kindly request that contact details in form of a website or email is supplied to requests to their free API. See https://icanhazdadjoke.com/api#custom-user-agent for more info.";

        private const string RandomJokeUrl = "/";
        private const string JokeUrl = "/j/{0}";

        private HttpClient client;

        /// <summary>
        /// Initializes a client to access the ICanHazDadJoke API
        /// </summary>
        /// <param name="site">Site that ICanHazDadJoke can use if to get in touch with the ones using the application</param>
        /// @TODO: Make as a setting instead if time
        public ICanHazDadJokeClient(string site)
        {
            if (string.IsNullOrWhiteSpace(site))
                throw new ArgumentException(NoUserAgentMessage, nameof(site));

            client = Init(site);
        }

        /// <summary>
        /// Initiate a client to access the ICanHazDadJoke API with
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        private HttpClient Init(string site)
        {
            client = new HttpClient {BaseAddress = new Uri(BaseUrl)};
            client.DefaultRequestHeaders.UserAgent.TryParseAdd($"Speedo: https://github.com/Sitecore-Hackathon/2021-Anonymous-Sitecoreholics - ({site})");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            return client;
        }

        /// <summary>
        /// Gets a random joke from ICanHazDadJoke
        /// </summary>
        /// <returns>A random joke deserialized</returns>
        public async Task<ICanHazDadJokeModel> GetRandomJokeAsync()
        {
            var response = await client.GetStringAsync(RandomJokeUrl).ConfigureAwait(false);
            
            return JsonConvert.DeserializeObject<ICanHazDadJokeModel>(response);
        }

        /// <summary>
        /// Fetches a random joke.
        /// </summary>
        /// <returns>A random joke as an png</returns>
        public async Task<Stream> GetRandomJokeAsImageAsync()
        {
            var response = await client.GetStringAsync(RandomJokeUrl).ConfigureAwait(false);
            var joke = JsonConvert.DeserializeObject<ICanHazDadJokeModel>(response);
            return await GetJokeAsImageAsync(joke.Id);
        }

        /// <summary>
        /// Gets a Joke from ICanHazDadJoke
        /// </summary>
        /// <returns>The specified joke deserialized</returns>
        /// <param name="id">The system id of the Joke at ICanHazDadJoke</param>
        public async Task<ICanHazDadJokeModel> GetJokeAsync(string id)
        {
            var uri = string.Format(JokeUrl, id);
            var response = await client.GetStringAsync(uri).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<ICanHazDadJokeModel>(response);
        }

        /// <summary>
        /// Gets a Joke from ICanHazDadJoke as an Image
        /// </summary>
        /// <returns>The specified joke as a png</returns>
        /// <param name="id">The system id of the Joke at ICanHazDadJoke</param>
        public async Task<Stream> GetJokeAsImageAsync(string id)
        {
            var uri = string.Format(JokeUrl, id + ".png");
            var response = await client.GetStreamAsync(uri).ConfigureAwait(false);
            return response;
        }
    }
}
