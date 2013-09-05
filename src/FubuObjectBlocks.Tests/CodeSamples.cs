using System.Collections.Generic;
using FubuCore;

namespace FubuObjectBlocks.Tests
{
    public class CodeSamples
    {
        // SAMPLE: Solution
        public class Solution
        {
            public SolutionOptions Options { get; set; }

            [BlockSettings(ExpressAs = "feed", ImplicitProperty = "Url")]
            public IEnumerable<Feed> Feeds { get; set; }
        }

        public class SolutionName
        {
            public SolutionName()
            {
            }

            public SolutionName(string name)
            {
                Name = name;
            }

            public string Name { get; private set; }
        }

        public class SolutionOptions
        {
            [ImplicitValue]
            public SolutionName Name { get; set; }
            public string Nuspecs { get; set; }
            public string SrcFolder { get; set; }
            public string BuildCmd { get; set; }
            public string FastBuildCommand { get; set; }

            public SolutionConstraints Constraints { get; set; }
        }

        public class SolutionConstraints
        {
            public string Float { get; set; }
            public string Fixed { get; set; }
        }

        public class Feed
        {
            public string Url { get; set; }
            public string Mode { get; set; }
            public string Stability { get; set; }
        }
        // ENDSAMPLE

        // SAMPLE: Deserialize
        public void deserialize()
        {
            var fileSystem = new FileSystem();
            
            // You can also register IObjectBlockSerializer/ObjectBlockSerializer
            // into your container
            var serializer = ObjectBlockSerializer.Basic();

            var contents = fileSystem.ReadStringFromFile("syntax.txt");
            var solution = serializer.Deserialize<Solution>(contents);
        }
        // ENDSAMPLE

        // SAMPLE: Serialize
        public void serialize()
        {
            var solution = new Solution {Options = new SolutionOptions {Name = new SolutionName("test")}};
            var serializer = ObjectBlockSerializer.Basic();

            var serializedString = serializer.Serialize(solution);
        }
        // ENDSAMPLE

        // SAMPLE: SerializeSettings
        public class SolutionBlockSettings : ObjectBlockSettings<Solution>
        {
            public SolutionBlockSettings()
            {
                //Question: calling .ImplicitValue here happens in addition to 
                Collection(x => x.Feeds)
                    .ExpressAs("feed")
                    .ImplicitValue(x => x.Url);
            }
        }

        public void serialize_with_settings()
        {
            var solution = new Solution { Options = new SolutionOptions { Name = new SolutionName("test") } };
            var serializer = ObjectBlockSerializer.Basic();

            var serializedString = serializer.Serialize<Solution, SolutionBlockSettings>(solution);
        }
        // ENDSAMPLE
    }
}