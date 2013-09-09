using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuObjectBlocks.Tests
{
    [TestFixture]
    public class roundtrip_serialization
    {
        [Test]
        public void reads_and_writes()
        {
            var solution = new Solution
            {
                Options = new SolutionOptions
                {
                    Name = new SolutionName("ripple"),
                    Nuspecs = "packaging/nuget",
                    SrcFolder = "src",
                    BuildCmd = "rake",
                    FastBuildCommand = "rake compile",
                    Constraints = new SolutionConstraints
                    {
                        Float = "current",
                        Fixed = "current,nextMajor"
                    }
                },
                Feeds = new[]
                {
                    new Feed
                    {
                        Url = "http://build.fubu-project.org/guestAuth/app/nuget/v1/FeedService.svc",
                        Mode = "float",
                        Stability = "released"
                    },
                    new Feed {Url = "http://nuget.org/api/v2", Mode = "fixed", Stability = "released"}
                }
            };


            var serializer = ObjectBlockSerializer.Basic();
            var writer = ObjectBlockWriter.Basic();

            var output = writer.Write(solution);

            Debug.WriteLine(output);

            var newSolution = serializer.Deserialize<Solution>(output);

            newSolution.ShouldEqual(solution);
        }

        public class Solution
        {
            public SolutionOptions Options { get; set; }

            [BlockSettings(ExpressAs = "feed", ImplicitProperty = "Url")]
            public IEnumerable<Feed> Feeds { get; set; }

            protected bool Equals(Solution other)
            {
                return Options.Equals(other.Options) && Feeds.SequenceEqual(other.Feeds);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Solution)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Options.GetHashCode() * 397) ^ Feeds.GetHashCode();
                }
            }
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

            public override string ToString()
            {
                return Name;
            }

            protected bool Equals(SolutionName other)
            {
                return string.Equals(Name, other.Name);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((SolutionName)obj);
            }

            public override int GetHashCode()
            {
                return Name.GetHashCode();
            }
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

            protected bool Equals(SolutionOptions other)
            {
                return string.Equals(Name, other.Name) && string.Equals(Nuspecs, other.Nuspecs) &&
                       string.Equals(SrcFolder, other.SrcFolder) && string.Equals(BuildCmd, other.BuildCmd) &&
                       string.Equals(FastBuildCommand, other.FastBuildCommand) && Constraints.Equals(other.Constraints);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((SolutionOptions)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = Name.GetHashCode();
                    hashCode = (hashCode * 397) ^ Nuspecs.GetHashCode();
                    hashCode = (hashCode * 397) ^ SrcFolder.GetHashCode();
                    hashCode = (hashCode * 397) ^ BuildCmd.GetHashCode();
                    hashCode = (hashCode * 397) ^ FastBuildCommand.GetHashCode();
                    hashCode = (hashCode * 397) ^ Constraints.GetHashCode();
                    return hashCode;
                }
            }
        }

        public class SolutionConstraints
        {
            public string Float { get; set; }
            public string Fixed { get; set; }

            protected bool Equals(SolutionConstraints other)
            {
                return string.Equals(Float, other.Float) && string.Equals(Fixed, other.Fixed);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((SolutionConstraints)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Float.GetHashCode() * 397) ^ Fixed.GetHashCode();
                }
            }

        }

        //TODO: refactor these to use AllPropertyValues.AreEqual and AllPropertyValues.HashCode
        //once they get included with FubuCore
        public class Feed
        {
            public string Url { get; set; }
            public string Mode { get; set; }
            public string Stability { get; set; }

            protected bool Equals(Feed other)
            {
                return string.Equals(Url, other.Url) && string.Equals(Mode, other.Mode) &&
                       string.Equals(Stability, other.Stability);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((Feed)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = Url.GetHashCode();
                    hashCode = (hashCode * 397) ^ Mode.GetHashCode();
                    hashCode = (hashCode * 397) ^ Stability.GetHashCode();
                    return hashCode;
                }
            }
        }
    }

}