using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MinusEngine
{
    public class MessageResult
    {
        [JsonProperty("body")]
        public String Body { get; set; }

        [JsonProperty("read")]
        public Boolean Read { get; set; }

        [JsonProperty("target")]
        public String Target { get; set; }

        [JsonProperty("sender")]
        public String Sender { get; set; }

        [JsonProperty("thread")]
        public String Thread { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        public MessageResult(String body, Boolean read, String target, String sender, String thread, DateTime created)
        {
            Body = body;
            Read = read;
            Target = target;
            Sender = sender;
            Thread = thread;
            Created = created;
        }

        public override string ToString()
        {
            return new StringBuilder("oAuthResult{Body=")
                .Append(Body)
                .Append(", Read=")
                .Append(Read)
                .Append(", Target=")
                .Append(Target)
                .Append(", Sender=")
                .Append(Sender)
                .Append(", Thread=")
                .Append(Thread)
                .Append(", Created=")
                .Append(Created)
                .Append('}').ToString();
        }
    }
}
