using Newtonsoft.Json;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.ItemSerializers;
using Sitecore.LayoutService.Serialization.Pipelines.GetFieldSerializer;
using System;
using System.Collections.Generic;
using System.IO;

namespace Speedo.Feature.SitecorePublisher
{
    public class OutOfHttpContextItemSerializer : DefaultItemSerializer
    {
        public OutOfHttpContextItemSerializer(IGetFieldSerializerPipeline getFieldSerializerPipeline) : base(getFieldSerializerPipeline)
        {
        }

        protected override string SerializeItem(Item item, SerializationOptions options)
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                JsonTextWriter val = new JsonTextWriter(stringWriter);

                try
                {
                    val.WriteStartObject();

                    IEnumerable<Field> itemFields = GetItemFields(item);

                    foreach (Field item2 in itemFields)
                    {
                        SerializeField(item2, val, options, int.MaxValue);
                    }

                    val.WriteEndObject();
                }
                finally
                {
                    ((IDisposable)val)?.Dispose();
                }

                return stringWriter.ToString();
            }
        }
    }
}
