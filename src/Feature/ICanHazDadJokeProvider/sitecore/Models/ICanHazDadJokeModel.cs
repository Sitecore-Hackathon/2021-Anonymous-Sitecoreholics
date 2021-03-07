using Newtonsoft.Json;

namespace Speedo.Feature.ICanHazDadJokeProvider
{
    /// <summary>
    /// Represents a dad joke.
    /// </summary>
    public class ICanHazDadJokeModel
    {
        /// <summary>
        /// Gets or sets the ID of a joke.
        /// </summary>
        /// <value>The joke ID.</value>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the joke.
        /// </summary>
        /// <value>The joke.</value>
        [JsonProperty("joke")]
        public string DadJoke { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        [JsonProperty("status")]
        public int Status { get; set; }
    }
}

